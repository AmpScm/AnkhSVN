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

            string urlBoxText = urlBox.Text.Trim(); // Url's can not contain whitespace

            if (string.IsNullOrEmpty(urlBoxText) || !Uri.TryCreate(urlBoxText, UriKind.Absolute, out dirUri))
            {
                dirUri = null;
                urlBox.Text = "";
            }

            string name = fileNameBox.Text.Trim();
            if (!string.IsNullOrEmpty(name) && Uri.TryCreate(name, UriKind.RelativeOrAbsolute, out nameUri))
            {
                if (dirUri != null && !nameUri.IsAbsoluteUri && nameUri.ToString().Contains("/"))
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

                        urlBox.Text = dir.ToString();
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

            lastValidUri = uri;
            RefreshBox(uri);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

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

                urlBox.Text = parentUri.ToString();
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


                    if(IsHandleCreated)
                    {
                        DoSomething fill = delegate()
                        {                            
                            if (uri == _currentUri)
                            {
                                dirView.Items.Clear();
                                
                                if(ok)
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

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            Uri uri;

            if (Uri.TryCreate(urlBox.Text, UriKind.Absolute, out uri))
            {
                RefreshBox(uri);
            }
        }

        private void urlBox_Leave(object sender, EventArgs e)
        {
            if (lastValidUri != null)
                urlBox.Text = lastValidUri.ToString();
        }

        Uri lastValidUri;
        private void urlBox_TextChanged(object sender, EventArgs e)
        {
            _changingDir = true;
            Uri uri;

            string txt = urlBox.Text;

            if (!txt.EndsWith("/"))
                txt += '/';

            if (Uri.TryCreate(txt, UriKind.Absolute, out uri))
            {
                lastValidUri = uri;
            }
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
    }
}
