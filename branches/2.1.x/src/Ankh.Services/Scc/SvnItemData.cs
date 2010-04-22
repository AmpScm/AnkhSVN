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
using System.Text;
using System.ComponentModel;
using Ankh.Selection;
using System.Diagnostics;
using SharpSvn;

namespace Ankh.Scc
{
    [DebuggerDisplay("File={FullPath}, Status={Status}")]
    public partial class SvnItemData : AnkhPropertyGridItem
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;
        public SvnItemData(IAnkhServiceProvider context, SvnItem item)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            _context = context;
            _item = item;
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnItem SvnItem
        {
            get { return _item; }
        }

        [DisplayName("Full Path"), Category("Misc")]
        public string FullPath
        {
            get { return _item.FullPath; }
        }

        [DisplayName("File Name"), Category("Misc")]
        public string Name
        {
            get { return _item.Name; }
        }

        [Browsable(false)]
        public string ChangeList
        {
            get { return _item.Status.ChangeList; }
            set
            {
                string cl = string.IsNullOrEmpty(value) ? null : value.Trim();

                if (_item.IsVersioned && _item.Status != null && _item.IsFile)
                {
                    if (value != _item.Status.ChangeList)
                    {
                        using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
                        {
                            if (cl != null)
                            {
                                SvnAddToChangeListArgs ca = new SvnAddToChangeListArgs();
                                ca.ThrowOnError = false;
                                client.AddToChangeList(_item.FullPath, cl);
                            }
                            else
                            {
                                SvnRemoveFromChangeListArgs ca = new SvnRemoveFromChangeListArgs();
                                ca.ThrowOnError = false;
                                client.RemoveFromChangeList(_item.FullPath, ca);
                            }
                        }
                    }
                }
            }
        }

        [DisplayName("Change List"), Category("Subversion"), DefaultValue(null)]
        public SvnChangeList ChangeListValue
        {
            get { return ChangeList; }
            set { ChangeList = value; }
        }

        [DisplayName("Project"), Category("Visual Studio")]
        public string Project
        {
            get 
            {
                IProjectFileMapper mapper = _context.GetService<IProjectFileMapper>();

                if (mapper != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (SvnProject p in mapper.GetAllProjectsContaining(FullPath))
                    {
                        ISvnProjectInfo info = mapper.GetProjectInfo(p);

                        if (info == null)
                        {
                            if (string.Equals(FullPath, mapper.SolutionFilename, StringComparison.OrdinalIgnoreCase))
                                return "<Solution>";
                        }
                        else
                        {
                            if (sb.Length > 0)
                                sb.Append(';');

                            sb.Append(info.UniqueProjectName);
                        }
                    }

                    return sb.ToString();
                }
                return ""; 
            }
        }

        PendingChangeStatus _chg;
        [DisplayName("Change"), Category("Subversion")]
        public string Change
        {
            get 
            {
                AnkhStatus status = _item.Status;
                PendingChangeKind kind = PendingChange.CombineStatus(status.LocalContentStatus, status.LocalPropertyStatus, SvnItem.IsTreeConflicted, SvnItem);

                if (kind == PendingChangeKind.None)
                    return "";

                if (_chg == null || _chg.State != kind)
                    _chg = new PendingChangeStatus(kind);

                return _chg.Text;
            }
        }

        [Category("Subversion"), Description("Current Revision")]
        public long? Revision
        {
            get 
            {
                if (SvnItem.IsVersioned)
                    return SvnItem.Status.Revision;
                else
                    return null;            
            }
        }

        [Category("Subversion"), Description("Last committed author"), DisplayName("Last Author")]
        public string LastCommittedAuthor
        {
            get { return SvnItem.Status.LastChangeAuthor; }
        }

        [Category("Subversion"), Description("Last committed revision"), DisplayName("Last Revision")]
        public long? LastCommittedRevision
        {
            get 
            {
                if (SvnItem.IsVersioned)
                    return SvnItem.Status.LastChangeRevision;
                else
                    return null;
            }
        }

        [DisplayName("Url"), Category("Subversion")]
        public Uri Uri
        {
            get { return SvnItem.Status.Uri; }
        }

        [Category("Subversion"), Description("Last committed date"), DisplayName("Last Committed")]
        public DateTime LastCommittedDate
        {
            get
            {
                DateTime dt = SvnItem.Status.LastChangeTime;
                if (dt != DateTime.MinValue)
                    return dt.ToLocalTime();
                else
                    return DateTime.MinValue;
            }
        }

        protected override string ComponentName
        {
            get { return Name; }
        }

        protected override string ClassName
        {
            get { return "Path Status"; }
        }
    }
}
