using System;
using System.Reflection;
using Ankh.Scc;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.Scc
{
    [TestFixture]
    public class SvnStatusDataBehaviorTests
    {
        [TestCase(SvnStatus.Normal, SvnStatus.Conflicted, SvnStatus.Normal, SvnStatus.Conflicted)]
        [TestCase(SvnStatus.Normal, SvnStatus.Normal, SvnStatus.Conflicted, SvnStatus.Conflicted)]
        [TestCase(SvnStatus.Missing, SvnStatus.Conflicted, SvnStatus.Conflicted, SvnStatus.Missing)]
        public void CombinedStatusHonorsConflictAndNodePriority(
            SvnStatus nodeStatus,
            SvnStatus textStatus,
            SvnStatus propertyStatus,
            SvnStatus expected)
        {
            SvnStatusData status = CreateStatus(nodeStatus, textStatus, propertyStatus);

            Assert.That(status.CombinedStatus, Is.EqualTo(expected));
        }

        static SvnStatusData CreateStatus(
            SvnStatus nodeStatus,
            SvnStatus textStatus,
            SvnStatus propertyStatus)
        {
            ConstructorInfo constructor = typeof(SvnStatusData).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new[] { typeof(SvnStatus) },
                null);

            Assert.That(constructor, Is.Not.Null);

            var status = (SvnStatusData)constructor.Invoke(new object[] { nodeStatus });

            SetField(status, "_localTextStatus", textStatus);
            SetField(status, "_localPropertyStatus", propertyStatus);

            return status;
        }

        static void SetField(SvnStatusData status, string name, SvnStatus value)
        {
            FieldInfo field = typeof(SvnStatusData).GetField(
                name,
                BindingFlags.Instance | BindingFlags.NonPublic);

            Assert.That(field, Is.Not.Null, name);
            field.SetValue(status, value);
        }
    }
}
