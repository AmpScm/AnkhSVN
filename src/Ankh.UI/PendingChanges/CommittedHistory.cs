using System;
using System.Collections.Generic;
using System.Linq;
using SharpSvn;

namespace Ankh.UI.PendingChanges
{
    internal sealed class CommittedHistoryEntry
    {
        public string Repository { get; set; }
        public long Revision { get; set; }
        public string Author { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }
        public string ChangedPaths { get; set; }
    }

    internal static class CommittedHistory
    {
        internal static List<CommittedHistoryEntry> Select(IEnumerable<CommittedHistoryEntry> entries, int limit)
        {
            return entries.GroupBy(e => new { e.Repository, e.Revision })
                .Select(g => g.First()).OrderByDescending(e => e.Time)
                .ThenByDescending(e => e.Revision).ThenBy(e => e.Repository, StringComparer.Ordinal)
                .Take(Math.Max(1, Math.Min(1000, limit))).ToList();
        }

        internal static List<CommittedHistoryEntry> Fetch(SvnClient client, IEnumerable<string> roots, int limit)
        {
            var entries = new List<CommittedHistoryEntry>();
            foreach (string path in roots.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                // Resolve the working-copy URL locally, then ask for HEAD history even
                // when the working copy has not yet been updated to the latest revision.
                SvnInfoEventArgs info;
                client.GetInfo(path, out info);
                if (info == null || info.Uri == null || info.RepositoryRoot == null)
                    continue;
                string repository = info.RepositoryRoot.AbsoluteUri;
                var args = new SvnLogArgs {
                    Start = SvnRevision.Head, End = new SvnRevision(0),
                    Limit = Math.Max(1, Math.Min(1000, limit)), RetrieveChangedPaths = true
                };
                client.Log(info.Uri, args, (sender, e) => entries.Add(new CommittedHistoryEntry {
                    Repository = repository, Revision = e.Revision, Author = e.Author ?? "",
                    Time = e.Time, Message = e.LogMessage ?? "",
                    ChangedPaths = e.ChangedPaths == null ? "" : string.Join(Environment.NewLine,
                        e.ChangedPaths.Select(p => p.Action + " " + p.Path))
                }));
            }
            return Select(entries, limit);
        }
    }

}
