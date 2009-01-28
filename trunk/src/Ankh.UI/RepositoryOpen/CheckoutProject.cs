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
using System.Collections;
using Ankh.VS;
using System.IO;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.SccManagement;

namespace Ankh.UI.RepositoryOpen
{
    public partial class CheckoutProject : VSDialogForm
    {
        public CheckoutProject()
        {
            InitializeComponent();
            version.Revision = SvnRevision.Head;
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);
            version.Context = Context;
        }

        public SvnOrigin SvnOrigin
        {
            get { return version.SvnOrigin; }
            set { version.SvnOrigin = value; }
        }

        public SvnRevision Revision
        {
            get { return version.Revision; }
            set { version.Revision = value; }
        }

        public string SelectedPath
        {
            get { return directory.Text; }
            set { directory.Text = value; }
        }

        Uri _projectUri;
        public Uri ProjectUri
        {
            get { return _projectUri; }
            set
            {
                _projectUri = value;
                projectUrl.Text = (value != null) ? value.ToString() : "";

                if (Context != null && value != null)
                {
                    IFileIconMapper mapper = Context.GetService<IFileIconMapper>();

                    string txt = Path.GetExtension(SvnTools.GetFileName(value));

                    int ico = mapper.GetIconForExtension(txt);

                    projectIcon.Image = CreateIcon(mapper.ImageList, ico);
                }
            }
        }

        Image CreateIcon(ImageList imgList, int index)
        {
            Bitmap bmp = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            using (Graphics g = Graphics.FromImage(bmp))
            using( SolidBrush sb = new SolidBrush(BackColor))
            {
                g.FillRectangle(sb, 0, 0, 16, 16);
                imgList.Draw(g, 0, 0, 16, 16, index);
            }

            return bmp;
        }

        Uri _rootUri;
        public Uri RepositoryRootUri
        {
            get { return _rootUri; }
            set { _rootUri = value; }
        }

        Uri _projectTop;
        public Uri ProjectTop
        {
            get { return (Uri)checkOutFrom.SelectedItem; }
            set 
            { 
                _projectTop = value;
                if (value != null)
                {
                    int l = value.ToString().Length;
                    foreach (Uri uri in new ArrayList(checkOutFrom.Items))
                    {
                        if (uri.ToString().Length > l)
                            checkOutFrom.Items.Remove(uri);
                    }

                    if (checkOutFrom.SelectedIndex < 0)
                        checkOutFrom.SelectedIndex = 0;
                }
            }
        }


        Uri _checkOutUri;
        public Uri CheckOutUri
        {
            get { return _checkOutUri; }
            set
            {
                _checkOutUri = value;
                checkOutFrom.Text = (value != null) ? value.ToString() : "";
            }
        }

        private void browseDirectory_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.ShowNewFolderButton = true;
                fbd.SelectedPath = SelectedPath;
                fbd.Description = "Select the location where you wish to save this project";

                if (fbd.ShowDialog(this) == DialogResult.OK)
                    SelectedPath = fbd.SelectedPath;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (checkOutFrom != null && RepositoryRootUri != null && ProjectUri != null)
            {
                checkOutFrom.Items.Clear();

                Uri inner = ProjectTop ?? new Uri(ProjectUri, "./");

                Uri info = RepositoryRootUri.MakeRelativeUri(inner);

                if(info.IsAbsoluteUri || !info.ToString().StartsWith("../", StringComparison.Ordinal))
                    RepositoryRootUri = new Uri(inner, "/");

                while(inner != RepositoryRootUri)
                {
                    checkOutFrom.Items.Add(inner);
                    inner = new Uri(inner, "../");
                }

                checkOutFrom.Items.Add(inner);

                // Ok, let's find some sensible default

                // First use our generic guess algorithm as used by branching
                RepositoryLayoutInfo li;
                if (RepositoryUrlUtils.TryGuessLayout(Context, ProjectUri, out li))
                {
                    foreach (Uri uri in checkOutFrom.Items)
                    {
                        if (uri == li.WorkingRoot)
                        {
                            checkOutFrom.SelectedItem = uri;
                            break;
                        }
                    }
                }

                if(checkOutFrom.SelectedIndex < 0)
                    foreach (Uri uri in checkOutFrom.Items)
                    {
                        string txt = uri.ToString();

                        if (txt.EndsWith("/trunk/", StringComparison.OrdinalIgnoreCase))
                        {
                            checkOutFrom.SelectedItem = uri;
                            break;
                        }
                    }

                if (checkOutFrom.SelectedIndex < 0)
                    foreach (Uri uri in checkOutFrom.Items)
                    {
                        string txt = uri.ToString();

                        if (txt.EndsWith("/branches/", StringComparison.OrdinalIgnoreCase) ||
                            txt.EndsWith("/tags/", StringComparison.OrdinalIgnoreCase) ||
                            txt.EndsWith("/releases/", StringComparison.OrdinalIgnoreCase))
                        {
                            int nIndex = checkOutFrom.Items.IndexOf(uri);

                            if (nIndex > 1)
                            {
                                checkOutFrom.SelectedIndex = nIndex - 1;
                                break;
                            }
                        }
                    }

                if (checkOutFrom.SelectedIndex < 0)
                    foreach (Uri uri in checkOutFrom.Items)
                    {
                        string txt = uri.ToString();

                        if (txt.EndsWith("/src/", StringComparison.OrdinalIgnoreCase) ||
                            txt.EndsWith("/source/", StringComparison.OrdinalIgnoreCase) ||
                            txt.EndsWith("/sourcecode/", StringComparison.OrdinalIgnoreCase))
                        {
                            int nIndex = checkOutFrom.Items.IndexOf(uri);

                            if (nIndex < checkOutFrom.Items.Count-1)
                            {
                                checkOutFrom.SelectedIndex = nIndex + 1;
                                break;
                            }
                        }
                    }

                if (checkOutFrom.SelectedIndex < 0 && checkOutFrom.Items.Count > 0)
                    checkOutFrom.SelectedIndex = 0;

                version.Context = Context;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string path = directory.Text;

            if (!SvnItem.IsValidPath(path) || File.Exists(path))
            {
                MessageBox.Show(this, "Path is not valid", "Open Project from Subversion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            directory.Text = path = SvnTools.GetNormalizedFullPath(path);

            if (Directory.Exists(path))
            {
                DirectoryInfo di = new DirectoryInfo(path);
                if (EnumTools.GetFirst(di.GetFileSystemInfos()) != null)
                {
                    if (MessageBox.Show(this, string.Format("{0} already contains files or directories.\nWould you like to continue?", path)
                        , "Open Project from Subversion", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
                    {
                        return;
                    }
                }
            }
            else
                Directory.CreateDirectory(path);

            DialogResult = DialogResult.OK;
        }
    }
}
