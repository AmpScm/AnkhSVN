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
using System.Collections.Generic;
using System.IO;

using Ankh.Scc;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class ProjectTrackerOriginLogicTests
    {
        [Test]
        public void BuildNameToItem_IncludesNewAndUnresolvedOriginsOnly()
        {
            string root = Path.GetPathRoot(Environment.CurrentDirectory);
            string newName = Path.Combine(root, "dest", "alpha.cs");
            string unresolved = Path.Combine(root, "pending", "beta.cs");
            string resolved = Path.Combine(root, "done", "gamma.cs");

            SortedList<string, string> origins =
                new SortedList<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { unresolved, null },
                    { resolved, Path.Combine(root, "source", "gamma.cs") }
                };

            SortedList<string, string> result =
                ProjectTrackerOriginLogic.BuildNameToItem(
                    newName,
                    origins);

            Assert.That(result["alpha.cs"], Is.EqualTo(newName));
            Assert.That(result["beta.cs"], Is.EqualTo(unresolved));
            Assert.That(result.ContainsKey("gamma.cs"), Is.False);
        }

        [Test]
        public void BuildNameToItem_LaterUnresolvedNameReplacesEarlierMapping()
        {
            string root = Path.GetPathRoot(Environment.CurrentDirectory);
            string newName = Path.Combine(root, "dest", "same.cs");
            string pending = Path.Combine(root, "other", "same.cs");

            SortedList<string, string> origins =
                new SortedList<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { pending, null }
                };

            SortedList<string, string> result =
                ProjectTrackerOriginLogic.BuildNameToItem(
                    newName,
                    origins);

            Assert.That(result["same.cs"], Is.EqualTo(pending));
        }

        [Test]
        public void InferOrigin_UsesKnownChildCopyToInferParent()
        {
            string root = Path.GetPathRoot(Environment.CurrentDirectory);
            string newName = Path.Combine(root, "dest", "folder");
            string copiedChild = Path.Combine(newName, "child.cs");
            string originRoot = Path.Combine(root, "source", "folder");
            string originChild = Path.Combine(originRoot, "child.cs");

            SortedList<string, string> origins =
                new SortedList<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { copiedChild, originChild }
                };

            string result =
                ProjectTrackerOriginLogic.InferOrigin(
                    newName,
                    origins);

            Assert.That(result, Is.EqualTo(originRoot));
        }

        [Test]
        public void InferOrigin_IgnoresNullAndUnrelatedEntries()
        {
            string root = Path.GetPathRoot(Environment.CurrentDirectory);
            string newName = Path.Combine(root, "dest", "child.cs");

            SortedList<string, string> origins =
                new SortedList<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { Path.Combine(root, "pending", "child.cs"), null },
                    {
                        Path.Combine(root, "other"),
                        Path.Combine(root, "source", "other")
                    }
                };

            Assert.That(
                ProjectTrackerOriginLogic.InferOrigin(
                    newName,
                    origins),
                Is.Null);
        }

        [Test]
        public void InferOrigin_RejectsConflictingParentOrigins()
        {
            string root = Path.GetPathRoot(Environment.CurrentDirectory);
            string newName = Path.Combine(root, "dest", "folder", "child.cs");

            SortedList<string, string> origins =
                new SortedList<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    {
                        Path.Combine(root, "dest"),
                        Path.Combine(root, "source-a")
                    },
                    {
                        Path.Combine(root, "dest", "folder"),
                        Path.Combine(root, "source-b", "folder")
                    }
                };

            Assert.That(
                ProjectTrackerOriginLogic.InferOrigin(
                    newName,
                    origins),
                Is.Null);
        }
    }
}
