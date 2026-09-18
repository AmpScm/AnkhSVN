using System;
using System.Linq;
using System.Reflection;
using Ankh.Scc.UI;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class AnkhDiffTemplateTests
    {
        static object CreateAnkhDiff()
        {
            Type diffType = Assembly.Load("Ankh").GetType("Ankh.Services.AnkhDiff", true);
            return Activator.CreateInstance(
                diffType,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new object[] { new AnkhServiceContainer() },
                null);
        }

        static string Substitute(object diff, string template, AnkhDiffToolArgs args, string mode)
        {
            Type diffType = diff.GetType();
            Type modeType = diffType.GetNestedType("DiffToolMode", BindingFlags.NonPublic);
            object modeValue = Enum.Parse(modeType, mode);

            MethodInfo method = diffType.GetMethod(
                "SubstituteArguments",
                BindingFlags.Instance | BindingFlags.NonPublic);

            return (string)method.Invoke(diff, new object[] { template, args, modeValue });
        }

        [Test]
        public void DiffTemplateExpandsFilesTitlesAndReadOnlyState()
        {
            object diff = CreateAnkhDiff();
            var args = new AnkhDiffArgs
            {
                BaseFile = @"C:\repo\base.txt",
                BaseTitle = "Base title",
                MineFile = @"C:\repo\mine.txt",
                MineTitle = "Mine title",
                ReadOnly = true
            };

            string result = Substitute(
                diff,
                "$(Base)|$(BaseName)|$(Mine)|$(MineName)|$(ReadOnly)",
                args,
                "Diff");

            Assert.That(
                result,
                Is.EqualTo(@"C:\repo\base.txt|Base title|C:\repo\mine.txt|Mine title|1"));
        }

        [Test]
        public void DiffTemplateFallsBackToFileNamesAndHandlesWritableState()
        {
            object diff = CreateAnkhDiff();
            var args = new AnkhDiffArgs
            {
                BaseFile = @"C:\repo\base.txt",
                MineFile = @"C:\repo\mine.txt",
                ReadOnly = false
            };

            string result = Substitute(
                diff,
                "$(BName)|$(YName)|$(ReadOnly)",
                args,
                "Diff");

            Assert.That(result, Is.EqualTo("base.txt|mine.txt|"));
        }

        [Test]
        public void MergeTemplateExpandsTheirsAndMergedValues()
        {
            object diff = CreateAnkhDiff();
            var args = new AnkhMergeArgs
            {
                BaseFile = @"C:\repo\base.txt",
                MineFile = @"C:\repo\mine.txt",
                TheirsFile = @"C:\repo\theirs.txt",
                TheirsTitle = "Incoming",
                MergedFile = @"C:\repo\merged.txt",
                MergedTitle = "Result"
            };

            string result = Substitute(
                diff,
                "$(Theirs)|$(TheirsName)|$(Merged)|$(MergedName)",
                args,
                "Merge");

            Assert.That(
                result,
                Is.EqualTo(@"C:\repo\theirs.txt|Incoming|C:\repo\merged.txt|Result"));
        }

        [Test]
        public void PatchTemplateExpandsPatchAndDestination()
        {
            object diff = CreateAnkhDiff();
            var args = new AnkhPatchArgs
            {
                PatchFile = @"C:\temp\change.patch",
                ApplyTo = @"C:\repo"
            };

            string result = Substitute(
                diff,
                "$(PatchFile)|$(ApplyToDir)|$(Base)",
                args,
                "Patch");

            Assert.That(result, Is.EqualTo(@"C:\temp\change.patch|C:\repo|"));
        }

        [Test]
        public void ConditionalTemplateUsesArgumentTruthiness()
        {
            object diff = CreateAnkhDiff();

            var readOnly = new AnkhDiffArgs
            {
                BaseFile = "base.txt",
                MineFile = "mine.txt",
                ReadOnly = true
            };

            var writable = new AnkhDiffArgs
            {
                BaseFile = "base.txt",
                MineFile = "mine.txt",
                ReadOnly = false
            };

            Assert.That(
                Substitute(diff, "$(ReadOnly?'--readonly':'--writable')", readOnly, "Diff"),
                Is.EqualTo("--readonly"));

            Assert.That(
                Substitute(diff, "$(ReadOnly?'--readonly':'--writable')", writable, "Diff"),
                Is.EqualTo("--writable"));
        }

        [Test]
        public void ResolveConflictOnTemplateSetsMergeExitCodes()
        {
            object diff = CreateAnkhDiff();
            var args = new AnkhMergeArgs
            {
                BaseFile = "base.txt",
                MineFile = "mine.txt",
                TheirsFile = "theirs.txt",
                MergedFile = "merged.txt"
            };

            string result = Substitute(
                diff,
                "$(ResolveConflictOn='0, 1, invalid, 5')",
                args,
                "Merge");

            Assert.That(result, Is.Empty);
            Assert.That(args.GetMergedExitCodes(), Is.EqualTo(new[] { 0, 1, 5 }));
        }

        [Test]
        public void EnvironmentTemplatesProduceValuesAndUnknownKeysAreEmpty()
        {
            object diff = CreateAnkhDiff();
            var args = new AnkhDiffArgs
            {
                BaseFile = "base.txt",
                MineFile = "mine.txt"
            };

            string result = Substitute(
                diff,
                "$(AppData)|$(LocalAppData)|$(ProgramFiles)|$(CommonProgramFiles)|$(HostProgramFiles)|$(DoesNotExist)",
                args,
                "Diff");

            string[] values = result.Split('|');

            Assert.That(values, Has.Length.EqualTo(6));
            Assert.That(values.Take(5), Has.All.Not.Null.And.Not.Empty);
            Assert.That(values[5], Is.Empty);
        }
    }
}
