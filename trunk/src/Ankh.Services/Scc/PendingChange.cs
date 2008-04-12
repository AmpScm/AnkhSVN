﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.VS;
using SharpSvn;

namespace Ankh.Scc
{
    [DebuggerDisplay("File={FullPath}, Status={Status}")]
    public sealed class PendingChange : CustomTypeDescriptor
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;

        /// <summary>
        /// Initializes a new instance of the <see cref="PendingChange"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="item">The item.</param>
        public PendingChange(RefreshContext context, SvnItem item, bool isDirty)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            _context = context;
            _item = item;
            Refresh(context, item, isDirty);
        }

        [Browsable(false)]
        public SvnItem Item
        {
            get { return _item; }
        }

        [DisplayName("Full Path")]
        public string FullPath
        {
            get { return _item.FullPath; }
        }

        [DisplayName("File Name")]
        public string Name
        {
            get { return _item.Name; }
        }

        [DisplayName("Change List")]
        public string ChangeList
        {
            get { return _item.Status.ChangeList; }
        }

        [DisplayName("Project")]
        public string Project
        {
            get { return _projects; }
        }

        [DisplayName("Status")]
        public string StatusText
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
            get { return _status == null; }
        }

        int _iconIndex;
        string _projects;
        PendingChangeStatus _status;

        [Browsable(false)]
        public int IconIndex
        {
            get { return _iconIndex; }
        }

        [Browsable(false)]
        public PendingChangeStatus Status
        {
            get { return _status; }
        }

        /// <summary>
        /// Refreshes the pending change. Returns true if the state was modified, otherwise false
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Refresh(RefreshContext context, SvnItem item, bool isDirty)
        {
            bool m = false;

            RefreshValue(ref m, ref _iconIndex, context.IconMapper.GetIcon(FullPath));
            RefreshValue(ref m, ref _projects, GetProjects(context));
            RefreshValue(ref m, ref _status, GetStatus(context, item, isDirty));

            return m || (_status == null);
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
                    name += ";" + info.ProjectName;
                else
                    name = info.ProjectName;
            }

            if (!string.IsNullOrEmpty(name))
                return name;
            else if(string.Equals(FullPath, context.SolutionSettings.SolutionFilename))
                return "<Solution>";
            else
                return "<none>";
        }

        PendingChangeStatus GetStatus(RefreshContext context, SvnItem item, bool isDirty)
        {
            AnkhStatus status = item.Status;

            switch (status.LocalContentStatus)
            {
                case SharpSvn.SvnStatus.Normal:
                    break; // Look further
                case SharpSvn.SvnStatus.NotVersioned:
                    return new PendingChangeStatus(PendingChangeState.New);
                case SharpSvn.SvnStatus.Modified:
                    return new PendingChangeStatus(PendingChangeState.Modified);
                case SharpSvn.SvnStatus.Replaced:
                    return new PendingChangeStatus(PendingChangeState.Replaced);
                case SharpSvn.SvnStatus.Added:
                    if (item.Status.IsCopied)
                        return new PendingChangeStatus(PendingChangeState.Copied);
                    else
                        return new PendingChangeStatus(PendingChangeState.Added);
                case SharpSvn.SvnStatus.Deleted:
                    return new PendingChangeStatus(PendingChangeState.Deleted);
                case SharpSvn.SvnStatus.Missing:
                    return new PendingChangeStatus(PendingChangeState.Missing);
                // Default text is ok
                default:
                    return new PendingChangeStatus(status.LocalContentStatus.ToString());
            }

            switch (status.LocalPropertyStatus)
            {
                case SharpSvn.SvnStatus.Normal:
                case SharpSvn.SvnStatus.None:
                    break; // Look further
                default:
                    return new PendingChangeStatus("Property " + status.LocalPropertyStatus.ToString());
            }

            if (isDirty)
                return new PendingChangeStatus("Editted");
            else
                return null;
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
            if (field == newValue)
            {
                changed = true;
                field = newValue;
            }
        }

        /// <summary>
        /// Creates if pending.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="isDirty">if set to <c>true</c> [is dirty].</param>
        /// <param name="pc">The pc.</param>
        /// <returns></returns>
        public static bool CreateIfPending(RefreshContext context, SvnItem item, bool isDirty, out PendingChange pc)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            bool create = false;
            if (isDirty)
            {
                create = !item.IsIgnored;
            }
            else if (item.IsModified)
                create = true;

            if (create && item.Status.LocalContentStatus != SvnStatus.None)
            {
                pc = new PendingChange(context, item, isDirty);
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
