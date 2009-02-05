// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = Context.GetService<IAnkhErrorHandler>();
                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
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

            if (pr.RunModal("Retrieving remote status",
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
                                if (IgnoreStatus(stat))
                                    return; // Not a synchronization item
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

        static bool IgnoreStatus(SvnStatusEventArgs stat)
        {
            switch(stat.LocalContentStatus)
            {
                case SvnStatus.NotVersioned:
                case SvnStatus.Ignored:
                case SvnStatus.External: // External root will be handled inside
                    return (stat.RemoteContentStatus == SvnStatus.None);
                case SvnStatus.None:
                    // Hide remote locked files
                    return (stat.RemoteContentStatus == SvnStatus.None);
                default:
                    return false;
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
            updateTime.Text = string.Format(PCStrings.RefreshTimeX, DateTime.Now.ToShortTimeString());
        }
    }
}
