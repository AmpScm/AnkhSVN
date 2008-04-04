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

namespace Ankh.UI.RepositoryOpen
{
    public partial class RepositoryOpenDialog : Form
    {
        IAnkhServiceProvider _context;
        Uri _lastUri;
        public RepositoryOpenDialog()
        {
            InitializeComponent();
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;

                if (value != null)
                    InitializeFromContext();
            }
        }

        int _dirOffset;
        int _fileOffset;
        void InitializeFromContext()
        {
            IFileIconMapper mapper = Context.GetService<IFileIconMapper>();

            dirView.SmallImageList = mapper.ImageList;
            _dirOffset = mapper.DirectoryIcon;
            _fileOffset = mapper.FileIcon;
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (urlBox == null)
                return;

            if (urlBox.Items.Count == 0)
            {
                try
                {
                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\AnkhSVN\\AnkhSVN\\CurrentVersion\\Dialogs", RegistryKeyPermissionCheck.ReadSubTree))
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

                    // Allow corporate rollout of a default repository list via group policy
                    using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(
                        "SOFTWARE\\AnkhSVN\\AnkhSVN\\CurrentVersion\\Subversion Repositories", RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (rk != null)
                            foreach (string name in rk.GetValueNames())
                            {
                                string value = rk.GetValue(name) as string;

                                Uri uri;
                                if (value != null && Uri.TryCreate(value, UriKind.Absolute, out uri))
                                {
                                    if (!urlBox.Items.Contains(uri))
                                        urlBox.Items.Add(uri);
                                }
                            }
                    }

                    using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(
                        "SOFTWARE\\AnkhSVN\\AnkhSVN\\CurrentVersion\\Subversion Repositories", RegistryKeyPermissionCheck.ReadSubTree))
                    {
                        if (rk != null)
                            foreach (string name in rk.GetValueNames())
                            {
                                string value = rk.GetValue(name) as string;

                                Uri uri;
                                if (value != null && Uri.TryCreate(value, UriKind.Absolute, out uri))
                                {
                                    if (!urlBox.Items.Contains(uri))
                                        urlBox.Items.Add(uri);
                                }
                            }
                    }

                }
                catch (SecurityException)
                { /* Ignore no read only access; stupid sysadmins */ }

            }
            if (urlBox.Items.Count > 0 && string.IsNullOrEmpty(urlBox.Text.Trim()))
            {
                urlBox.SelectedIndex = 0;
                UpdateDirectories();
            }

            if (string.IsNullOrEmpty(fileTypeBox.Text) && fileTypeBox.Items.Count > 0)
                fileTypeBox.SelectedItem = fileTypeBox.Items[0];
        }

        string _filter;

        class FilterItem
        {
            string _name;
            string _filter;

            public FilterItem(string name, string filter)
            {
                _name = name;
                _filter = filter;
            }

            public override string ToString()
            {
                return _name;
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
            }
            finally
            {
                _inSetDirectory = false;
            }
        }

        private void urlBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(!_inSetDirectory)
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

        bool _changingDir;
        private void OnOkClicked(object sender, EventArgs e)
        {
            ProcessOk();
        }

        void ProcessOk()
        {
            UpdateDirectories();
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
                    nameUri = new Uri(dirUri, nameUri);

                if (nameUri.IsAbsoluteUri)
                {
                    // We have an absolute url. Split it in file and directory

                    string path = nameUri.AbsolutePath;

                    int dirEnd = path.LastIndexOf('/');

                    if (dirEnd > 1)
                    {
                        Uri dir = new Uri(nameUri, path.Substring(0, dirEnd + 1));
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
            _changingDir = false;

            string txt = urlBox.Text;

            if (!txt.EndsWith("/"))
                txt += '/';

            Uri uri;
            if (!Uri.TryCreate(txt, UriKind.Absolute, out uri))
                RefreshBox(null);

            RefreshBox(uri);
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

                if (parentUri == uri)
                    return;

                Uri fileUri;
                string fileText = fileNameBox.Text.Trim();
                if (!string.IsNullOrEmpty(fileText) &&
                    Uri.TryCreate(fileText.Trim(), UriKind.Relative, out fileUri))
                {
                    fileUri = parentUri.MakeRelativeUri(new Uri(uri, fileUri));

                    fileNameBox.Text = fileUri.ToString();
                }

                SetDirectory(parentUri);
                RefreshBox(parentUri);
            }
        }

        delegate void DoSomething();

        Uri _currentUri;
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
                    dirView.Items.AddRange(items.ToArray());
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
            }

            DoSomething fill = delegate()
            {
                OnFill(uri);
            };
            fill.BeginInvoke(null, null);
        }



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

                    Uri repositoryRoot = null;
                    List<ListViewItem> items = new List<ListViewItem>();
                    bool ok = client.List(uri, la,
                        delegate(object sender, SvnListEventArgs e)
                        {
                            if (string.IsNullOrEmpty(e.Path))
                            {
                                // The directory itself. This gives us the repository root.

                                // TODO: Move this code to the SvnListEventArgs object in SharpSvn

                                string uriStr = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped).Trim('/');

                                if (uriStr.EndsWith(e.BasePath))
                                {
                                    repositoryRoot = new Uri(uri, '/' + uriStr.Substring(0, uriStr.Length - e.BasePath.Length) + '/');
                                }
                                else if (e.BasePath == "/")
                                    repositoryRoot = new Uri(uri, '/' + uriStr + '/');

                                return;
                            }
                            else if (repositoryRoot == null)
                                return;

                            ListViewItem lvi = new ListViewItem();
                            lvi.Tag = new Uri(repositoryRoot, e.BasePath.Substring(1) + '/' + e.Path);
                            lvi.Text = e.Path;
                            lvi.ImageIndex = (e.Entry.NodeKind == SvnNodeKind.Directory) ? _dirOffset : _fileOffset;
                            items.Add(lvi);
                        });


                    if (IsHandleCreated)
                    {
                        DoSomething fill = delegate()
                        {
                            if (uri == _currentUri)
                            {
                                dirView.Items.Clear();

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


                                    dirView.Items.AddRange(items.ToArray());
                                    _walking[uri] = items;
                                }
                                else
                                    dirView.Items.Add("<Nothing>");
                            }
                        };

                        Invoke(fill);
                    }
                }
            }
            finally
            {
                lock (_running)
                {
                    _running.Remove(uri);
                }
                // Exception or something
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
                SelectItem(item);
                break;
            }
        }

        void SelectItem(ListViewItem item)
        {
            if (item.Tag == null)
                fileNameBox.Text = "";
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
                SelectItem(info.Item);
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
    }
}
