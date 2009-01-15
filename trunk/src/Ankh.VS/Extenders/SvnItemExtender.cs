// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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
using System.Runtime.InteropServices;
using System.ComponentModel;

using SharpSvn;
using Ankh.Selection;
using Ankh.Scc;


namespace Ankh.VS.Extenders
{
    /// <summary>
    /// Extends <see cref="SvnItem"/> in the property grid
    /// </summary>
    [ComVisible(true)]
    public class SvnItemExtender
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;
        public SvnItemExtender(SvnItem item, IAnkhServiceProvider context)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            else if (context == null)
                throw new ArgumentNullException("context");

            _item = item;
            _context = context;
        }

        [Browsable(false)]
        private SvnItem SvnItem
        {
            get 
            {
                return _item; 
            }
        }


        [Category("Subversion"), Description("Url"), DisplayName("Url")]
        public Uri Url
        {
            get { return SvnItem.Status.Uri; }
        }

        [Category("Subversion"), DisplayName("Change List"), Description("Change List")]
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

        public bool ShouldSerializeChangeList()
        {
            return false; // Keep changelist non-bold
        }

        [Category("Subversion"), Description("Last committed author"), DisplayName("Last Author")]
        public string LastCommittedAuthor
        {
            get { return SvnItem.Status.LastChangeAuthor; }
        }

        [Category("Subversion"), Description("Current Revision")]
        public long Revision
        {
            get { return SvnItem.Status.Revision; }
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

        [Category("Subversion"), Description("Last committed revision"), DisplayName("Last Revision")]
        public long LastCommittedRevision
        {
            get { return SvnItem.Status.LastChangeRevision; }
        }

        [Category("Subversion"), Description("Text Status"), DisplayName("Status Content")]
        public string TextStatus
        {
            get { return SvnItem.Status.LocalContentStatus.ToString(); }
        }

        [Category("Subversion"), Description("Property status"), DisplayName("Status Properties")]
        public string PropertyStatus
        {
            get { return SvnItem.Status.LocalPropertyStatus.ToString(); }
        }

        [Category("Subversion"), Description("Locked")]
        public bool Locked
        {
            get { return SvnItem.Status.IsLockedLocal; }
        }

        [Category("Properties"), DisplayName("Line ending style"), Description("svn:eol-style")]
        public LineEndingStyle LineEndingStyle
        {
            get
            {
                if (SvnItem == null)
                    return LineEndingStyle.None;

                using (SvnClient c = GetNoUIClient())
                {
                    if (c == null)
                        return LineEndingStyle.None;

                    string value;
                    if (SvnItem.Exists && c.TryGetProperty(new SvnPathTarget(SvnItem.FullPath), SvnPropertyNames.SvnEolStyle, out value))
                    {
                        try
                        {
                            return (LineEndingStyle)Enum.Parse(typeof(LineEndingStyle), value, true);
                        }
                        catch { }
                    }
                }
                return LineEndingStyle.None;
            }
            set
            {
                string strValue = null;
                switch (value)
                {
                    case LineEndingStyle.Native:
                        strValue = "native";
                        break;
                    case LineEndingStyle.None:
                        break;
                    default:
                        strValue = value.ToString();
                        break;
                }
                using (SvnClient c = GetNoUIClient())
                {
                    if (c == null)
                        return;

                    if (string.IsNullOrEmpty(strValue))
                        c.DeleteProperty(SvnItem.FullPath, SvnPropertyNames.SvnEolStyle);
                    else
                        c.SetProperty(SvnItem.FullPath, SvnPropertyNames.SvnEolStyle, strValue);

                    ScheduleUpdate();
                }
            }
        }

        public bool ShouldSerializeLineEndingStyle()
        {
            return false; // Keep LineEndingStyle field non-bold
        }

        [Category("Properties"), DisplayName("Mime type"), Description("svn:mime-type")]
        [DefaultValue(null)]
        public string MimeType
        {
            get
            {
                if (SvnItem == null || SvnItem.IsDirectory || !SvnItem.IsVersioned)
                    return null;

                using (SvnClient c = GetNoUIClient())
                {
                    if (c == null)
                        return null;

                    string value;
                    try
                    {
                        if (c.TryGetProperty(new SvnPathTarget(SvnItem.FullPath), SvnPropertyNames.SvnMimeType, out value))
                            return value;
                    }
                    catch { }
                }
                return null;
            }
            set
            {
                using (SvnClient c = GetNoUIClient())
                {
                    if (c == null)
                        return;

                    if (string.IsNullOrEmpty(value))
                        c.DeleteProperty(SvnItem.FullPath, SvnPropertyNames.SvnMimeType);
                    else
                        c.SetProperty(SvnItem.FullPath, SvnPropertyNames.SvnMimeType, value);

                    ScheduleUpdate();
                }
            }
        }

        #region Helpers
        SvnPoolClient GetNoUIClient()
        {
            if (_context == null)
                return null;

            ISvnClientPool pool = _context.GetService<ISvnClientPool>();
            if (pool == null)
                return null;

            return pool.GetNoUIClient();
        }

        void ScheduleUpdate()
        {
            if (_context == null || SvnItem == null)
                return;

            IFileStatusMonitor monitor = _context.GetService<IFileStatusMonitor>();
            if (monitor == null)
                return;

            monitor.ScheduleSvnStatus(SvnItem.FullPath);
        }
        #endregion
    }

    public enum LineEndingStyle
    {
        None,
        Native,
        CRLF,
        LF,
        CR
    }
}
