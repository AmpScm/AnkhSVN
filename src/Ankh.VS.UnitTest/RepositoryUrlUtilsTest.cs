using System;
using System.Reflection;
using Ankh.UI.SccManagement;
using NUnit.Framework;

namespace AnkhSvn_UnitTestProject
{
    [TestFixture]
    public class RepositoryUrlUtilsTest
    {
        [TestCase("http://server.tld/svn/trunk/something", "http://server.tld/svn/branches/", TestName = "TryGuessLayout_HttpTrunk_ReturnsBranches")]
        [TestCase("file:///c:/repos/trunk/something", "file:///C:/repos/branches/", TestName = "TryGuessLayout_LocalFileTrunk_ReturnsBranches")]
        [TestCase("file://server/share/repos/trunk/something", "file://server/share/repos/branches/", TestName = "TryGuessLayout_UNCFileTrunk_ReturnsBranches")]
        public void GuessNormalizedLayoutReturnsBranches(string repositoryUri, string expectedBranchesRoot)
        {
            RepositoryLayoutInfo info = GuessNormalizedLayout(new Uri(repositoryUri));

            Assert.That(info, Is.Not.Null);
            Assert.That(info.BranchesRoot, Is.EqualTo(new Uri(expectedBranchesRoot)));
        }

        static RepositoryLayoutInfo GuessNormalizedLayout(Uri uri)
        {
            MethodInfo parser = typeof(RepositoryUrlUtils).GetMethod(
                "TryGuessLayoutNormalized",
                BindingFlags.Static | BindingFlags.NonPublic);

            Assert.That(parser, Is.Not.Null, "Expected the managed repository layout parser");

            object[] arguments = { uri, null };
            bool success = (bool)parser.Invoke(null, arguments);

            Assert.That(success, Is.True, "Expected repository layout parsing to succeed");
            return arguments[1] as RepositoryLayoutInfo;
        }
    }
}
