// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Text;
using Ankh.Scc;
using System.ComponentModel;
using SharpSvn;

namespace Ankh.UI.PendingChanges.Synchronize
{
    sealed class SynchronizeItem : AnkhPropertyGridItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SynchronizeListItem _listItem;

        SvnItem SvnItem
        {
            get { return _listItem.SvnItem; }
        }

        public SynchronizeItem(IAnkhServiceProvider context, SynchronizeListItem listItem)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (listItem == null)
                throw new ArgumentNullException("listItem");

            _context = context;
            _listItem = listItem;
        }

        protected override string ClassName
        {
            get { return "Recent Change"; }
        }

        protected override string ComponentName
        {
            get { return SvnItem.Name; }
        }

        internal SynchronizeListItem ListItem
        {
            get { return _listItem; }
        }

        [DisplayName("Full Path")]
        public string FullPath
        {
            get { return _listItem.SvnItem.FullPath; }
        }

        [DisplayName("File Name")]
        public string Name
        {
            get { return SvnItem.Name; }
        }

        [DisplayName("Change List"), Category("Subversion")]
        public string ChangeList
        {
            get { return SvnItem.Status.ChangeList; }
            set
            {
                string cl = string.IsNullOrEmpty(value) ? null : value.Trim();

                if (SvnItem.IsVersioned && SvnItem.Status != null && SvnItem.IsFile)
                {
                    if (value != SvnItem.Status.ChangeList)
                    {
                        using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
                        {
                            if (cl != null)
                            {
                                SvnAddToChangeListArgs ca = new SvnAddToChangeListArgs();
                                ca.ThrowOnError = false;
                                client.AddToChangeList(SvnItem.FullPath, cl);
                            }
                            else
                            {
                                SvnRemoveFromChangeListArgs ca = new SvnRemoveFromChangeListArgs();
                                ca.ThrowOnError = false;
                                client.RemoveFromChangeList(SvnItem.FullPath, ca);
                            }
                        }
                    }
                }
            }
        }
    }
}
