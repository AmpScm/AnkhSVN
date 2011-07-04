using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ankh.UI.SccManagement;
using AnkhSvn_UnitTestProject.Helpers;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace AnkhSvn_UnitTestProject
{
    [TestFixture]
    public class RepositoryUrlUtilsTest
    {
        [Test]
        public void TryGuessLayout_HttpTrunk_ReturnsBranches()
        {
            RepositoryLayoutInfo info;
            bool rslt = RepositoryUrlUtils.TryGuessLayout(new AnkhServiceProvider(), new Uri("http://server.tld/svn/trunk/something"), out info);

            Assert.That(rslt);
            Assert.That(info.BranchesRoot, Is.EqualTo(new Uri("http://server.tld/svn/branches/")));
        }

        [Test]
        public void TryGuessLayout_LocalFileTrunk_ReturnsBranches()
        {
            RepositoryLayoutInfo info;
            bool rslt = RepositoryUrlUtils.TryGuessLayout(new AnkhServiceProvider(), new Uri("file:///c:/repos/trunk/something"), out info);

            Assert.That(rslt);
            Assert.That(info.BranchesRoot, Is.EqualTo(new Uri("file:///C:/repos/branches/")));
        }

        [Test]
        public void TryGuessLayout_UNCFileTrunk_ReturnsBranches()
        {
            RepositoryLayoutInfo info;
            bool rslt = RepositoryUrlUtils.TryGuessLayout(new AnkhServiceProvider(), new Uri("file://server/share/repos/trunk/something"), out info);

            Assert.That(rslt);
            Assert.That(info.BranchesRoot, Is.EqualTo(new Uri("file://server/share/repos/branches/")));
        }
    }
}
