using System;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Ankh.Diff;
using Ankh.Diff.DiffUtils;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class DiffUtilityFunctionsTests
    {
        [Test]
        public void BinaryDetectionDistinguishesAsciiNullBytesAndUnicodeBom()
        {
            string dir = Path.Combine(Path.GetTempPath(), "ankh-diff-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                string ascii = Path.Combine(dir, "ascii.txt");
                string binary = Path.Combine(dir, "binary.bin");
                string unicode = Path.Combine(dir, "unicode.txt");

                File.WriteAllBytes(ascii, Encoding.Default.GetBytes("alpha\r\nbeta"));
                File.WriteAllBytes(binary, new byte[] { 65, 66, 0, 67 });
                File.WriteAllText(unicode, "hello", Encoding.Unicode);

                Assert.Multiple(() =>
                {
                    Assert.That(Functions.IsBinaryFile(ascii), Is.False);
                    Assert.That(Functions.IsBinaryFile(binary), Is.True);
                    Assert.That(Functions.IsBinaryFile(unicode), Is.False);
                });

                using (FileStream stream = File.OpenRead(ascii))
                    Assert.That(Functions.IsBinaryFile(stream), Is.False);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Test]
        public void FileComparisonCoversLengthDifferenceContentDifferenceAndEquality()
        {
            string dir = Path.Combine(Path.GetTempPath(), "ankh-file-compare-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                string one = Path.Combine(dir, "one.txt");
                string two = Path.Combine(dir, "two.txt");
                string three = Path.Combine(dir, "three.txt");
                string four = Path.Combine(dir, "four.txt");

                File.WriteAllText(one, "abc");
                File.WriteAllText(two, "abcd");
                File.WriteAllText(three, "abd");
                File.WriteAllText(four, "abc");

                Assert.Multiple(() =>
                {
                    Assert.That(Functions.AreFilesDifferent(one, two), Is.True);
                    Assert.That(Functions.AreFilesDifferent(new FileInfo(one), new FileInfo(three)), Is.True);
                    Assert.That(Functions.AreFilesDifferent(one, four), Is.False);
                });
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [Test]
        public void TextLineHelpersHandleEmptySingleAndMultipleLines()
        {
            CollectionAssert.IsEmpty(Functions.GetStringTextLines(""));
            CollectionAssert.AreEqual(
                new[] { "one" },
                Functions.GetStringTextLines("one"));
            CollectionAssert.AreEqual(
                new[] { "one", "two", "" },
                Functions.GetStringTextLines("one\r\ntwo\r\n\r\n"));
        }

        [Test]
        public void XmlTextFormattingCoversDeclarationsAttributesCommentsCdataAndWhitespace()
        {
            const string xml =
                "<?xml version=\"1.0\"?>" +
                "<root id=\"7\">" +
                "<!-- first\n second -->" +
                "<empty value=\"x\"/>" +
                "<![CDATA[cdata text]]>" +
                "<?sample instruction?>" +
                " text " +
                "</root>";

            StringCollection significant = Functions.GetXMLTextLinesFromXML(
                xml, WhitespaceHandling.Significant);
            StringCollection all = Functions.GetXMLTextLinesFromXML(
                "<root> \n <child/> \n </root>",
                WhitespaceHandling.All);
            StringCollection none = Functions.GetXMLTextLinesFromXML(
                "<root> \n <child/> \n </root>",
                WhitespaceHandling.None);

            Assert.Multiple(() =>
            {
                Assert.That(significant.Count, Is.GreaterThan(5));
                Assert.That(string.Join("\n", ToArray(significant)), Does.Contain("<?xml"));
                Assert.That(string.Join("\n", ToArray(significant)), Does.Contain("<!--"));
                Assert.That(string.Join("\n", ToArray(significant)), Does.Contain("<empty value=\"x\"/>"));
                Assert.That(string.Join("\n", ToArray(significant)), Does.Contain("<![CDATA[cdata text]]>"));
                Assert.That(string.Join("\n", ToArray(significant)), Does.Contain("<?sample instruction?>"));
                Assert.That(all.Count, Is.GreaterThan(none.Count));
            });
        }

        [Test]
        public void XmlFormattingCoversDocumentTypeAndDeepIndentation()
        {
            const string xml =
                "<!DOCTYPE root [<!ELEMENT root (a)><!ELEMENT a (b)><!ELEMENT b (c)>" +
                "<!ELEMENT c (d)><!ELEMENT d (e)><!ELEMENT e (f)><!ELEMENT f (g)>" +
                "<!ELEMENT g (h)><!ELEMENT h (i)><!ELEMENT i (#PCDATA)>]>" +
                "<root><a><b><c><d><e><f><g><h><i>deep</i></h></g></f></e></d></c></b></a></root>";

            StringCollection lines = Functions.GetXMLTextLinesFromXML(xml, WhitespaceHandling.None);
            string joined = string.Join("\n", ToArray(lines));

            Assert.Multiple(() =>
            {
                Assert.That(joined, Does.Contain("<!DOCTYPE root ["));
                Assert.That(joined, Does.Contain("deep"));
                Assert.That(joined, Does.Contain("\t\t\t\t\t\t\t\t\t<i>"));
            });
        }

        [Test]
        public void QuoteHelpersHandleMissingOneAndBothQuotes()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Utilities.EnsureQuotes("value"), Is.EqualTo("\"value\""));
                Assert.That(Utilities.EnsureQuotes("\"value"), Is.EqualTo("\"value\""));
                Assert.That(Utilities.EnsureQuotes("value\""), Is.EqualTo("\"value\""));
                Assert.That(Utilities.EnsureQuotes("\"value\""), Is.EqualTo("\"value\""));

                Assert.That(Utilities.StripQuotes("\"value\""), Is.EqualTo("value"));
                Assert.That(Utilities.StripQuotes("\"value"), Is.EqualTo("value"));
                Assert.That(Utilities.StripQuotes("value\""), Is.EqualTo("value"));
                Assert.That(Utilities.StripQuotes("value"), Is.EqualTo("value"));

                Assert.That(Utilities.EnsureQuotes("value", "'"), Is.EqualTo("'value'"));
                Assert.That(Utilities.StripQuotes("[value]", "[", "]"), Is.EqualTo("value"));
            });
        }

        [Test]
        public void CaseInsensitiveSearchAndReplacementCoverNoMatchAndRepeatedMatches()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Utilities.IndexOfNoCase("Alpha Beta", "beta"), Is.EqualTo(6));
                Assert.That(Utilities.IndexOfNoCase("Alpha Beta Beta", "BETA", 7), Is.EqualTo(11));
                Assert.That(Utilities.IndexOfNoCase("Alpha Beta", "PHA", 1, 4), Is.EqualTo(2));

                Assert.That(
                    Utilities.ReplaceNoCase("Alpha beta ALPHA", "alpha", "X"),
                    Is.EqualTo("X beta X"));
                Assert.That(
                    Utilities.ReplaceNoCase("unchanged", "missing", "X"),
                    Is.EqualTo("unchanged"));
                Assert.That(
                    Utilities.ReplaceNoCase("aaaa", "aa", "X"),
                    Is.EqualTo("XX"));
            });
        }

        [Test]
        public void ControlCharacterReplacementHandlesEmptyCleanAndDirtyStrings()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Utilities.ReplaceControlCharacters(""), Is.Empty);
                Assert.That(Utilities.ReplaceControlCharacters("clean"), Is.EqualTo("clean"));
                Assert.That(
                    Utilities.ReplaceControlCharacters("a\tb\r\nc"),
                    Is.EqualTo("a b  c"));
                Assert.That(
                    Utilities.ReplaceControlCharacters("a\tb", '_'),
                    Is.EqualTo("a_b"));
            });
        }

        [Test]
        public void GeometryAndComparisonHelpersCoverToleranceBoundaries()
        {
            Assert.Multiple(() =>
            {
                Assert.That(Utilities.Compare(1.0f, 1.05f, 0.1f), Is.Zero);
                Assert.That(Utilities.Compare(2.0f, 1.0f, 0.1f), Is.EqualTo(1));
                Assert.That(Utilities.Compare(0.0f, 1.0f, 0.1f), Is.EqualTo(-1));
                Assert.That(Utilities.CompareEqual(
                    new PointF(1, 2), new PointF(1.05f, 2.05f), 0.1f), Is.True);
                Assert.That(Utilities.CompareEqual(
                    new PointF(1, 2), new PointF(2, 2), 0.1f), Is.False);
                Assert.That(Utilities.Between(5, 4, 6, 0), Is.True);
                Assert.That(Utilities.Between(3, 4, 6, 0), Is.False);
                Assert.That(Utilities.Between(7, 4, 6, 0), Is.False);
                Assert.That(Utilities.GetAngle(new PointF(0, 0), new PointF(1, 0)), Is.EqualTo(0).Within(0.001));
                Assert.That(Utilities.GetAngle(new PointF(0, 0), new PointF(0, 1)), Is.EqualTo(90).Within(0.001));
                Assert.That(Utilities.IsEmpty(null), Is.True);
                Assert.That(Utilities.IsEmpty(""), Is.True);
                Assert.That(Utilities.IsEmpty("x"), Is.False);
            });
        }

        [Test]
        public void AssemblyVersionAndSoundNameMappingAreStable()
        {
            string version = Utilities.GetAssemblyVersion(typeof(Utilities).Assembly);
            Assert.That(version.Split('.').Length, Is.EqualTo(4));

            MethodInfo method = typeof(Utilities).GetMethod(
                "GetSoundFileName",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.That(method, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(InvokeSound(method, SystemSound.Error), Is.EqualTo("SystemHand"));
                Assert.That(InvokeSound(method, SystemSound.Question), Is.EqualTo("SystemQuestion"));
                Assert.That(InvokeSound(method, SystemSound.Warning), Is.EqualTo("SystemExclamation"));
                Assert.That(InvokeSound(method, SystemSound.Information), Is.EqualTo("SystemAsterisk"));
                Assert.That(InvokeSound(method, SystemSound.Default), Is.EqualTo("SystemDefault"));
                Assert.That(InvokeSound(method, (SystemSound)999), Is.EqualTo(".Default"));
            });
        }

        static string[] ToArray(StringCollection collection)
        {
            string[] values = new string[collection.Count];
            collection.CopyTo(values, 0);
            return values;
        }

        static string InvokeSound(MethodInfo method, SystemSound sound)
        {
            return (string)method.Invoke(null, new object[] { sound });
        }
    }
}
