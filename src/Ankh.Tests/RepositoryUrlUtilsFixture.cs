// $Id$
//
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
using System.Collections.Generic;
using System.Text;
using Ankh.UI.SccManagement;
using NUnit.Framework;

namespace Ankh.Tests
{
    [TestFixture]
    public class RepositoryUrlUtilsFixture
    {
        [Test]
        public void TestGuessLayoutSimpleTrunk()
        {
            Uri u = new Uri("http://svn.test.org/repos/project/trunk/project.sln");

            RepositoryLayoutInfo i;
            RepositoryUrlUtils.TryGuessLayout(new MockContext(), u, out i);

            Assert.IsNotNull(i, "expected a layout info");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/"), i.WholeProjectRoot, "wrong project root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/trunk/"), i.WorkingRoot, "wrong working root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/branches/"), i.BranchesRoot, "wrong branch root");
            Assert.AreEqual(new Uri("trunk/", UriKind.Relative), i.SelectedBranch, "wrong selected branch");
            Assert.AreEqual("trunk", i.SelectedBranchName, "wrong branch name");
        }

        [Test]
        public void TestGuessLayoutComplexTrunk()
        {
            Uri u = new Uri("http://svn.test.org/repos/project/trunk/s/r/c/project.sln");

            RepositoryLayoutInfo i;
            RepositoryUrlUtils.TryGuessLayout(new MockContext(), u, out i);

            Assert.IsNotNull(i, "expected a layout info");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/"), i.WholeProjectRoot, "wrong project root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/trunk/"), i.WorkingRoot, "wrong working root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/branches/"), i.BranchesRoot, "wrong branch root");
            Assert.AreEqual(new Uri("trunk/", UriKind.Relative), i.SelectedBranch, "wrong selected branch");
            Assert.AreEqual("trunk", i.SelectedBranchName, "wrong branch name");
        }

        [Test]
        public void TestGuessLayoutSimpleBranch()
        {
            Uri u = new Uri("http://svn.test.org/repos/project/branches/experimental/project.sln");

            RepositoryLayoutInfo i;
            RepositoryUrlUtils.TryGuessLayout(new MockContext(), u, out i);

            Assert.IsNotNull(i, "expected a layout info");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/"), i.WholeProjectRoot, "wrong project root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/branches/experimental/"), i.WorkingRoot, "wrong working root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/branches/"), i.BranchesRoot, "wrong branch root");
            Assert.AreEqual(new Uri("experimental/", UriKind.Relative), i.SelectedBranch, "wrong selected branch");
            Assert.AreEqual("experimental", i.SelectedBranchName, "wrong selected name");
        }

        [Test]
        public void TestGuessLayoutComplexBranch()
        {
            Uri u = new Uri("http://svn.test.org/repos/project/branches/experimental/s/r/c/project.sln");

            RepositoryLayoutInfo i;
            RepositoryUrlUtils.TryGuessLayout(new MockContext(), u, out i);

            Assert.IsNotNull(i, "expected a layout info");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/"), i.WholeProjectRoot, "wrong project root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/branches/experimental/"), i.WorkingRoot, "wrong working root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/project/branches/"), i.BranchesRoot, "wrong branch root");
            Assert.AreEqual(new Uri("experimental/", UriKind.Relative), i.SelectedBranch, "wrong selected branch");
            Assert.AreEqual("experimental", i.SelectedBranchName, "wrong selected name");
        }

        [Test]
        public void TestGuessLayoutFromNonStandardBranch()
        {
            Uri u = new Uri("http://svn.test.org/repos/myproj/sandbox/src/project.sln");

            RepositoryLayoutInfo i;
            RepositoryUrlUtils.TryGuessLayout(new MockContext(), u, out i);

            Assert.IsNotNull(i, "expected a layout info");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/myproj/sandbox/"), i.WholeProjectRoot, "wrong project root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/myproj/sandbox/src/"), i.WorkingRoot, "wrong working root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/myproj/sandbox/branches/"), i.BranchesRoot, "wrong branch root");
            Assert.AreEqual(new Uri("src/", UriKind.Relative), i.SelectedBranch, "wrong selected branch");
            Assert.AreEqual("src", i.SelectedBranchName, "wrong selected name");
        }

        [Test]
        public void TestGuessLayoutFromReposRoot()
        {
            Uri u = new Uri("http://svn.test.org/repos/project.sln");

            RepositoryLayoutInfo i;
            RepositoryUrlUtils.TryGuessLayout(new MockContext(), u, out i);

            Assert.IsNotNull(i, "expected a layout info");
            Assert.AreEqual(new Uri("http://svn.test.org/"), i.WholeProjectRoot, "wrong project root");
            Assert.AreEqual(new Uri("http://svn.test.org/repos/"), i.WorkingRoot, "wrong working root");
            Assert.AreEqual(new Uri("http://svn.test.org/branches/"), i.BranchesRoot, "wrong branch root");
            Assert.AreEqual(new Uri("repos/", UriKind.Relative), i.SelectedBranch, "wrong selected branch");
            Assert.AreEqual("repos", i.SelectedBranchName, "wrong selected name");
        }

        class MockContext : IAnkhServiceProvider
        {
            #region IAnkhServiceProvider Members

            public T GetService<T>() where T : class
            {
                throw new NotImplementedException();
            }

            public T GetService<T>(Type serviceType) where T : class
            {
                throw new NotImplementedException();
            }

            #endregion

            #region IServiceProvider Members

            public object GetService(Type serviceType)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
