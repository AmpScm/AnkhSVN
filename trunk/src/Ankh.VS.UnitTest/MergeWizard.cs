using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using SharpSvn;
using Ankh.UI.MergeWizard;
using NUnit.Framework.SyntaxHelpers;

namespace Ankh.Tests
{
    [TestFixture]
    public class MergeWizardTest
    {
        [Test]
        public void TestMergeRangeToString()
        {
            List<SvnRevisionRange> revisions = new List<SvnRevisionRange>();

            Assert.That(
                MergeWizard.MergeRevisionsAsString(revisions),
                Is.EqualTo(""));

            revisions.Add(new SvnRevisionRange(2, 3));

            Assert.That(
                MergeWizard.MergeRevisionsAsString(revisions),
                Is.EqualTo("3"));

            revisions.Add(new SvnRevisionRange(6, 7));
            
            Assert.That(
                MergeWizard.MergeRevisionsAsString(revisions),
                Is.EqualTo("3, 7"));

            revisions.Add(new SvnRevisionRange(10, 15));

            Assert.That(
                MergeWizard.MergeRevisionsAsString(revisions),
                Is.EqualTo("3, 7, 11-15"));
        }
    }
}
