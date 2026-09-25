using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ankh.Configuration;
using Ankh.UI.PendingChanges;
using NUnit.Framework;
using SharpSvn;

namespace AnkhSvn_UnitTestProject.Dialogs
{
    [TestFixture]
    public class CommittedHistoryTests
    {
        [Test, Apartment(ApartmentState.STA)]
        public void HistoryViewShowsCommitColumnsAndDoesNotOverlapSettingsOrDetails()
        {
            using (var page = new RecentChangesPage())
            {
                page.PerformLayout();
                var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                var list = (ListView)typeof(RecentChangesPage).GetField("syncView", flags).GetValue(page);
                var details = (TextBox)typeof(RecentChangesPage).GetField("_details", flags).GetValue(page);
                var limit = (NumericUpDown)typeof(RecentChangesPage).GetField("_historyLimit", flags).GetValue(page);
                Assert.That(limit.Value, Is.EqualTo(25));
                Assert.That(list.Columns.Cast<ColumnHeader>().Select(c => c.Text),
                    Is.EqualTo(new[] { "Revision", "Author", "Date", "Message", "Repository" }));
                Assert.That(list.Bottom, Is.LessThanOrEqualTo(details.Top));
                Assert.That(list.Parent.Parent.Top, Is.GreaterThanOrEqualTo(limit.Parent.Bottom));
            }
        }

        [Test]
        public void DefaultAndConfiguredLimitSurviveSettingsRoundTrip()
        {
            var serializer = new XmlSerializer(typeof(AnkhConfig));
            using (var reader = new StringReader("<AnkhConfig />"))
                Assert.That(((AnkhConfig)serializer.Deserialize(reader)).RecentChangesHistoryLimit, Is.EqualTo(25));
            var config = new AnkhConfig { RecentChangesHistoryLimit = 60 };
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, config);
                using (var reader = new StringReader(writer.ToString()))
                    Assert.That(((AnkhConfig)serializer.Deserialize(reader)).RecentChangesHistoryLimit, Is.EqualTo(60));
            }
        }

        [TestCase(0, 1)]
        [TestCase(-1, 1)]
        [TestCase(1001, 1000)]
        public void LimitCannotRequestAnUnboundedLog(int value, int expected)
        {
            Assert.That(new AnkhConfig { RecentChangesHistoryLimit = value }.RecentChangesHistoryLimit, Is.EqualTo(expected));
        }

        [Test]
        public void OverlappingRootsDeduplicateButDifferentRepositoriesKeepSameRevision()
        {
            var time = DateTime.UtcNow;
            var entries = new[] {
                new CommittedHistoryEntry { Repository = "a", Revision = 5, Time = time.AddDays(-1) },
                new CommittedHistoryEntry { Repository = "a", Revision = 6, Time = time },
                new CommittedHistoryEntry { Repository = "a", Revision = 6, Time = time },
                new CommittedHistoryEntry { Repository = "b", Revision = 6, Time = time.AddHours(1) }
            };
            var result = CommittedHistory.Select(entries, 2);
            Assert.That(result.Select(e => e.Repository), Is.EqualTo(new[] { "b", "a" }));
            Assert.That(result.Select(e => e.Revision), Is.EqualTo(new long[] { 6, 6 }));
        }

        [Test]
        public void HistoryComesFromHeadSurvivesNewClientAndExcludesLocalEdits()
        {
            string directory = Path.Combine(Path.GetTempPath(), "AnkhHistory-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);
            try
            {
                string repository = Path.Combine(directory, "repository");
                string workingCopy = Path.Combine(directory, "working-copy");
                using (var admin = new SvnRepositoryClient())
                    admin.CreateRepository(repository);
                using (var client = new SvnClient())
                {
                    client.CheckOut(new Uri(repository + Path.DirectorySeparatorChar), workingCopy);
                    string file = Path.Combine(workingCopy, "file.txt");
                    File.WriteAllText(file, "first");
                    client.Add(file);
                    for (int i = 1; i <= 30; i++)
                    {
                        File.WriteAllText(file, "change " + i);
                        client.Commit(workingCopy, new SvnCommitArgs { LogMessage = "commit " + i });
                    }
                    client.Update(workingCopy, new SvnUpdateArgs { Revision = new SvnRevision(1) });
                    File.WriteAllText(file, "uncommitted local activity");
                    var history = CommittedHistory.Fetch(client, new[] { workingCopy, workingCopy }, 25);
                    Assert.That(history.Count, Is.EqualTo(25));
                    Assert.That(history.First().Revision, Is.EqualTo(30));
                    Assert.That(history.Last().Revision, Is.EqualTo(6));
                    Assert.That(history.First().Message, Is.EqualTo("commit 30"));
                    Assert.That(history.First().ChangedPaths, Does.Contain("/file.txt"));
                }
                using (var reopened = new SvnClient())
                {
                    var history = CommittedHistory.Fetch(reopened, new[] { workingCopy }, 3);
                    Assert.That(history.Select(e => e.Revision), Is.EqualTo(new long[] { 30, 29, 28 }));
                }
            }
            finally
            {
                foreach (string file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
                    File.SetAttributes(file, FileAttributes.Normal);
                Directory.Delete(directory, true);
            }
        }
    }
}
