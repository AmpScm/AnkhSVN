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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.UI.VSSelectionControls;
using SharpSvn.Security;

namespace Ankh.UI.OptionsPages
{
    public partial class SvnAuthenticationCacheEditor : VSContainerForm
    {
        public SvnAuthenticationCacheEditor()
        {
            InitializeComponent();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            ResizeList();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
            {
                ResizeList();
                Refreshlist();
            }
        }

        private void Refreshlist()
        {
            credentialList.Items.Clear();
            SortedList<string, AuthenticationListItem> cache = new SortedList<string, AuthenticationListItem>();
            using (SvnClient client = Context.GetService<ISvnClientPool>().GetClient())
            {
                foreach(SvnAuthenticationCacheType tp in Enum.GetValues(typeof(SvnAuthenticationCacheType)))
                {
                    if(tp == SvnAuthenticationCacheType.None)
                        continue;

                    foreach(SvnAuthenticationCacheItem i in client.Authentication.GetCachedItems(tp))
                    {
                        if (i.RealmUri == null)
                            continue; // Just ignore local repositories

                        AuthenticationListItem lvi;
                        if(!cache.TryGetValue(i.Realm, out lvi))
                            cache[i.Realm] = lvi = new AuthenticationListItem(credentialList);

                        lvi.CacheItems.Add(i);
                    }
                }                
            }

            foreach (AuthenticationListItem i in cache.Values)
            {
                i.Refresh();
            }

            credentialList.Items.AddRange(new List<AuthenticationListItem>(cache.Values).ToArray());
        }
        void ResizeList()
        {
            if (!DesignMode && credentialList != null)
                credentialList.ResizeColumnsToFit(realmHeader, cachedHeader);
        }

        class AuthenticationListItem : SmartListViewItem
        {
            readonly List<SvnAuthenticationCacheItem> _items = new List<SvnAuthenticationCacheItem>();
            public AuthenticationListItem(SmartListView listview)
                : base(listview)
            {
            }

            public List<SvnAuthenticationCacheItem> CacheItems
            {
                get { return _items; }
            }

            public void Refresh()
            {
                SvnAuthenticationCacheItem i = CacheItems[0];
                SetValues(
                    i.RealmUri != null ? i.RealmUri.ToString() : "",
                    (i.Realm[0] == '<') ? i.Realm.Substring(i.Realm.IndexOf('>')+1).Trim() : i.Realm,
                    MakeNames());
            }

            private string MakeNames()
            {
                StringBuilder sb = new StringBuilder();
                bool next = false;

                foreach (SvnAuthenticationCacheItem i in CacheItems)
                {
                    if (next)
                        sb.Append(", ");
                    else
                        next = true;

                    switch ((SvnAuthenticationCacheType)i.CacheType)
                    {
                        case SvnAuthenticationCacheType.UserNamePassword:
                            sb.Append("Normal");
                            break;
                        default:
                            sb.Append(i.CacheType);
                            break;
                    }
                }

                return sb.ToString();
            }
        }

        private void credentialList_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (AuthenticationListItem li in credentialList.SelectedItems)
            {
                if (li != null)
                {
                    removeButton.Enabled = true;
                    return;
                }
            }

            removeButton.Enabled = false;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            AnkhMessageBox mb = new AnkhMessageBox(Context);

            if (DialogResult.OK != mb.Show(OptionsResources.TheSelectedCredentialsWillBeRemoved, "", MessageBoxButtons.OKCancel))
                return;

            bool changed = false;
            try
            {
                foreach (AuthenticationListItem li in credentialList.SelectedItems)
                {
                    foreach (SvnAuthenticationCacheItem i in li.CacheItems)
                    {
                        if (!i.IsDeleted)
                        {
                            changed = true;
                            i.Delete();
                        }
                    }
                }
            }
            finally
            {
                if (changed)
                {
                    Context.GetService<ISvnClientPool>().FlushAllClients();

                    Refreshlist();
                }
            }
        }
    }
}
