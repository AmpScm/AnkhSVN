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
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.VS;
using Microsoft.VisualStudio;

namespace Ankh.WorkingCopyExplorer
{
    class WorkingCopyExplorer : AnkhService, Ankh.IWorkingCopyExplorer
    {
        readonly List<FileSystemRootItem> _roots;
        WorkingCopyExplorerControl _control;

        public WorkingCopyExplorer(IAnkhServiceProvider context)
            : base(context)
        {
            _roots = new List<FileSystemRootItem>();

            if (Shell != null && Shell.WorkingCopyExplorer != null)
            {
                SetControl(Shell.WorkingCopyExplorer);
            }
        }

        IExplorersShell _shell;
        IExplorersShell Shell
        {
            get { return _shell ?? (_shell = GetService<IExplorersShell>()); }
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

            foreach (string path in Directory.GetFileSystemEntries(directoryItem.FullPath))
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

        internal void OpenItem(IAnkhServiceProvider context, string path)
        {
            if (!File.Exists(path))
                return;

            try
            {
                IAnkhSolutionSettings settings = context.GetService<IAnkhSolutionSettings>();

                string filter = settings.AllProjectExtensionsFilter;

                string pathExt = Path.GetExtension(path);

                foreach (string ext in filter.Split(';'))
                {
                    string ex = ext.Trim().Replace("*", "");

                    if (StringComparer.OrdinalIgnoreCase.Equals(ex, pathExt))
                    {
                        IVsSolution sol = context.GetService<IVsSolution>(typeof(SVsSolution));

                        ErrorHandler.ThrowOnFailure(sol.OpenSolutionFile(0, path)); // This method allows opening projects and solutions
                        return;
                    }
                }


                IVsUIHierarchy hier;
                IVsWindowFrame frame;
                uint id;


                VsShellUtilities.OpenDocument(context, path, VSConstants.LOGVIEWID_TextView, out hier, out id, out frame);
            }
            catch (Exception exception)
            {
                IAnkhErrorHandler handler = context.GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(exception);
                else
                    throw;
            }
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
