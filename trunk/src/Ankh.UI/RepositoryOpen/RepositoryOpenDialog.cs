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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Threading;
using Ankh.VS;
using System.IO;
using Microsoft.Win32;
using System.Security;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Ankh.UI.RepositoryExplorer;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Forms.Design;

namespace Ankh.UI.RepositoryOpen
{
    public partial class RepositoryOpenDialog : VSDialogForm
    {
        public RepositoryOpenDialog()
        {
            InitializeComponent();
            dirView.ListViewItemSorter = new ItemSorter(this);
        }

        int _dirOffset;
        int _fileOffset;
        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            IFileIconMapper mapper = Context.GetService<IFileIconMapper>();

            dirView.SmallImageList = mapper.ImageList;
            _dirOffset = mapper.DirectoryIcon;
            _fileOffset = mapper.FileIcon;        

            IUIService ds = GetService<IUIService>();

            if (ds != null)
            {
                ToolStripRenderer renderer = ds.Styles["VsToolWindowRenderer"] as ToolStripRenderer;

                if (renderer != null)
                    toolStrip1.Renderer = renderer;
                else
                {
                    IAnkhVSColor color = Context.GetService<IAnkhVSColor>();

                    Color clr;
                    if (color != null && color.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_GRADIENT_MIDDLE, out clr))
                        toolStrip1.BackColor = clr;

                    if (color != null && color.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_HOVEROVERSELECTED, out clr))
                        toolStrip1.ForeColor = clr;

