// $Id$
//
// Copyright 2003-2009 The AnkhSVN Project
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
using System.Diagnostics;
using Ankh.Services;
using Ankh.Commands;


namespace Ankh.VS.Extenders
{
    /// <summary>
    /// Extends <see cref="SvnItem"/> in the property grid
    /// </summary>
    [ComVisible(true)] // This class must be public or the extender won't accept it.
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class SvnItemExtender : IDisposable
    {
        readonly AnkhExtenderProvider _provider;
        readonly object _extendeeObject;
        readonly IDisposable _disposer;
        readonly string _catId;

        internal SvnItemExtender(object extendeeObject, AnkhExtenderProvider provider, IDisposable disposer, string catId)
        {
            if (extendeeObject == null)
                throw new ArgumentNullException("extendeeObject");
            else if (provider == null)
                throw new ArgumentNullException("provider");
            else if (disposer == null)
                throw new ArgumentNullException("disposer");
            else if (catId == null)
                throw new ArgumentNullException("catId");

            _extendeeObject = extendeeObject;
            _provider = provider;
            _disposer = disposer;
            _catId = catId;
#if DEBUG
            if (!Marshal.IsTypeVisibleFromCom(GetType()))
                throw new InvalidOperationException("Descendants of SvnItemExtender mist be visible from COM");
#endif
        }

        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        ~SvnItemExtender()
        {
            Dispose(false);
        }

        bool _disposed;
        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            _disposed = true;

            // Extenders must notify the site when they are disposed, but we might
            // be executing in a background thread
            IAnkhCommandService svc = _provider.CommandService;

            if (svc != null)
                svc.PostIdleAction(_disposer.Dispose);
        }

        private T GetService<T>() where T : class
        {
            return ((IAnkhServiceProvider)_provider).GetService<T>();
        }

        [Browsable(false)]
        internal SvnItem SvnItem
        {
            get
            {
                return _provider.FindItem(_extendeeObject, _catId);
            }
        }

        [Category("Subversion"), Description("Url"), DisplayName("Url")]
        public Uri Url
        {
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return null;
                return item.Status.Uri;
            }
        }

        [Category("Subversion"), DisplayName("Change List"), Description("Change List")]
        public SvnItemData.SvnChangeList ChangeList
        {
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return null;
                if (item.IsVersioned && item.IsFile)
                    return new SvnItemData.SvnChangeList(item.Status.ChangeList ?? "");
                else
                    return null;
            }
            set
            {
                string cl = value;

                SvnItem item = SvnItem;

                if (item == null)
                    return;

                cl = string.IsNullOrEmpty(cl) ? null : cl.Trim();

                if (item.IsVersioned && item.Status != null && item.IsFile)
                {
                    if (value != item.Status.ChangeList)
                    {
                        using (SvnClient client = GetService<ISvnClientPool>().GetNoUIClient())
                        {
                            if (cl != null)
                            {
                                SvnAddToChangeListArgs ca = new SvnAddToChangeListArgs();
                                ca.ThrowOnError = false;
                                client.AddToChangeList(item.FullPath, cl);
                            }
                            else
                            {
                                SvnRemoveFromChangeListArgs ca = new SvnRemoveFromChangeListArgs();
                                ca.ThrowOnError = false;
                                client.RemoveFromChangeList(item.FullPath, ca);
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
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return null;

                return item.Status.LastChangeAuthor;
            }
        }

        [Category("Subversion"), Description("Current Revision")]
        public long? Revision
        {
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return null;

                if (item.IsVersioned && item.Status.Revision > 0)
                    return item.Status.Revision;
                else
                    return null;
            }
        }

        [Category("Subversion"), Description("Last committed date"), DisplayName("Last Committed")]
        public DateTime LastCommittedDate
        {
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return DateTime.MinValue;

                DateTime dt = item.Status.LastChangeTime;
                if (dt != DateTime.MinValue)
                    return dt.ToLocalTime();
                else
                    return DateTime.MinValue;
            }
        }

        [Category("Subversion"), Description("Last committed revision"), DisplayName("Last Revision")]
        public long? LastCommittedRevision
        {
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return null;

                if (item.IsVersioned && item.Status.LastChangeRevision > 0)
                    return item.Status.LastChangeRevision;
                else
                    return null;
            }
        }

        PendingChangeStatus _chg;
        [DisplayName("Change"), Category("Subversion")]
        public string Change
        {
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return null;

                AnkhStatus status = item.Status;
                PendingChangeKind kind = PendingChange.CombineStatus(status.LocalContentStatus, status.LocalPropertyStatus, status.HasTreeConflict, item);

                if (kind == PendingChangeKind.None)
                    return "";

                if (_chg == null || _chg.State != kind)
                    _chg = new PendingChangeStatus(kind);

                return _chg.Text;
            }
        }

        [Category("Subversion"), Description("Locked")]
        public bool? Locked
        {
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return null;

                return item.Status.IsLockedLocal;
            }
        }

        [Category("Properties"), DisplayName("Line ending style"), Description("svn:eol-style")]
        public LineEndingStyle LineEndingStyle
        {
            get
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return LineEndingStyle.None;

                using (SvnClient c = GetNoUIClient())
                {
                    if (c == null)
                        return LineEndingStyle.None;

                    string value;
                    if (item.Exists && c.TryGetProperty(item.FullPath, SvnPropertyNames.SvnEolStyle, out value))
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
                SvnItem item = SvnItem;

                if (item == null)
                    return;

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
                        c.DeleteProperty(item.FullPath, SvnPropertyNames.SvnEolStyle);
                    else
                        c.SetProperty(item.FullPath, SvnPropertyNames.SvnEolStyle, strValue);

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
                SvnItem item = SvnItem;

                if (item == null)
                    return null;

                if (item.IsDirectory || !item.IsVersioned)
                    return null;

                using (SvnClient c = GetNoUIClient())
                {
                    if (c == null)
                        return null;

                    string value;
                    try
                    {
                        if (c.TryGetProperty(item.FullPath, SvnPropertyNames.SvnMimeType, out value))
                            return value;
                    }
                    catch { }
                }
                return null;
            }
            set
            {
                SvnItem item = SvnItem;

                if (item == null)
                    return;

                using (SvnClient c = GetNoUIClient())
                {
                    if (c == null)
                        return;

                    if (string.IsNullOrEmpty(value))
                        c.DeleteProperty(item.FullPath, SvnPropertyNames.SvnMimeType);
                    else
                        c.SetProperty(item.FullPath, SvnPropertyNames.SvnMimeType, value);

                    ScheduleUpdate();
                }
            }
        }

        #region Helpers
        SvnPoolClient GetNoUIClient()
        {
            ISvnClientPool pool = GetService<ISvnClientPool>();
            if (pool == null)
                return null;

            return pool.GetNoUIClient();
        }

        void ScheduleUpdate()
        {
            SvnItem item = SvnItem;

            if (item == null)
                return;

            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();
            if (monitor == null)
                return;

            monitor.ScheduleSvnStatus(item.FullPath);
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
