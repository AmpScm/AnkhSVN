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

using System.IO;

using Ankh.Scc;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class SvnConflictArtifactLogicTests
    {
        [TestCase("source.cs.mine", "source.cs")]
        [TestCase("source.cs.r0", "source.cs")]
        [TestCase("source.cs.r123", "source.cs")]
        [TestCase("source.cs.R456", "source.cs")]
        public void TryGetConflictedPath_RecognizesSvnTextConflictNames(
            string artifactName,
            string expectedName)
        {
            string directory = Path.Combine("working-copy", "project");
            string artifact = Path.Combine(directory, artifactName);

            string conflictedPath;
            bool result = SvnConflictArtifactLogic.TryGetConflictedPath(
                artifact,
                out conflictedPath);

            Assert.That(result, Is.True);
            Assert.That(
                conflictedPath,
                Is.EqualTo(Path.Combine(directory, expectedName)));
        }

        [TestCase("source.cs")]
        [TestCase("source.cs.r")]
        [TestCase("source.cs.r12x")]
        [TestCase(".mine")]
        [TestCase(".r123")]
        public void TryGetConflictedPath_RejectsNonArtifactNames(string fileName)
        {
            string conflictedPath;

            Assert.That(
                SvnConflictArtifactLogic.TryGetConflictedPath(
                    Path.Combine("working-copy", fileName),
                    out conflictedPath),
                Is.False);
            Assert.That(conflictedPath, Is.Null);
        }

        [Test]
        public void MatchesConflictMetadata_MatchesMineFile()
        {
            string directory = Path.Combine("working-copy", "project");
            string conflicted = Path.Combine(directory, "source.cs");
            string artifact = Path.Combine(directory, "source.cs.mine");

            Assert.That(
                SvnConflictArtifactLogic.MatchesConflictMetadata(
                    artifact,
                    conflicted,
                    null,
                    null,
                    "source.cs.mine"),
                Is.True);
        }

        [Test]
        public void MatchesConflictMetadata_MatchesRevisionFile()
        {
            string directory = Path.Combine("working-copy", "project");
            string conflicted = Path.Combine(directory, "source.cs");
            string artifact = Path.Combine(directory, "source.cs.r123");

            Assert.That(
                SvnConflictArtifactLogic.MatchesConflictMetadata(
                    artifact,
                    conflicted,
                    "source.cs.r123",
                    "source.cs.r124",
                    "source.cs.mine"),
                Is.True);
        }

        [Test]
        public void MatchesConflictMetadata_DoesNotSuppressLegitimateSimilarName()
        {
            string directory = Path.Combine("working-copy", "project");
            string conflicted = Path.Combine(directory, "report");
            string artifact = Path.Combine(directory, "report.mine");

            Assert.That(
                SvnConflictArtifactLogic.MatchesConflictMetadata(
                    artifact,
                    conflicted,
                    "report.r10",
                    "report.r11",
                    null),
                Is.False);
        }

        [Test]
        public void MatchesConflictMetadata_AcceptsAbsoluteConflictPath()
        {
            string directory = Path.Combine(
                Path.GetTempPath(),
                "ankh-conflict-artifact-test");
            string conflicted = Path.Combine(directory, "source.cs");
            string artifact = Path.Combine(directory, "source.cs.r42");

            Assert.That(
                SvnConflictArtifactLogic.MatchesConflictMetadata(
                    artifact,
                    conflicted,
                    artifact,
                    null,
                    null),
                Is.True);
        }
    }
}