                    if (color != null && color.TryGetColor(__VSSYSCOLOREX.VSCOLOR_COMMANDBAR_TEXT_ACTIVE, out clr))
                        urlLabel.ForeColor = clr;
                }
            }
        }

        IAnkhSolutionSettings _solutionSettings;
        IAnkhSolutionSettings SolutionSettings
        {
            get { return _solutionSettings ?? (_solutionSettings = GetService<IAnkhSolutionSettings>()); }
        }

        IAnkhConfigurationService _config;
        IAnkhConfigurationService Config
        {
            get { return _config ?? (_config = GetService<IAnkhConfigurationService>()); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (urlBox == null)
                return;

            // Add current project root (if available) first
            if (SolutionSettings != null && SolutionSettings.ProjectRootUri != null)
            {
                if (!urlBox.Items.Contains(SolutionSettings.ProjectRootUri))
                    urlBox.Items.Add(SolutionSettings.ProjectRootUri);
            }

            if (Config != null)
            {
                // Add last used url
                using (RegistryKey rk = Config.OpenUserInstanceKey("Dialogs"))
                {
                    if (rk != null)
                    {
                        string value = rk.GetValue("Last Repository") as string;

                        Uri uri;
                        if (value != null && Uri.TryCreate(value, UriKind.Absolute, out uri))
                        {
                            if (!urlBox.Items.Contains(uri))
                                urlBox.Items.Add(uri);
                        }
                    }
                }

                foreach (string value in Config.GetRecentReposUrls())
                {
                    Uri uri;
                    if (value != null && Uri.TryCreate(value, UriKind.Absolute, out uri))
                    {
                        if (!urlBox.Items.Contains(uri))
                            urlBox.Items.Add(uri);
                    }
                }
            }



            if (SolutionSettings != null)
                foreach (Uri uri in SolutionSettings.GetRepositoryUris(true))
                {
                    if (!urlBox.Items.Contains(uri))
                        urlBox.Items.Add(uri);
                }

            if (urlBox.Items.Count > 0 && string.IsNullOrEmpty(urlBox.Text.Trim()))
            {
                urlBox.SelectedIndex = 0;
                UpdateDirectories();
            }

            if (string.IsNullOrEmpty(fileTypeBox.Text) && fileTypeBox.Items.Count > 0)
                fileTypeBox.SelectedItem = fileTypeBox.Items[0];
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_currentUri != null)
                try
                {
                    using (RegistryKey rk = Config.OpenUserInstanceKey("Dialogs"))
                    {
                        rk.SetValue("Last Repository", _currentUri.ToString());
                    }
                }
                catch
                { }
        }

        string _filter;

        class FilterItem
        {
            readonly string _name;
            readonly string _filter;
            readonly Regex _itemRegEx;

            public FilterItem(string name, string filter)
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");
                else if (filter == null)
                    throw new ArgumentNullException("filter");

                _name = name;
                _filter = filter;

                if (string.IsNullOrEmpty(filter) || filter == "*" || filter == "*.*")
                {
                    _itemRegEx = new Regex("^.*$", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture); // Match all
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("^(");

                bool first = true;
                foreach (string part in filter.Split(';'))
                {
                    string p = part.Trim();

                    sb.Append(first ? "(" : "|(");
                    first = false;
                    for (int i = 0; i < p.Length; i++)
                        switch (p[i])
                        {
                            case '*':
                                sb.Append(".*");
                                break;
                            case '?':
                                if (i + 1 >= p.Length || p[i + 1] == '.')
                                    sb.Append(".?"); // Dos artifact
                                else
                                    sb.Append(".");
                                break;
                            default:
                                sb.Append(Regex.Escape(p[i].ToString()));
                                break;
                        }
                    sb.Append(')');
                }
                sb.Append(")");

                _itemRegEx = new Regex(sb.ToString(), RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            }

            public override string ToString()
            {
                return _name;
            }

            public Regex Regex
            {
                get { return _itemRegEx; }
            }
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;

                fileTypeBox.Items.Clear();

                string[] parts = value.Split('|');
                for (int i = 0; i < parts.Length; i += 2)
                {
                    if (i + 1 > parts.Length)
                        continue;

                    fileTypeBox.Items.Add(new FilterItem(parts[i], parts[i + 1]));
                }

                if (fileTypeBox.SelectedValue == null && fileTypeBox.Items.Count > 0)
                    fileTypeBox.SelectedValue = fileTypeBox.Items[0];
            }
        }

        FilterItem CurrentFilter
        {
            get { return fileTypeBox.SelectedItem as FilterItem; }
        }

        bool _inSetDirectory;
        void SetDirectory(Uri uri)
        {
            _inSetDirectory = true;
            try
            {
                string v = uri.ToString();

                if (!v.EndsWith("/"))
                    uri = new Uri(v + "/");

                if (!urlBox.Items.Contains(uri))
                    urlBox.Items.Add(uri);

                urlBox.SelectedItem = uri;
                dirUpButton.Enabled = !_repositoryRoots.Contains(uri);
            }
            finally
            {
                _inSetDirectory = false;
            }
        }

        private void urlBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_inSetDirectory)
                OnDirChanged();
        }

        protected SvnClient GetClient()
        {
            ISvnClientPool pool = (Context != null) ? Context.GetService<ISvnClientPool>() : null;

            if (pool != null)
                return pool.GetClient();
            else
                return new SvnClient();
        }

        private void OnOkClicked(object sender, EventArgs e)
        {
            ProcessOk();
        }

        void ProcessOk()
        {
            UpdateDirectories();

            string fileText = fileNameBox.Text;

            if (string.IsNullOrEmpty(fileText))
                return;

            Uri dirUri;
            Uri fileUri;

            if (Uri.TryCreate(urlBox.Text, UriKind.Absolute, out dirUri) && Uri.TryCreate(fileText, UriKind.Relative, out fileUri))
            {
                Uri combined = SvnTools.AppendPathSuffix(dirUri, fileText);

                AnkhAction fill = delegate()
                {
                    CheckResult(combined);
                };
                fill.BeginInvoke(null, null);
            }
        }

        private void CheckResult(Uri combined)
        {
            CheckResult(combined, false);
        }

        private void CheckResult(Uri combined, bool forceLoad)
        {
            using (SvnClient client = Context.GetService<ISvnClientPool>().GetClient())
            {
                SvnInfoArgs args = new SvnInfoArgs();
                args.ThrowOnError = false;
                args.Depth = SvnDepth.Empty;

                client.Info(combined, args,
                    delegate(object sender, SvnInfoEventArgs e)
                    {
                        e.Detach();
                        if (!IsHandleCreated)
                            return;

                        Invoke((AnkhAction)delegate
                        {
                            if (e.NodeKind == SvnNodeKind.Directory)
                            {
                                Uri parentUri = new Uri(e.Uri, "../");

                                if (!forceLoad && parentUri.ToString() != urlBox.Text)
                                    return; // The user selected something else while we where busy

                                // The user typed a directory Url without ending '/'
                                fileNameBox.Text = e.Uri.ToString();
                                UpdateDirectories();
                                return;
                            }
                            else
                            {
                                Uri parentUri = new Uri(e.Uri, "./");

                                if (parentUri.ToString() != urlBox.Text)
                                    return; // The user selected something else while we where busy

                                SelectedUri = e.Uri;
                                SelectedRepositoryRoot = e.RepositoryRoot;
                                DialogResult = DialogResult.OK;
                            }
                        });
                    });
            }
        }

        Uri _selectedUri;
        public Uri SelectedUri
        {
            get { return _selectedUri; }
            set { _selectedUri = value; }
        }

        Uri _selectedRepositoryRoot;
        /// <summary>
        /// Contains the repository root of <see cref="SelectedUri"/> when available
        /// </summary>
        public Uri SelectedRepositoryRoot
        {
            get { return _selectedRepositoryRoot; }
            private set { _selectedRepositoryRoot = value; }
        }


        void UpdateDirectories()
        {
            Uri nameUri;
            Uri dirUri;

            string urlBoxText = urlBox.Text; // Url's can not contain whitespace

            if (string.IsNullOrEmpty(urlBoxText) || !Uri.TryCreate(urlBoxText, UriKind.Absolute, out dirUri))
            {
                dirUri = null;
                urlBox.Text = "";
            }

            string name = fileNameBox.Text.Trim();
            if (!string.IsNullOrEmpty(name) && Uri.TryCreate(name, UriKind.RelativeOrAbsolute, out nameUri))
            {
                if (dirUri != null && !nameUri.IsAbsoluteUri && nameUri.ToString().Contains("/") || (nameUri.ToString() == ".."))
                    nameUri = SvnTools.AppendPathSuffix(dirUri, name);

                if (nameUri.IsAbsoluteUri)
                {
                    // We have an absolute url. Split it in file and directory

                    string path = nameUri.GetComponents(UriComponents.PathAndQuery, UriFormat.SafeUnescaped);

                    int dirEnd = path.LastIndexOf('/');

                    if (dirEnd >= 0)
                    {
                        path = path.Substring(0, dirEnd + 1);

                        Uri uriRoot = new Uri(nameUri.GetComponents(UriComponents.StrongAuthority | UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));

                        if (uriRoot.IsFile)
                            path = path.TrimStart('/'); // Fixup for UNC paths

                        Uri dir = new Uri(uriRoot, path);
                        nameUri = dir.MakeRelativeUri(nameUri);

                        SetDirectory(dir);
                        fileNameBox.Text = nameUri.ToString();
                        RefreshBox(dir);
                    }
                }
                else if (dirUri == null)
                    return;
            }
        }

        void OnDirChanged()
        {
            string txt = urlBox.Text;

            if (!txt.EndsWith("/"))
                txt += '/';

            Uri uri;
            if (!Uri.TryCreate(txt, UriKind.Absolute, out uri))
                RefreshBox(null);

            RefreshBox(uri);
        }

        void ShowAddUriDialog()
        {
            IUIShell uiShell = GetService<IUIShell>();
            Uri dirUri = uiShell.ShowAddRepositoryRootDialog();

            AnkhAction action = delegate
            {
                CheckResult(dirUri, true);
            };

            action.BeginInvoke(null, null);
        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {
            OnDirChanged();
        }

        private void dirUpButton_Click(object sender, EventArgs e)
        {
            Uri uri;

            if (Uri.TryCreate(urlBox.Text.Trim(), UriKind.Absolute, out uri))
            {
                Uri parentUri = new Uri(uri, "../");

                if (parentUri == uri || _repositoryRoots.Contains(uri))
                    return;

                Uri fileUri;
                string fileText = fileNameBox.Text.Trim();
                if (!string.IsNullOrEmpty(fileText) &&
                    Uri.TryCreate(fileText.Trim(), UriKind.Relative, out fileUri))
                {
                    fileUri = parentUri.MakeRelativeUri(SvnTools.AppendPathSuffix(uri, fileText.Trim()));

                    fileNameBox.Text = fileUri.ToString();
                }

                SetDirectory(parentUri);
                RefreshBox(parentUri);
            }
        }

        Uri _currentUri;
        BusyOverlay _busy;
        bool _loading;
        readonly Dictionary<Uri, List<ListViewItem>> _walking = new Dictionary<Uri, List<ListViewItem>>();
        readonly Dictionary<Uri, Uri> _running = new Dictionary<Uri, Uri>();
        private void RefreshBox(Uri uri)
        {
            _currentUri = uri;
            dirView.Items.Clear();

            if (_walking.ContainsKey(uri))
            {
                List<ListViewItem> items = _walking[uri];

                if (items != null)
                    SetView(items.ToArray());
                else
                    dirView.Items.Add("<loading>");
            }
            else
                dirView.Items.Add("<loading>");


            lock (_running)
            {
                if (_running.ContainsKey(uri))
                    return;

                _running[uri] = uri; // Mark as walking

                if (!_loading)
                {
                    if (_busy == null)
                        _busy = new BusyOverlay(dirView, AnchorStyles.Right | AnchorStyles.Top);

                    _loading = true;
                    _busy.Show();
                }
            }

            AnkhAction fill = delegate()
            {
                OnFill(uri);
            };
            fill.BeginInvoke(null, null);
        }

        List<Uri> _repositoryRoots = new List<Uri>();

        void OnFill(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException();

            try
            {
                using (SvnClient client = GetClient())
                {
                    SvnListArgs la = new SvnListArgs();
                    la.ThrowOnError = false;
                    la.Depth = SvnDepth.Children;
                    la.RetrieveEntries = SvnDirEntryItems.Kind;

                    Uri repositoryRoot = null;
                    List<ListViewItem> items = new List<ListViewItem>();
                    bool ok = client.List(uri, la,
                        delegate(object sender, SvnListEventArgs e)
                        {
                            if (string.IsNullOrEmpty(e.Path))
                                return;

                            ListViewItem lvi = new ListViewItem();
                            lvi.Tag = e.EntryUri;
                            lvi.Text = e.Path;
                            lvi.ImageIndex = (e.Entry.NodeKind == SvnNodeKind.Directory) ? _dirOffset : _fileOffset;
                            items.Add(lvi);

                            if (repositoryRoot == null)
                                repositoryRoot = e.RepositoryRoot;
                        });


                    if (IsHandleCreated)
                    {
                        Invoke((AnkhAction)delegate()
                        {
                            if (uri == _currentUri)
                            {
                                dirView.Items.Clear();

                                if (repositoryRoot != null && !_repositoryRoots.Contains(repositoryRoot))
                                    _repositoryRoots.Add(repositoryRoot);

                                if (ok)
                                {
                                    IFileIconMapper mapper = Context != null ? Context.GetService<IFileIconMapper>() : null;

                                    foreach (ListViewItem item in items)
                                    {
                                        if (item.ImageIndex == _fileOffset)
                                        {
                                            string ext = Path.GetExtension(item.Text);

                                            if (!string.IsNullOrEmpty(ext))
                                            {
                                                int n = mapper.GetIconForExtension(ext);

                                                if (n > 0)
                                                    item.ImageIndex = n;
                                            }
                                        }
                                    }

                                    SetView(items.ToArray());
                                    _walking[uri] = items;
                                }
                                else
                                {
                                    string message = 
                                        string.Format("<{0}>",
                                        la.LastException != null ? la.LastException.Message : "Nothing");
                                    dirView.Items.Add(message);
                                }
                            }
                        });
                    }
                }
            }
            finally
            {
                Invoke((AnkhAction)delegate()
                {
                    lock (_running)
                    {
                        _running.Remove(uri);

                        if (_running.Count == 0)
                        {
                            if (_busy != null && _loading)
                            {
                                _loading = false;
                                _busy.Hide();
                            }
                        }
                    }
                });
                // Exception or something
            }
        }

        IList<ListViewItem> _currentItems;
        void SetView(IList<ListViewItem> items)
        {
            dirView.Items.Clear();
            _currentItems = items;

            if (items.Count > 0)
            {
                FilterItem filter = CurrentFilter;

                ListViewItem select = null;
                string selText = fileNameBox.Text;

                List<ListViewItem> newList = new List<ListViewItem>();
                foreach (ListViewItem i in items)
                {
                    // Show directories and matching items
                    if ((i.ImageIndex == _dirOffset) || filter == null || filter.Regex.Match(i.Text).Success)
                    {
                        if (!string.IsNullOrEmpty(selText) &&
                            selText.StartsWith(i.Text, StringComparison.Ordinal) &&
                            (i.Text.Length == selText.Length || selText[i.Text.Length] == '/'))
                        {
                            select = i;
                        }
                        newList.Add(i);
                    }
                }

                dirView.Items.AddRange(newList.ToArray());

                if (select != null)
                    select.Selected = true;
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            Uri uri = urlBox.SelectedItem as Uri;

            if (uri != null)
                RefreshBox(uri);
        }


        private void dirView_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in dirView.SelectedItems)
            {
                SelectItem(item, false);
                break;
            }
        }

        void SelectItem(ListViewItem item, bool fullUrl)
        {
            if (item.Tag == null)
                fileNameBox.Text = "";
            else if (fullUrl)
                fileNameBox.Text = ((Uri)item.Tag).AbsoluteUri;
            else if (item.ImageIndex == _dirOffset)
                fileNameBox.Text = item.Text + '/';
            else
                fileNameBox.Text = item.Text;
        }

        private void dirView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = dirView.HitTest(e.X, e.Y);

            if (info != null && info.Location != ListViewHitTestLocations.None)
            {
                SelectItem(info.Item, true);
                ProcessOk();
            }
        }

        private void dirView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;

            switch (e.KeyCode)
            {
                case Keys.Back:
                    if (ModifierKeys == Keys.None)
                    {
                        dirUpButton_Click(this, e);
                        e.Handled = true;
                    }
                    break;
                case Keys.Up:
                    if (ModifierKeys == Keys.Alt)
                    {
                        dirUpButton_Click(this, e);
                        e.Handled = true;
                    }
                    break;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Handled)
                return;

            switch (e.KeyCode)
            {
                case Keys.Back:
                    if (ModifierKeys == Keys.None)
                    {
                        dirUpButton_Click(this, e);
                        e.Handled = true;
                    }
                    break;
                case Keys.Up:
                    if (ModifierKeys == Keys.Alt)
                    {
                        dirUpButton_Click(this, e);
                        e.Handled = true;
                    }
                    break;
            }

            base.OnKeyDown(e);
        }

        private void fileNameBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
                return;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (ModifierKeys == Keys.Alt)
                    {
                        dirUpButton_Click(this, e);
                        e.Handled = true;
                    }
                    break;
            }

            base.OnKeyDown(e);
        }

        private void fileTypeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_currentItems != null)
                SetView(_currentItems); // Refilter current directory
        }

        sealed class ItemSorter : IComparer<ListViewItem>, System.Collections.IComparer
        {
            readonly RepositoryOpenDialog _dlg;

            public ItemSorter(RepositoryOpenDialog dlg)
            {
                if (dlg == null)
                    throw new ArgumentNullException("dlg");
                _dlg = dlg;
            }

            #region IComparer Members
            // This method is called by the ListView; map it to the typed version
            public int Compare(object x, object y)
            {
                return Compare((ListViewItem)x, (ListViewItem)y);
            }

            #endregion

            public int Compare(ListViewItem x, ListViewItem y)
            {
                if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                int dirOffset = _dlg._dirOffset;

                if ((x.ImageIndex == dirOffset) != (y.ImageIndex == dirOffset))
                    return (x.ImageIndex == dirOffset) ? -1 : 1;

                return StringComparer.OrdinalIgnoreCase.Compare(x.Text, y.Text);
            }
        }

        bool _forcedToolTip;
        private void fileNameBox_Enter(object sender, EventArgs e)
        {
            if (IsHandleCreated && Visible && !_forcedToolTip && urlBox.Items.Count == 0)
            {
                _forcedToolTip = true;
                string message = toolTip.GetToolTip(fileNameBox);
                toolTip.Show(message, fileNameBox);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            ShowAddUriDialog();
        }
    }
}
