using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.VS;
using SharpSvn;
using System.IO;

namespace Ankh.Scc
{
    [DebuggerDisplay("File={FullPath}, Change={ChangeText}")]
    public sealed class PendingChange : CustomTypeDescriptor
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;        

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingChange"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The item.</param>
        public PendingChange(RefreshContext context, SvnItem item)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            _context = context;
            _item = item;
            Refresh(context, item);
        }

        [Browsable(false)]
        public SvnItem Item
        {
            get { return _item; }
        }

        [DisplayName("Full Path"), Category("Subversion")]
        public string FullPath
        {
            get { return _item.FullPath; }
        }

        [DisplayName("File Name"), Category("Subversion")]
        public string Name
        {
            get { return _item.Name; }
        }

        [DisplayName("Change List"), Category("Subversion")]
        public string ChangeList
        {
            get { return _item.Status.ChangeList; }
            set
            {
                string cl = string.IsNullOrEmpty(value) ? null : value.Trim();

                if (_item.IsVersioned && _item.Status != null && _item.IsFile)
                {
                    if (value != _item.Status.ChangeList)
                    {
                        using (SvnClient client = _context.GetService<ISvnClientPool>().GetNoUIClient())
                        {
                            if (cl != null)
                            {
                                SvnAddToChangeListArgs ca = new SvnAddToChangeListArgs();
                                ca.ThrowOnError = false;
                                client.AddToChangeList(_item.FullPath, cl);
                            }
                            else
                            {
                                SvnRemoveFromChangeListArgs ca = new SvnRemoveFromChangeListArgs();
                                ca.ThrowOnError = false;
                                client.RemoveFromChangeList(_item.FullPath, ca);
                            }
                        }
                    }
                }
            }
        }

        [DisplayName("Project")]
        public string Project
        {
            get { return _projects; }
        }

        [DisplayName("Change"), Category("Subversion")]
        public string ChangeText
        {
            get { return _status.Text; }
        }

        public override string GetComponentName()
        {
            return Name;
        }

        public override string GetClassName()
        {
            return "Pending Change";
        }

        TypeConverter _rawDescriptor;
        TypeConverter Raw
        {
            get { return _rawDescriptor ?? (_rawDescriptor = TypeDescriptor.GetConverter(this, true)); }
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return Raw.GetProperties(this);
        }

        public override TypeConverter GetConverter()
        {
            return Raw;
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return Raw.GetProperties(null, null, attributes);
        }

        public override string ToString()
        {
            return Name;
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        /// <summary>
        /// Gets a boolean indicating whether this pending change is clear / is no longer a pending change
        /// </summary>
        [Browsable(false)]
        public bool IsClean
        {
            get { return !PendingChange.IsPending(Item); }
        }

        int _iconIndex;
        string _projects;
        string _relativePath;
        PendingChangeStatus _status;
        PendingChangeKind _kind;

        [Browsable(false)]
        public int IconIndex
        {
            get { return _iconIndex; }
        }

        [Browsable(false)]
        public PendingChangeKind Kind
        {
            get { return _kind; }
        }

        [Browsable(false)]
        public PendingChangeStatus Change
        {
            get { return _status; }
        }

        [Browsable(false)]
        public string RelativePath
        {
            get { return _relativePath; }
        }

        [Browsable(false)]
        public string LogMessageToolTipText
        {
            get { return String.Format("{0}: {1}", RelativePath, Change.PendingCommitText); }
        }

        /// <summary>
        /// Refreshes the pending change. Returns true if the state was modified, otherwise false
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Refresh(RefreshContext context, SvnItem item)
        {
            bool m = false;

            RefreshValue(ref m, ref _iconIndex, GetIcon(context));
            RefreshValue(ref m, ref _projects, GetProjects(context));
            RefreshValue(ref m, ref _status, GetStatus(context, item));
            RefreshValue(ref m, ref _relativePath, GetRelativePath(context));

            return m || (_status == null);
        }

        private string GetRelativePath(RefreshContext context)
        {
            string projectRoot = context.SolutionSettings.ProjectRootWithSeparator;

            if (!string.IsNullOrEmpty(projectRoot) && FullPath.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
                return FullPath.Substring(projectRoot.Length).Replace('\\', '/');
            else
                return FullPath;
        }

        string GetProjects(RefreshContext context)
        {
            string name = null;
            foreach (SvnProject project in context.ProjectFileMapper.GetAllProjectsContaining(FullPath))
            {
                ISvnProjectInfo info = context.ProjectFileMapper.GetProjectInfo(project);

                if (info == null)
                    continue;

                if (name != null)
                    name += ";" + info.UniqueProjectName;
                else
                    name = info.UniqueProjectName;
            }

            if (!string.IsNullOrEmpty(name))
                return name;
            else if(string.Equals(FullPath, context.SolutionSettings.SolutionFilename))
                return "<Solution>";
            else
                return "<none>";
        }

        int GetIcon(RefreshContext context)
        {
            if (Item.Exists)
                return context.IconMapper.GetIcon(FullPath);
            else
                return context.IconMapper.GetIconForExtension(Path.GetExtension(Name));
        }

        PendingChangeStatus GetStatus(RefreshContext context, SvnItem item)
        {
            AnkhStatus status = item.Status;

            switch (status.LocalContentStatus)
            {
                case SvnStatus.NotVersioned:
                    return new PendingChangeStatus(_kind = PendingChangeKind.New);
                case SvnStatus.Modified:
                    return new PendingChangeStatus(_kind = PendingChangeKind.Modified);
                case SvnStatus.Replaced:
                    return new PendingChangeStatus(_kind = PendingChangeKind.Replaced);
                case SvnStatus.Added:
                    if (item.Status.IsCopied)
                        return new PendingChangeStatus(_kind = PendingChangeKind.Copied);
                    else
                        return new PendingChangeStatus(_kind = PendingChangeKind.Added);
                case SvnStatus.Deleted:
                    return new PendingChangeStatus(_kind = PendingChangeKind.Deleted);
                case SvnStatus.Missing:
                    return new PendingChangeStatus(_kind = PendingChangeKind.Missing);
                case SvnStatus.Conflicted:
                    return new PendingChangeStatus(_kind = PendingChangeKind.Conflicted);
                case SvnStatus.Obstructed:
                    return new PendingChangeStatus(_kind = PendingChangeKind.Obstructed);

                //case SvnStatus.Zero:
                //case SvnStatus.Normal:
                //case SvnStatus.None:
                default:
                    break; // Look further
            }

            switch (status.LocalPropertyStatus)
            {
                case SharpSvn.SvnStatus.Normal:
                case SharpSvn.SvnStatus.None:
                    break; // Look further
                case SharpSvn.SvnStatus.Conflicted:
                    return new PendingChangeStatus(_kind = PendingChangeKind.PropertyConflicted);
                default:
                    return new PendingChangeStatus(_kind = PendingChangeKind.PropertyModified);
            }

            if (item.IsDocumentDirty)
                return new PendingChangeStatus(_kind = PendingChangeKind.EditorDirty);
            else
            {
                _kind = PendingChangeKind.None;
                return null;
            }
        }

        static void RefreshValue<T>(ref bool changed, ref T field, T newValue)
            where T : class, IEquatable<T>
        {
            if (field == null || !field.Equals(newValue))
            {
                changed = true;
                field = newValue;
            }
        }

        static void RefreshValue(ref bool changed, ref int field, int newValue)
        {
            if (field != newValue)
            {
                changed = true;
                field = newValue;
            }
        }

        public static bool IsPending(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            bool create = false;
            if (item.IsModified)
                create = true; // Must commit
            else if (item.InSolution && !item.IsVersioned && !item.IsIgnored && item.IsVersionable)
                create = true; // To be added
            else if (item.IsVersioned && item.IsDocumentDirty)
                create = true;

            return create;
        }

        /// <summary>
        /// Creates if pending.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="isDirty">if set to <c>true</c> [is dirty].</param>
        /// <param name="pc">The pc.</param>
        /// <returns></returns>
        public static bool CreateIfPending(RefreshContext context, SvnItem item, out PendingChange pc)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            if (IsPending(item))
            {
                pc = new PendingChange(context, item);
                return true;
            }
            
            pc = null;
            return false;
        }

        public sealed class RefreshContext : IAnkhServiceProvider
        {
            readonly IAnkhServiceProvider _context;
            public RefreshContext(IAnkhServiceProvider context)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                _context = context;
            }

            IProjectFileMapper _fileMapper;
            IFileIconMapper _iconMapper;
            IAnkhSolutionSettings _solutionSettings;

            public IProjectFileMapper ProjectFileMapper
            {
                [DebuggerStepThrough]
                get { return _fileMapper ?? (_fileMapper = GetService<IProjectFileMapper>()); }
            }

            public IFileIconMapper IconMapper
            {
                [DebuggerStepThrough]
                get { return _iconMapper ?? (_iconMapper = GetService<IFileIconMapper>()); }
            }

            public IAnkhSolutionSettings SolutionSettings
            {
                [DebuggerStepThrough]
                get { return _solutionSettings ?? (_solutionSettings = GetService<IAnkhSolutionSettings>()); }
            }

            #region IAnkhServiceProvider Members
            [DebuggerStepThrough]
            public T GetService<T>() where T : class
            {
                return _context.GetService<T>();
            }

            [DebuggerStepThrough]
            public T GetService<T>(Type serviceType) where T : class
            {
                return _context.GetService<T>(serviceType);
            }

            [DebuggerStepThrough]
            public object GetService(Type serviceType)
            {
                return _context.GetService(serviceType);
            }
            #endregion
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class PendingChangeCollection : KeyedCollection<string, PendingChange>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PendingChangeCollection"/> class.
        /// </summary>
        public PendingChangeCollection()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// Extracts the FullPath from the specified element.
        /// </summary>
        /// <param name="item">The element from which to extract the key.</param>
        /// <returns>The key for the specified element.</returns>
        protected override string GetKeyForItem(PendingChange item)
        {
            return item.FullPath;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(string key, out PendingChange value)
        {
            if (Dictionary == null)
            {
                // List is empty
                value = null;
                return false;
            }
            return Dictionary.TryGetValue(key, out value);
        }
    }
}
