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
using Utils.Services;
using Ankh.Scc;

namespace Ankh.WorkingCopyExplorer
{
    class WorkingCopyExplorer : Ankh.IWorkingCopyExplorer
    {

        public WorkingCopyExplorer(IContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.context = context;
            this.statusCache = context.GetService<IFileStatusCache>();

            this.roots = new ArrayList();

            this.LoadRoots();

            if (context.UIShell.WorkingCopyExplorer != null)
            {
                SetControl(context.UIShell.WorkingCopyExplorer);
            }
        }

        public void SetControl(WorkingCopyExplorerControl wcControl)
        {
            if (this.control != null)
            {
                if (wcControl != this.control)
                    throw new InvalidOperationException();
            }

            IStatusImageMapper mapper = context.GetService<IStatusImageMapper>();

            control = wcControl;
            control.StateImages = mapper.StatusImageList;
            control.WantNewRoot += new EventHandler(control_WantNewRoot);
            control.ValidatingNewRoot += new System.ComponentModel.CancelEventHandler(control_ValidatingNewRoot);
        }

        public IContext Context
        {
            get { return this.context; }
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
                this.roots.Remove(root);
                this.control.RemoveRoot(root);

                root.Dispose();
            }
        }

        private FileSystemRootItem GetSelectedRoot()
        {
            if (control == null)
                return null;
            IFileSystemItem[] items = this.control.GetSelectedItems();

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
            foreach (IFileSystemItem selectedItem in this.control.GetSelectedItems())
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
            foreach (IFileSystemItem selectedItem in this.control.GetSelectedItems())
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
            foreach (FileSystemItem item in this.roots)
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
            foreach (FileSystemItem item in this.roots)
            {
                item.GetResources(resources, true, filter);
            }
            return resources;
        }

        #endregion

        internal IFileSystemItem[] GetFileSystemItemsForDirectory(SvnItem directoryItem)
        {
            ArrayList items = new ArrayList();
            foreach (string path in Directory.GetFileSystemEntries(directoryItem.FullPath))
            {
                if (PathUtils.GetName(path) != SvnClient.AdministrativeDirectoryName)
                {
                    SvnItem item = this.statusCache[path];
                    items.Add(FileSystemItem.Create(context, this, item));
                }
            }

            foreach (SvnItem item in this.statusCache.GetDeletions(directoryItem.FullPath))
            {
                items.Add(FileSystemItem.Create(context, this, item));
            }

            return (IFileSystemItem[])items.ToArray(typeof(IFileSystemItem));
        }

        internal void OpenItem(string path)
        {
            if (File.Exists(path))
            {
                if (path.ToLower().EndsWith(".sln"))
                {
                    ((IDTEContext)this.Context).DTE.Solution.Open(path);
                }
                else if (path.ToLower().EndsWith("proj"))
                {
                    ((IDTEContext)this.Context).DTE.ExecuteCommand("File.OpenProject", path);
                }
                else
                {
                    ((IDTEContext)this.Context).DTE.ItemOperations.OpenFile(path, EnvDTE.Constants.vsViewKindPrimary);
                }
            }
        }

        void Context_Unloading(object sender, EventArgs e)
        {
            this.SaveRoots();
        }

        void control_ValidatingNewRoot(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !IsRootValid(this.control.NewRootPath);
        }

        private bool IsRootValid(string path)
        {
            return Directory.Exists(path) && context.GetService<IWorkingCopyOperations>().IsWorkingCopyPath(path);
        }

        void control_WantNewRoot(object sender, EventArgs e)
        {
            if (this.IsRootValid(this.control.NewRootPath))
            {
                this.AddRoot(this.control.NewRootPath);
            }
        }

        private void SaveRoots()
        {
            string[] rootPaths = new string[this.roots.Count];
            for (int i = 0; i < rootPaths.Length; i++)
            {
                rootPaths[i] = ((FileSystemItem)this.roots[i]).SvnItem.FullPath;
            }
            this.Context.Configuration.SaveWorkingCopyExplorerRoots(rootPaths);
        }

        private void LoadRoots()
        {
            System.Threading.Thread t = new System.Threading.Thread(new ThreadStart(this.DoLoadRoots));
            t.Start();
        }

        private delegate void DoAddRootDelegate(FileSystemRootItem rootItem);

        private void DoLoadRoots()
        {
            string[] rootPaths;
            try
            {
                rootPaths = this.Context.Configuration.LoadWorkingCopyExplorerRoots();
                if (rootPaths == null)
                {
                    return;
                }

                foreach (string root in rootPaths)
                {
                    if (Directory.Exists(root))
                    {
                        FileSystemRootItem rootItem = CreateRoot(root);
                        this.Context.UIShell.SynchronizingObject.Invoke(
                            new DoAddRootDelegate(this.DoAddRoot), new object[] { rootItem });
                    }
                }

            }
            catch (TargetInvocationException ex)
            {
                IAnkhErrorHandler handler = context.GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex.InnerException);
                else
                    throw;
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = context.GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);
                else
                    throw;
            }
        }

        private void DoAddRoot(FileSystemRootItem root)
        {
            this.control.AddRoot(root);
            this.roots.Add(root);
        }

        private FileSystemRootItem CreateRoot(string directory)
        {
            this.statusCache.UpdateStatus(directory, SvnDepth.Infinity);
            SvnItem item = this.statusCache[directory];
            FileSystemRootItem root = new FileSystemRootItem(context, this, item);
            return root;
        }


        private WorkingCopyExplorerControl control;
        private IContext context;
        private IFileStatusCache statusCache;
        private ArrayList roots;
    }
}
