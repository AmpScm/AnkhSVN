// Copyright 2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Reflection;
using Ankh.UI.SccManagement;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class RepositoryUrlUtilsFixture
    {
        [TestCase(
            "http://svn.test.org/repos/project/trunk/project.sln",
            "http://svn.test.org/repos/project/",
            "http://svn.test.org/repos/project/trunk/",
            "http://svn.test.org/repos/project/branches/",
            "trunk/",
            "trunk",
            TestName = "TestGuessLayoutSimpleTrunk")]
        [TestCase(
            "http://svn.test.org/repos/project/trunk/s/r/c/project.sln",
            "http://svn.test.org/repos/project/",
            "http://svn.test.org/repos/project/trunk/",
            "http://svn.test.org/repos/project/branches/",
            "trunk/",
            "trunk",
            TestName = "TestGuessLayoutComplexTrunk")]
        [TestCase(
            "http://svn.test.org/repos/project/branches/experimental/project.sln",
            "http://svn.test.org/repos/project/",
            "http://svn.test.org/repos/project/branches/experimental/",
            "http://svn.test.org/repos/project/branches/",
            "experimental/",
            "experimental",
            TestName = "TestGuessLayoutSimpleBranch")]
        [TestCase(
            "http://svn.test.org/repos/project/branches/experimental/s/r/c/project.sln",
            "http://svn.test.org/repos/project/",
            "http://svn.test.org/repos/project/branches/experimental/",
            "http://svn.test.org/repos/project/branches/",
            "experimental/",
            "experimental",
            TestName = "TestGuessLayoutComplexBranch")]
        [TestCase(
            "http://svn.test.org/repos/myproj/sandbox/src/project.sln",
            "http://svn.test.org/repos/myproj/sandbox/",
            "http://svn.test.org/repos/myproj/sandbox/src/",
            "http://svn.test.org/repos/myproj/sandbox/branches/",
            "src/",
            "src",
            TestName = "TestGuessLayoutFromNonStandardBranch")]
        [TestCase(
            "http://svn.test.org/repos/project.sln",
            "http://svn.test.org/",
            "http://svn.test.org/repos/",
            "http://svn.test.org/branches/",
            "repos/",
            "repos",
            TestName = "TestGuessLayoutFromReposRoot")]
        public void GuessLayoutFromNormalizedUri(
            string repositoryUri,
            string wholeProjectRoot,
            string workingRoot,
            string branchesRoot,
            string selectedBranch,
            string selectedBranchName)
        {
            RepositoryLayoutInfo info = GuessNormalizedLayout(new Uri(repositoryUri));

            Assert.IsNotNull(info, "expected a layout info");
            Assert.AreEqual(new Uri(wholeProjectRoot), info.WholeProjectRoot, "wrong project root");
            Assert.AreEqual(new Uri(workingRoot), info.WorkingRoot, "wrong working root");
            Assert.AreEqual(new Uri(branchesRoot), info.BranchesRoot, "wrong branch root");
            Assert.AreEqual(new Uri(selectedBranch, UriKind.Relative), info.SelectedBranch, "wrong selected branch");
            Assert.AreEqual(selectedBranchName, info.SelectedBranchName, "wrong branch name");
        }

        static RepositoryLayoutInfo GuessNormalizedLayout(Uri uri)
        {
            MethodInfo parser = typeof(RepositoryUrlUtils).GetMethod(
                "TryGuessLayoutNormalized",
                BindingFlags.Static | BindingFlags.NonPublic);

            Assert.NotNull(parser, "Expected the managed repository layout parser");

            object[] arguments = { uri, null };
            bool success = (bool)parser.Invoke(null, arguments);

            Assert.IsTrue(success, "Expected repository layout parsing to succeed");
            return arguments[1] as RepositoryLayoutInfo;
        }
    }
}
