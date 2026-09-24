// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;

using Ankh.Diff.DiffUtils;
using Ankh.Diff.DiffUtils.Controls;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Diff
{
    [TestFixture]
    public class DiffAlgorithmCoverageTests
    {
        [Test]
        public void MyersDiffFindsReplacementLongestCommonSubsequence()
        {
            var diff = new MyersDiff(
                new[] { 1, 2, 3 },
                new[] { 1, 9, 3 });

            CollectionAssert.AreEqual(new[] { 1, 3 }, diff.GetLCS());

            Assert.Multiple(() =>
            {
                Assert.That(diff.GetLCSLength(), Is.EqualTo(2));
                Assert.That(diff.GetSESLength(), Is.EqualTo(2));
                Assert.That(diff.GetReverseSESLength(), Is.EqualTo(2));
                Assert.That(diff.GetSimilarity(), Is.EqualTo(2.0 / 3.0).Within(0.000001));
                Assert.That(diff.Execute().TotalEditLength, Is.EqualTo(2));
            });
        }

        [Test]
        public void MyersDiffHandlesEmptyAndInsertionInputs()
        {
            var emptyToThree = new MyersDiff(new int[0], new[] { 1, 2, 3 });
            var insertion = new MyersDiff(new[] { 1, 2 }, new[] { 1, 7, 2 });

            Assert.Multiple(() =>
            {
                CollectionAssert.IsEmpty(emptyToThree.GetLCS());
                Assert.That(emptyToThree.GetSESLength(), Is.EqualTo(3));
                Assert.That(emptyToThree.GetReverseSESLength(), Is.EqualTo(3));
                Assert.That(emptyToThree.Execute().TotalEditLength, Is.EqualTo(3));

                Assert.That(insertion.GetSESLength(), Is.EqualTo(1));
                Assert.That(insertion.GetReverseSESLength(), Is.EqualTo(1));
                CollectionAssert.AreEqual(new[] { 1, 2 }, insertion.GetLCS());
            });
        }

        [Test]
        public void TextDiffHonorsWhitespaceCaseAndLeadingCharacterOptions()
        {
            var normalized = new TextDiff(HashType.Unique, true, true);
            EditScript normalizedScript = normalized.Execute(
                new[] { "  Alpha  ", "Beta" },
                new[] { "alpha", " beta " });

            var ignorePrefix = new TextDiff(HashType.CRC32, false, false, 5);
            EditScript prefixScript = ignorePrefix.Execute(
                new[] { "0000 Alpha", "0001 Beta" },
                new[] { "9999 Alpha", "8888 Beta" });

            Assert.Multiple(() =>
            {
                Assert.That(normalizedScript.Count, Is.Zero);
                Assert.That(normalizedScript.TotalEditLength, Is.Zero);
                Assert.That(prefixScript.Count, Is.Zero);
                Assert.That(prefixScript.TotalEditLength, Is.Zero);
            });
        }

        [Test]
        public void DiffOptionsBatchChangesRaiseOneNotificationAndMapEditColors()
        {
            Color originalInserted = DiffOptions.InsertedColor;
            Color originalDeleted = DiffOptions.DeletedColor;
            Color originalChanged = DiffOptions.ChangedColor;
            int originalSpacesPerTab = DiffOptions.SpacesPerTab;

            Color inserted = Color.FromArgb(originalInserted.ToArgb() ^ 0x00FFFFFF);
            Color deleted = Color.FromArgb(originalDeleted.ToArgb() ^ 0x00FFFFFF);
            Color changed = Color.FromArgb(originalChanged.ToArgb() ^ 0x00FFFFFF);
            int spacesPerTab = originalSpacesPerTab == 7 ? 8 : 7;
            int notifications = 0;
            EventHandler handler = delegate { notifications++; };

            DiffOptions.OptionsChanged += handler;
            try
            {
                DiffOptions.BeginUpdate();
                try
                {
                    DiffOptions.InsertedColor = inserted;
                    DiffOptions.DeletedColor = deleted;
                    DiffOptions.ChangedColor = changed;
                    DiffOptions.SpacesPerTab = spacesPerTab;
                }
                finally
                {
                    DiffOptions.EndUpdate();
                }

                Assert.Multiple(() =>
                {
                    Assert.That(notifications, Is.EqualTo(1));
                    Assert.That(DiffOptions.InsertedColor, Is.EqualTo(inserted));
                    Assert.That(DiffOptions.DeletedColor, Is.EqualTo(deleted));
                    Assert.That(DiffOptions.ChangedColor, Is.EqualTo(changed));
                    Assert.That(DiffOptions.SpacesPerTab, Is.EqualTo(spacesPerTab));
                    Assert.That(DiffOptions.GetColorForEditType(EditType.Insert), Is.EqualTo(inserted));
                    Assert.That(DiffOptions.GetColorForEditType(EditType.Delete), Is.EqualTo(deleted));
                    Assert.That(DiffOptions.GetColorForEditType(EditType.Change), Is.EqualTo(changed));
                    Assert.That(DiffOptions.GetColorForEditType((EditType)int.MaxValue), Is.EqualTo(Color.Transparent));
                });

                DiffOptions.InsertedColor = inserted;
                DiffOptions.DeletedColor = deleted;
                DiffOptions.ChangedColor = changed;
                DiffOptions.SpacesPerTab = spacesPerTab;

                Assert.That(notifications, Is.EqualTo(1),
                    "Reassigning identical values must not raise OptionsChanged.");
            }
            finally
            {
                DiffOptions.OptionsChanged -= handler;

                DiffOptions.BeginUpdate();
                try
                {
                    DiffOptions.InsertedColor = originalInserted;
                    DiffOptions.DeletedColor = originalDeleted;
                    DiffOptions.ChangedColor = originalChanged;
                    DiffOptions.SpacesPerTab = originalSpacesPerTab;
                }
                finally
                {
                    DiffOptions.EndUpdate();
                }
            }
        }

        [Test]
        public void DiffOptionsExposeStableDefaultColors()
        {
            Assert.Multiple(() =>
            {
                Assert.That(DiffOptions.DefaultInsertedColor, Is.EqualTo(Color.PaleTurquoise));
                Assert.That(DiffOptions.DefaultDeletedColor, Is.EqualTo(Color.Pink));
                Assert.That(DiffOptions.DefaultChangedColor, Is.EqualTo(Color.PaleGreen));
            });
        }

        [Test]
        public void BinaryDiffPatchReconstructsModifiedVersion()
        {
            byte[] baseBytes = Encoding.ASCII.GetBytes(
                "0123456789abcdefghijklmnopqrstuvwxyz");
            byte[] versionBytes = Encoding.ASCII.GetBytes(
                "01234HELLO56789abcXYZdefghijklmnopqrstuvwxyz!");

            var diff = new BinaryDiff
            {
                FootprintLength = 4,
                TableSize = 101,
                FavorLastMatch = true
            };

            AddCopyList patch;
            using (var baseStream = new MemoryStream(baseBytes, false))
            using (var versionStream = new MemoryStream(versionBytes, false))
            {
                patch = diff.Execute(baseStream, versionStream);
            }

            CollectionAssert.AreEqual(versionBytes, ApplyPatch(baseBytes, patch));
            Assert.That(patch.TotalByteLength, Is.EqualTo(versionBytes.Length));
            Assert.That(patch.Count, Is.GreaterThan(0));
        }

        [Test]
        public void BinaryDiffValidatesStreamsAndTuningRanges()
        {
            var diff = new BinaryDiff();

            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => diff.FootprintLength = 0);
                Assert.Throws<ArgumentOutOfRangeException>(() => diff.FootprintLength = 32);
                Assert.Throws<ArgumentOutOfRangeException>(() => diff.TableSize = 0);
            });

            diff.FootprintLength = 1;
            diff.FootprintLength = 31;
            diff.TableSize = 1;
            diff.FavorLastMatch = true;

            Assert.Multiple(() =>
            {
                Assert.That(diff.FootprintLength, Is.EqualTo(31));
                Assert.That(diff.TableSize, Is.EqualTo(1));
                Assert.That(diff.FavorLastMatch, Is.True);
            });

            using (var nonSeekable = new NonSeekableMemoryStream(new byte[16]))
            using (var seekable = new MemoryStream(new byte[16], false))
            {
                Assert.Throws<ArgumentException>(() => diff.Execute(nonSeekable, seekable));
                Assert.Throws<ArgumentException>(() => diff.Execute(seekable, nonSeekable));
            }
        }

        [Test]
        public void GdiffSerializesSmallAdditionAndCopyWithExpectedWireFormat()
        {
            var patch = new AddCopyList
            {
                new Addition { arBytes = new byte[] { 0x41, 0x42 } },
                new Copy { iBaseOffset = 1, iLength = 2 }
            };

            byte[] actual;
            using (var stream = new MemoryStream())
            {
                patch.GDIFF(stream);
                actual = stream.ToArray();
            }

            CollectionAssert.AreEqual(
                new byte[]
                {
                    0xD1, 0xFF, 0xD1, 0xFF, 0x04,
                    0x02, 0x41, 0x42,
                    0xF9, 0x00, 0x01, 0x02,
                    0x00
                },
                actual);
            Assert.That(patch.TotalByteLength, Is.EqualTo(4));
        }

        [Test]
        public void GdiffUsesSizeAppropriateCommandsAtEncodingBoundaries()
        {
            Assert.Multiple(() =>
            {
                Assert.That(FirstGdiffCommand(
                    new Addition { arBytes = new byte[246] }), Is.EqualTo(246));
                Assert.That(FirstGdiffCommand(
                    new Addition { arBytes = new byte[247] }), Is.EqualTo(247));
                Assert.That(FirstGdiffCommand(
                    new Addition { arBytes = new byte[65536] }), Is.EqualTo(248));

                Assert.That(FirstGdiffCommand(
                    new Copy { iBaseOffset = 1, iLength = 255 }), Is.EqualTo(249));
                Assert.That(FirstGdiffCommand(
                    new Copy { iBaseOffset = 1, iLength = 256 }), Is.EqualTo(250));
                Assert.That(FirstGdiffCommand(
                    new Copy { iBaseOffset = 1, iLength = 65536 }), Is.EqualTo(251));
                Assert.That(FirstGdiffCommand(
                    new Copy { iBaseOffset = 65536, iLength = 255 }), Is.EqualTo(252));
                Assert.That(FirstGdiffCommand(
                    new Copy { iBaseOffset = 65536, iLength = 256 }), Is.EqualTo(253));
                Assert.That(FirstGdiffCommand(
                    new Copy { iBaseOffset = 65536, iLength = 65536 }), Is.EqualTo(254));
            });
        }

        [Test]
        public void BinaryDiffLinesTrackBaseAndVersionOffsets()
        {
            byte[] baseBytes = Encoding.ASCII.GetBytes("ABCDEFGH");
            var patch = new AddCopyList
            {
                new Copy { iBaseOffset = 0, iLength = 4 },
                new Addition { arBytes = Encoding.ASCII.GetBytes("xy") },
                new Copy { iBaseOffset = 6, iLength = 2 }
            };

            BinaryDiffLines lines;
            using (var baseStream = new MemoryStream(baseBytes, false))
            {
                lines = new BinaryDiffLines(baseStream, patch, 4);
            }

            Assert.Multiple(() =>
            {
                Assert.That(lines.LeadingCharactersToIgnore, Is.EqualTo(12));
                Assert.That(lines.BaseLines.Count, Is.EqualTo(3));
                Assert.That(lines.VerLines.Count, Is.EqualTo(3));

                Assert.That(lines.BaseLines[0], Does.StartWith("00000000"));
                Assert.That(lines.BaseLines[1], Does.StartWith("00000004"));
                Assert.That(lines.BaseLines[2], Does.StartWith("00000006"));

                Assert.That(lines.VerLines[0], Does.StartWith("00000000"));
                Assert.That(lines.VerLines[1], Does.StartWith("00000004"));
                Assert.That(lines.VerLines[2], Does.StartWith("00000006"));

                Assert.That(lines.BaseLines[0], Does.Contain("41 42 43 44"));
                Assert.That(lines.VerLines[1], Does.Contain("78 79"));
            });
        }

        static int FirstGdiffCommand(object entry)
        {
            var patch = new AddCopyList { entry };

            using (var stream = new MemoryStream())
            {
                patch.GDIFF(stream);
                return stream.ToArray()[5];
            }
        }

        static byte[] ApplyPatch(byte[] baseBytes, AddCopyList patch)
        {
            using (var output = new MemoryStream())
            {
                foreach (object entry in patch)
                {
                    Addition addition = entry as Addition;
                    if (addition != null)
                    {
                        output.Write(addition.arBytes, 0, addition.arBytes.Length);
                        continue;
                    }

                    Copy copy = (Copy)entry;
                    output.Write(baseBytes, copy.iBaseOffset, copy.iLength);
                }

                return output.ToArray();
            }
        }

        sealed class NonSeekableMemoryStream : MemoryStream
        {
            public NonSeekableMemoryStream(byte[] buffer)
                : base(buffer, false)
            {
            }

            public override bool CanSeek
            {
                get { return false; }
            }
        }
    }
}
