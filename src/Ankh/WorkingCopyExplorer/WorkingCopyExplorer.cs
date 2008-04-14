using System;
using System.Text;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Threading;
using System.Reflection;

using Utils;
using SharpSvn;
using Ankh.Scc;
using System.Collections.Generic;

namespace Ankh.WorkingCopyExplorer
{
    class WorkingCopyExplorer : AnkhService, Ankh.IWorkingCopyExplorer
    {
        readonly List<FileSystemRootItem> _roots;
        WorkingCopyExplorerControl _control;
        
        public WorkingCopyExplorer(IContext context)
            : base(context)
        {
            _roots = new List<FileSystemRootItem>();
            LoadRoots();

            if (context.UIShell.WorkingCopyExplorer != null)
            {
                SetControl(context.UIShell.WorkingCopyExplorer);
            }
        }

        private void LoadRoots()
        {
            DoLoadRoots();
        }

        IFileStatusCache _statusCache;
        protected IFileStatusCache StatusCache
        {
            get { return _statusCache ?? (_statusCache = GetService<IFileStatusCache>()); }
        }

        public void SetControl(WorkingCopyExplorerControl wcControl)
        {
            if (this._control != null)
            {
                if (wcControl != this._control)
                    throw new InvalidOperationException();
            }

            IStatusImageMapper mapper = GetService<IStatusImageMapper>();

            _control = wcControl;
            _control.StateImages = mapper.StatusImageList;
            _control.WantNewRoot += new EventHandler(control_WantNewRoot);
            _control.ValidatingNewRoot += new System.ComponentModel.CancelEventHandler(control_ValidatingNewRoot);
        }

        #region IWorkingCopyExplorer Members

        public void AddRoot(string directory)
        {
            FileSystemRootItem root = CreateRoot(directory);

            DoAddRoot(root);
        }

        public bool IsRootSelected
        {
            get
            {
                return GetSelectedRoot() != null;
            }
        }

        public void RemoveSelectedRoot()
        {
            FileSystemRootItem root = GetSelectedRoot();
            if (root != null)
            {
                this._roots.Remove(root);
                this._control.RemoveRoot(root);

                root.Dispose();
            }
        }

        private FileSystemRootItem GetSelectedRoot()
        {
            if (_control == null)
                return null;
            IFileSystemItem[] items = this._control.GetSelectedItems();

            if (items.Length != 1)
            {
                return null;
            }

            return items[0] as FileSystemRootItem;
        }

        #endregion

        #region ISelectionContainer Members

        public void RefreshSelectionParents()
        {
            foreach (IFileSystemItem selectedItem in this._control.GetSelectedItems())
            {
                FileSystemItem item = selectedItem as FileSystemItem;
                if (item != null && item.Parent != null)
                {
                    item.Parent.Refresh();
                }
            }
        }

        public void RefreshSelection()
        {
            foreach (IFileSystemItem selectedItem in this._control.GetSelectedItems())
            {
                FileSystemItem item = selectedItem as FileSystemItem;
                if (item != null)
                {
                    item.Refresh();
                }
            }
        }

        public void SyncAll()
        {
            foreach (FileSystemItem item in this._roots)
            {
                item.Refresh();
            }
        }

        public System.Collections.IList GetSelectionResources(bool getChildItems)
        {
            return this.GetSelectionResources(getChildItems, new Predicate<SvnItem>(SvnItemFilters.NoFilter));
        }

        public IList GetSelectionResources(bool getChildItems, Predicate<SvnItem> filter)
        {
            ArrayList selectedResources = new ArrayList();
            return selectedResources;
        }

        public IList GetAllResources(Predicate<SvnItem> filter)
        {
            ArrayList resources = new ArrayList();
            foreach (FileSystemItem item in this._roots)
            {
                item.GetResources(resources, true, filter);
            }
            return resources;
        }

        #endregion

        internal IFileSystemItem[] GetFileSystemItemsForDirectory(SvnItem directoryItem)
        {
            SortedList<string, IFileSystemItem> items = new SortedList<string, IFileSystemItem>(StringComparer.OrdinalIgnoreCase);

            SvnDirectory dir = StatusCache.GetDirectory(directoryItem.FullPath);

            if (dir != null)
            {
                SvnItem dirItem = dir.Directory;

                foreach (SvnItem item in dir)
                {
                    if (items.ContainsKey(item.Name))
                        items.Add(item.Name, FileSystemFileItem.Create(Context, this, item));
                }
            }

            foreach(string path in Directory.GetFileSystemEntries(directoryItem.FullPath))
            {
                string name = Path.GetFileName(path);
                if (!string.Equals(name, SvnClient.AdministrativeDirectoryName, StringComparison.OrdinalIgnoreCase) &&
                    !items.ContainsKey(name))
                {
                    SvnItem item = StatusCache[path];

                    items.Add(name, FileSystemItem.Create(Context, this, item));
                }
            }

            return new List<IFileSystemItem>(items.Values).ToArray();
        }

        internal void OpenItem(string path)
        {
            EnvDTE._DTE dte = GetService<EnvDTE._DTE>(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));
            if (File.Exists(path))
            {
                if (string.Equals(Path.GetExtension(path), ".sln", StringComparison.OrdinalIgnoreCase))
                {
                    dte.Solution.Open(path);
                }
                else if (path.ToLower().EndsWith("proj"))
                {
                    dte.ExecuteCommand("File.OpenProject", path);
                }
                else
                {
                    dte.ItemOperations.OpenFile(path, EnvDTE.Constants.vsViewKindPrimary);
                }
            }
        }

        void Context_Unloading(object sender, EventArgs e)
        {
            this.SaveRoots();
        }

        void control_ValidatingNewRoot(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !IsRootValid(this._control.NewRootPath);
        }

        private bool IsRootValid(string path)
        {
            return Directory.Exists(path) && SvnTools.IsManagedPath(path);
        }

        void control_WantNewRoot(object sender, EventArgs e)
        {
            if (this.IsRootValid(this._control.NewRootPath))
            {
                this.AddRoot(this._control.NewRootPath);
            }
        }

        private void SaveRoots()
        {
            string[] rootPaths = new string[this._roots.Count];
            for (int i = 0; i < rootPaths.Length; i++)
            {
                rootPaths[i] = ((FileSystemItem)this._roots[i]).SvnItem.FullPath;
            }
            GetService<IContext>().Configuration.SaveWorkingCopyExplorerRoots(rootPaths);
        }        

        private delegate void DoAddRootDelegate(FileSystemRootItem rootItem);

        private void DoLoadRoots()
        {
            string[] rootPaths = null;
            try
            {
                IContext ctx = GetService<IContext>();

                if (ctx == null)
                    return;

                rootPaths = ctx.Configuration.LoadWorkingCopyExplorerRoots();
                if (rootPaths == null || rootPaths.Length == 0)
                {
                    return;
                }

                foreach (string root in rootPaths)
                {
                    if (Directory.Exists(root))
                    {
                        FileSystemRootItem rootItem = CreateRoot(root);

                        DoAddRoot(rootItem);
                    }
                }

            }
            catch (TargetInvocationException ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex.InnerException);
                else
                    throw;
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);
                else
                    throw;
            }
        }

        private void DoAddRoot(FileSystemRootItem root)
        {
            this._control.AddRoot(root);
            this._roots.Add(root);
        }

        private FileSystemRootItem CreateRoot(string directory)
        {
            StatusCache.UpdateStatus(directory, SvnDepth.Infinity);
            SvnItem item = StatusCache[directory];
            FileSystemRootItem root = new FileSystemRootItem(Context, this, item);
            return root;
        }        
    }
}
