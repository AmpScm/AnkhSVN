﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace Ankh.UI.RepositoryOpen
{
    public partial class CheckoutProject : Form
    {
        IAnkhServiceProvider _context;
        public CheckoutProject()
        {
            InitializeComponent();
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                if (_context != value)
                {
                    _context = value;
                    OnContextChanged(EventArgs.Empty);
                }
            }
        }

        protected virtual void OnContextChanged(EventArgs e)
        {
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
            }
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

                // Ok, let's find some sensible default
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

                if (checkOutFrom.SelectedIndex < 0 && checkOutFrom.Items.Count > 0)
                    checkOutFrom.SelectedIndex = 0;
            }
        }            
    }
}
