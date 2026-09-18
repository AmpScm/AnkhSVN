using System;
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using SharpSvn;

namespace Ankh.Tests
{
    [TestFixture]
    public class SolutionSettingsPropertyTests
    {
        sealed class Harness
        {
            readonly Type _settingsType;
            readonly Type _cacheType;
            readonly object _settings;
            readonly object _cache;
            readonly MethodInfo _loadProperty;

            public Harness()
            {
                _settingsType = Assembly.Load("Ankh").GetType("Ankh.Settings.SolutionSettings", true);
                _cacheType = _settingsType.GetNestedType("SettingsCache", BindingFlags.NonPublic);
                _settings = FormatterServices.GetUninitializedObject(_settingsType);
                _cache = Activator.CreateInstance(_cacheType, true);
                _loadProperty = _settingsType.GetMethod(
                    "LoadPropertyBoth",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            public void Load(string key, string value)
            {
                _loadProperty.Invoke(
                    _settings,
                    new object[] { _cache, new SvnPropertyValue(key, value) });
            }

            public T Get<T>(string fieldName)
            {
                return (T)_cacheType
                    .GetField(fieldName, BindingFlags.Instance | BindingFlags.Public)
                    .GetValue(_cache);
            }
        }

        [Test]
        public void LoadsSupportedTortoiseAndBugTrackProperties()
        {
            var h = new Harness();

            h.Load(SvnPropertyNames.BugTrackAppend, "true");
            h.Load(SvnPropertyNames.BugTrackLabel, "Issue:");
            h.Load(SvnPropertyNames.BugTrackLogRegex, "BUG-(\\d+)");
            h.Load(SvnPropertyNames.BugTrackMessage, "Fix %BUGID%\r\nDetails");
            h.Load(SvnPropertyNames.BugTrackNumber, "no");
            h.Load(SvnPropertyNames.BugTrackUrl, "https://tracker.example/%BUGID%");
            h.Load(SvnPropertyNames.BugTrackWarnIfNoIssue, "yes");
            h.Load(SvnPropertyNames.TortoiseSvnLogMinSize, "12");
            h.Load(SvnPropertyNames.TortoiseSvnLockMsgMinSize, "7");
            h.Load(SvnPropertyNames.TortoiseSvnLogWidthLine, "80");
            h.Load(SvnPropertyNames.TortoiseSvnLogSummary, "summary-regex");
            h.Load(SvnPropertyNames.TortoiseSvnLogRevRegex, "r(\\d+)");
            h.Load(SvnPropertyNames.WebViewerRevision, "https://viewer.example/rev/%REVISION%");
            h.Load(SvnPropertyNames.WebViewerPathRevision, "https://viewer.example/path");

            Assert.That(h.Get<bool?>("BugTrackAppend"), Is.True);
            Assert.That(h.Get<string>("BugTrackLabel"), Is.EqualTo("Issue:"));
            Assert.That(h.Get<string>("BugTrackLogRegexes"), Is.EqualTo("BUG-(\\d+)"));
            Assert.That(h.Get<string>("BugTrackMessage"), Is.EqualTo("Fix %BUGID%\nDetails"));
            Assert.That(h.Get<bool?>("BugTrackNumber"), Is.False);
            Assert.That(h.Get<string>("BugTrackUrl"), Is.EqualTo("https://tracker.example/%BUGID%"));
            Assert.That(h.Get<bool?>("BugTrackWarnIfNoIssue"), Is.True);
            Assert.That(h.Get<int?>("LogMessageMinSize"), Is.EqualTo(12));
            Assert.That(h.Get<int?>("LockMessageMinSize"), Is.EqualTo(7));
            Assert.That(h.Get<int?>("LogWidth"), Is.EqualTo(80));
            Assert.That(h.Get<string>("LogSummary"), Is.EqualTo("summary-regex"));
            Assert.That(h.Get<string>("RevisionRegex"), Is.EqualTo("r(\\d+)"));
            Assert.That(h.Get<string>("RevisionUrl"), Is.EqualTo("https://viewer.example/rev/%REVISION%"));
            Assert.That(h.Get<string>("RevisionPathUrl"), Is.EqualTo("https://viewer.example/path"));
        }

        [Test]
        public void FirstInheritedStandardPropertyWins()
        {
            var h = new Harness();

            h.Load(SvnPropertyNames.BugTrackLabel, "nearest");
            h.Load(SvnPropertyNames.BugTrackLabel, "parent");

            h.Load(SvnPropertyNames.TortoiseSvnLogMinSize, "10");
            h.Load(SvnPropertyNames.TortoiseSvnLogMinSize, "99");

            h.Load(SvnPropertyNames.BugTrackAppend, "yes");
            h.Load(SvnPropertyNames.BugTrackAppend, "no");

            Assert.That(h.Get<string>("BugTrackLabel"), Is.EqualTo("nearest"));
            Assert.That(h.Get<int?>("LogMessageMinSize"), Is.EqualTo(10));
            Assert.That(h.Get<bool?>("BugTrackAppend"), Is.True);
        }

        [Test]
        public void InvalidBooleanAndNumericPropertiesAreIgnored()
        {
            var h = new Harness();

            h.Load(SvnPropertyNames.BugTrackAppend, "");
            h.Load(SvnPropertyNames.BugTrackNumber, "maybe");
            h.Load(SvnPropertyNames.TortoiseSvnLogMinSize, "");
            h.Load(SvnPropertyNames.TortoiseSvnLockMsgMinSize, "not-a-number");
            h.Load(SvnPropertyNames.TortoiseSvnLogWidthLine, "wide");

            Assert.That(h.Get<bool?>("BugTrackAppend"), Is.Null);
            Assert.That(h.Get<bool?>("BugTrackNumber"), Is.Null);
            Assert.That(h.Get<int?>("LogMessageMinSize"), Is.Null);
            Assert.That(h.Get<int?>("LockMessageMinSize"), Is.Null);
            Assert.That(h.Get<int?>("LogWidth"), Is.Null);
        }

        [Test]
        public void LoadsAnkhIssueRepositoryProperties()
        {
            var h = new Harness();

            h.Load(AnkhSccPropertyNames.IssueRepositoryConnector, "connector");
            h.Load(AnkhSccPropertyNames.IssueRepositoryUri, "https://issues.example/");
            h.Load(AnkhSccPropertyNames.IssueRepositoryId, "project");
            h.Load(AnkhSccPropertyNames.IssueRepositoryPropertyNames, "one,two");
            h.Load(AnkhSccPropertyNames.IssueRepositoryPropertyValues, "1,2");

            Assert.That(h.Get<string>("IssueRepositoryConnectorName"), Is.EqualTo("connector"));
            Assert.That(h.Get<string>("IssueRepositoryUri"), Is.EqualTo("https://issues.example/"));
            Assert.That(h.Get<string>("IssueRepositoryId"), Is.EqualTo("project"));
            Assert.That(h.Get<string>("IssueRepositoryPropertyNames"), Is.EqualTo("one,two"));
            Assert.That(h.Get<string>("IssueRepositoryPropertyValues"), Is.EqualTo("1,2"));
        }

        [Test]
        public void IssueRepositoryPropertiesUseLatestInheritedValue()
        {
            var h = new Harness();

            h.Load(AnkhSccPropertyNames.IssueRepositoryConnector, "first");
            h.Load(AnkhSccPropertyNames.IssueRepositoryConnector, "second");

            Assert.That(h.Get<string>("IssueRepositoryConnectorName"), Is.EqualTo("second"));
        }
    }
}
