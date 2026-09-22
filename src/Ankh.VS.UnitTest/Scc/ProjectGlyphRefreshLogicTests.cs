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

using Ankh.Scc.ProjectMap;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class ProjectGlyphRefreshLogicTests
    {
        [TestCase(true, false, true, true, true)]
        [TestCase(false, false, true, true, false)]
        [TestCase(true, true, true, true, false)]
        [TestCase(true, false, false, true, false)]
        [TestCase(true, false, true, false, false)]
        public void ShouldNotifyNewItem_RequiresPaintableProjectNode(
            bool loaded,
            bool nonMember,
            bool validPath,
            bool exists,
            bool expected)
        {
            Assert.That(
                ProjectGlyphRefreshLogic.ShouldNotifyNewItem(
                    loaded,
                    nonMember,
                    validPath,
                    exists),
                Is.EqualTo(expected));
        }
    }
}
