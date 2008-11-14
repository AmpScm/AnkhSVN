﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.PendingChanges.Synchronize;
using Ankh.VS;

namespace Ankh.UI.PendingChanges
{
    partial class RecentChangesPage : PendingChangesPage
    {
        public RecentChangesPage()
        {
            InitializeComponent();
        }

        protected override Type PageType
        {
            get
            {
                return typeof(RecentChangesPage);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            syncView.Context = Context;

            IFileIconMapper mapper = Context.GetService<IFileIconMapper>();
            syncView.SmallImageList = mapper.ImageList;
            syncView.StateImageList = mapper.ImageList;
        }

        public override bool CanRefreshList
        {
            get { return true; }
        }

        public override void RefreshList()
        {
            try
            {
                DoRefresh();
            }
            catch (Exception e)
            {
                IAnkhErrorHandler eh = Context.GetService<IAnkhErrorHandler>();
                if (eh != null)
                    eh.OnError(e);
                else
                    throw;
            }
        }

        void DoRefresh()
        {
            IProgressRunner pr = Context.GetService<IProgressRunner>();
            IAnkhProjectLayoutService pls = Context.GetService<IAnkhProjectLayoutService>();

            List<SvnStatusEventArgs> resultList = new List<SvnStatusEventArgs>();
            List<string> roots = new List<string>(SvnItem.GetPaths(pls.GetUpdateRoots(null)));
            Dictionary<string, string> found = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (pr.Run("Retrieving remote status",
                delegate(object sender, ProgressWorkerArgs e)
                {
                    SvnStatusArgs sa = new SvnStatusArgs();
                    sa.RetrieveRemoteStatus = true;
                    foreach (string path in roots)
                    {
                        // TODO: Find some way to get this information safely in the status cache
                        // (Might not be possible because of delays in network check)
                        e.Client.Status(path, sa,
                            delegate(object s, SvnStatusEventArgs stat)
                            {
                                if (stat.LocalContentStatus == SvnStatus.NotVersioned
                                    && stat.RemoteContentStatus == SvnStatus.None)
                                {
                                    return; // Not a synchronization item
                                }
                                else if (found.ContainsKey(stat.FullPath))
                                    return; // Already reported

                                stat.Detach();
                                resultList.Add(stat);
                                found.Add(stat.FullPath, "");
                            });
                    }
                }).Succeeded)
            {
                RefreshFromList(resultList);
            }
        }

        private void RefreshFromList(List<SvnStatusEventArgs> resultList)
        {
            syncView.Items.Clear();
            IFileStatusCache fs = Context.GetService<IFileStatusCache>();
            List<SynchronizeListItem> items = new List<SynchronizeListItem>(resultList.Count);
            foreach (SvnStatusEventArgs s in resultList)
            {
                SvnItem item = fs[s.FullPath];

                if (item == null)
                    return;

                items.Add(new SynchronizeListItem(syncView, item, s));
            }

            syncView.Items.AddRange(items.ToArray());
        }
    }
}
