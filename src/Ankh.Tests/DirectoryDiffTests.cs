using System;
using System.IO;
using Ankh.Diff.DiffUtils;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class DirectoryDiffTests
    {
        string _root;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), "AnkhDirDiff-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_root);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_root))
                Directory.Delete(_root, true);
        }

        string MakeDirectory(string name)
        {
            string path = Path.Combine(_root, name);
            Directory.CreateDirectory(path);
            return path;
        }

        static DirectoryDiffEntry Find(DirectoryDiffEntries entries, string name)
        {
            foreach (DirectoryDiffEntry entry in entries)
            {
                if (string.Equals(entry.Name, name, StringComparison.OrdinalIgnoreCase))
                    return entry;
            }

            return null;
        }

        [Test]
        public void ReportsSameDifferentAndOneSidedFiles()
        {
            string a = MakeDirectory("A");
            string b = MakeDirectory("B");

            File.WriteAllText(Path.Combine(a, "same.txt"), "same");
            File.WriteAllText(Path.Combine(b, "same.txt"), "same");
            File.WriteAllText(Path.Combine(a, "different.txt"), "left");
            File.WriteAllText(Path.Combine(b, "different.txt"), "rite");
            File.WriteAllText(Path.Combine(a, "only-a.txt"), "a");
            File.WriteAllText(Path.Combine(b, "only-b.txt"), "b");

            var diff = new DirectoryDiff(true, true, true, true, false, false, null);
            DirectoryDiffResults result = diff.Execute(a, b);

            DirectoryDiffEntry same = Find(result.Entries, "same.txt");
            DirectoryDiffEntry different = Find(result.Entries, "different.txt");
            DirectoryDiffEntry onlyA = Find(result.Entries, "only-a.txt");
            DirectoryDiffEntry onlyB = Find(result.Entries, "only-b.txt");

            Assert.That(same, Is.Not.Null);
            Assert.That(same.Different, Is.False);
            Assert.That(same.InA, Is.True);
            Assert.That(same.InB, Is.True);

            Assert.That(different, Is.Not.Null);
            Assert.That(different.Different, Is.True);
            Assert.That(different.InA, Is.True);
            Assert.That(different.InB, Is.True);

            Assert.That(onlyA, Is.Not.Null);
            Assert.That(onlyA.InA, Is.True);
            Assert.That(onlyA.InB, Is.False);

            Assert.That(onlyB, Is.Not.Null);
            Assert.That(onlyB.InA, Is.False);
            Assert.That(onlyB.InB, Is.True);
        }

        [Test]
        public void HonorsVisibilityFlags()
        {
            string a = MakeDirectory("A");
            string b = MakeDirectory("B");

            File.WriteAllText(Path.Combine(a, "same.txt"), "same");
            File.WriteAllText(Path.Combine(b, "same.txt"), "same");
            File.WriteAllText(Path.Combine(a, "different.txt"), "left");
            File.WriteAllText(Path.Combine(b, "different.txt"), "rite");
            File.WriteAllText(Path.Combine(a, "only-a.txt"), "a");
            File.WriteAllText(Path.Combine(b, "only-b.txt"), "b");

            var diff = new DirectoryDiff(false, false, true, false, false, false, null);
            DirectoryDiffResults result = diff.Execute(a, b);

            Assert.That(Find(result.Entries, "different.txt"), Is.Not.Null);
            Assert.That(Find(result.Entries, "same.txt"), Is.Null);
            Assert.That(Find(result.Entries, "only-a.txt"), Is.Null);
            Assert.That(Find(result.Entries, "only-b.txt"), Is.Null);
        }

        [Test]
        public void RecursiveDiffMarksDirectoryWhenChildDiffers()
        {
            string a = MakeDirectory("A");
            string b = MakeDirectory("B");
            string aSub = Path.Combine(a, "sub");
            string bSub = Path.Combine(b, "sub");
            Directory.CreateDirectory(aSub);
            Directory.CreateDirectory(bSub);

            File.WriteAllText(Path.Combine(aSub, "child.txt"), "left");
            File.WriteAllText(Path.Combine(bSub, "child.txt"), "rite");

            var diff = new DirectoryDiff(true, true, true, true, true, false, null);
            DirectoryDiffResults result = diff.Execute(a, b);

            DirectoryDiffEntry sub = Find(result.Entries, "sub");
            Assert.That(sub, Is.Not.Null);
            Assert.That(sub.IsFile, Is.False);
            Assert.That(sub.Different, Is.True);

            DirectoryDiffEntry child = Find(sub.SubEntries, "child.txt");
            Assert.That(child, Is.Not.Null);
            Assert.That(child.Different, Is.True);
        }

        [Test]
        public void IgnoreDirectoryComparisonKeepsDirectoryButClearsItsDifferentFlag()
        {
            string a = MakeDirectory("A");
            string b = MakeDirectory("B");
            string aSub = Path.Combine(a, "sub");
            string bSub = Path.Combine(b, "sub");
            Directory.CreateDirectory(aSub);
            Directory.CreateDirectory(bSub);

            File.WriteAllText(Path.Combine(aSub, "child.txt"), "left");
            File.WriteAllText(Path.Combine(bSub, "child.txt"), "rite");

            var diff = new DirectoryDiff(true, true, true, false, true, true, null);
            DirectoryDiffResults result = diff.Execute(a, b);

            DirectoryDiffEntry sub = Find(result.Entries, "sub");
            Assert.That(sub, Is.Not.Null);
            Assert.That(sub.Different, Is.False);

            DirectoryDiffEntry child = Find(sub.SubEntries, "child.txt");
            Assert.That(child, Is.Not.Null);
            Assert.That(child.Different, Is.True);
        }

        [Test]
        public void FileFilterLimitsComparedFiles()
        {
            string a = MakeDirectory("A");
            string b = MakeDirectory("B");

            File.WriteAllText(Path.Combine(a, "included.txt"), "left");
            File.WriteAllText(Path.Combine(b, "included.txt"), "rite");
            File.WriteAllText(Path.Combine(a, "excluded.bin"), "a");
            File.WriteAllText(Path.Combine(b, "excluded.bin"), "b");

            var filter = new DirectoryDiffFileFilter("*.txt", true);
            var diff = new DirectoryDiff(true, true, true, true, false, false, filter);
            DirectoryDiffResults result = diff.Execute(a, b);

            Assert.That(Find(result.Entries, "included.txt"), Is.Not.Null);
            Assert.That(Find(result.Entries, "excluded.bin"), Is.Null);
        }

        [Test]
        public void FileFilterSupportsMultipleTrimmedPatternsWithoutDuplicates()
        {
            string directory = MakeDirectory("Filter");
            File.WriteAllText(Path.Combine(directory, "b.cs"), "cs");
            File.WriteAllText(Path.Combine(directory, "a.txt"), "txt");
            File.WriteAllText(Path.Combine(directory, "ignored.md"), "md");

            const string filterText = " *.txt ; *.cs ; *.txt ";
            var filter = new DirectoryDiffFileFilter(filterText, true);

            FileInfo[] files = filter.Filter(new DirectoryInfo(directory));
            string[] names = Array.ConvertAll(files, file => file.Name);

            Assert.Multiple(() =>
            {
                Assert.That(filter.Include, Is.True);
                Assert.That(filter.FilterString, Is.EqualTo(filterText));
                CollectionAssert.AreEqual(new[] { "a.txt", "b.cs" }, names);
            });
        }

        [Test]
        public void FileFilterExcludeModeReturnsFilesOutsideMatchingPatterns()
        {
            string directory = MakeDirectory("Filter");
            File.WriteAllText(Path.Combine(directory, "keep.txt"), "keep");
            File.WriteAllText(Path.Combine(directory, "skip.tmp"), "tmp");
            File.WriteAllText(Path.Combine(directory, "skip.log"), "log");

            var filter = new DirectoryDiffFileFilter("*.tmp; *.log", false);

            FileInfo[] files = filter.Filter(new DirectoryInfo(directory));
            string[] names = Array.ConvertAll(files, file => file.Name);

            Assert.Multiple(() =>
            {
                Assert.That(filter.Include, Is.False);
                CollectionAssert.AreEqual(new[] { "keep.txt" }, names);
            });
        }

        [Test]
        public void NonRecursiveDiffTreatsMatchingDirectoriesAsSameWithoutInspectingChildren()
        {
            string a = MakeDirectory("A");
            string b = MakeDirectory("B");
            string aSub = Path.Combine(a, "sub");
            string bSub = Path.Combine(b, "sub");
            Directory.CreateDirectory(aSub);
            Directory.CreateDirectory(bSub);

            File.WriteAllText(Path.Combine(aSub, "child.txt"), "left");
            File.WriteAllText(Path.Combine(bSub, "child.txt"), "right");

            var diff = new DirectoryDiff(true, true, true, true, false, false, null);
            DirectoryDiffResults result = diff.Execute(a, b);

            DirectoryDiffEntry sub = Find(result.Entries, "sub");

            Assert.Multiple(() =>
            {
                Assert.That(sub, Is.Not.Null);
                Assert.That(sub.IsFile, Is.False);
                Assert.That(sub.Different, Is.False);
                Assert.That(sub.SubEntries, Is.Empty);
                Assert.That(result.Recursive, Is.False);
            });
        }

        [Test]
        public void DirectoryDiffEntriesAndResultsPreserveComparisonMetadata()
        {
            string a = MakeDirectory("A");
            string b = MakeDirectory("B");
            var filter = new DirectoryDiffFileFilter("*.txt", true);
            var directory = new DirectoryDiffEntry("sub", false, true, true, false);
            var file = new DirectoryDiffEntry("file.txt", true, true, false, true);

            DirectoryDiffEntries children = directory.SubEntries;
            file.Error = "read failed";
            directory.Different = true;

            var results = new DirectoryDiffResults(
                new DirectoryInfo(a),
                new DirectoryInfo(b),
                children,
                true,
                filter);

            Assert.Multiple(() =>
            {
                Assert.That(directory.Name, Is.EqualTo("sub"));
                Assert.That(directory.IsFile, Is.False);
                Assert.That(directory.InA, Is.True);
                Assert.That(directory.InB, Is.True);
                Assert.That(directory.Different, Is.True);
                Assert.That(directory.SubEntries, Is.SameAs(children));

                Assert.That(file.Name, Is.EqualTo("file.txt"));
                Assert.That(file.IsFile, Is.True);
                Assert.That(file.InA, Is.True);
                Assert.That(file.InB, Is.False);
                Assert.That(file.Different, Is.True);
                Assert.That(file.Error, Is.EqualTo("read failed"));
                Assert.That(file.SubEntries, Is.Null);

                Assert.That(results.A.FullName, Is.EqualTo(new DirectoryInfo(a).FullName));
                Assert.That(results.B.FullName, Is.EqualTo(new DirectoryInfo(b).FullName));
                Assert.That(results.Entries, Is.SameAs(children));
                Assert.That(results.Recursive, Is.True);
                Assert.That(results.Filter, Is.SameAs(filter));
            });
        }

        [Test]
        public void ComparingDirectoryToItselfSkipsFileContentComparison()
        {
            string a = MakeDirectory("A");
            File.WriteAllText(Path.Combine(a, "same.txt"), "same");

            var diff = new DirectoryDiff(true, true, true, true, false, false, null);
            DirectoryDiffResults result = diff.Execute(a, a);

            DirectoryDiffEntry entry = Find(result.Entries, "same.txt");
            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.Different, Is.False);
        }
    }
}
