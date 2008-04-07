using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Ankh.Scc;
using Ankh.Selection;
using System.Collections.ObjectModel;

namespace Ankh.Scc
{
    public sealed class PendingChange : CustomTypeDescriptor
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;        

        public PendingChange(IAnkhServiceProvider context, SvnItem item)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            _context = context;
            _item = item;
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

        public string Project
        {
            get
            {
                string name = null;
                IProjectFileMapper mapper = _context.GetService<IProjectFileMapper>();
                foreach (SvnProject project in mapper.GetAllProjectsContaining(FullPath))
                {
                    ISvnProjectInfo info = mapper.GetProjectInfo(project);

                    if (info == null)
                        continue;

                    if (name != null)
                        name += ";" + info.ProjectName;
                    else
                        name = info.ProjectName;                    
                }

                return name;
            }
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
        public bool IsClean
        {
            get { return false; }
        }

        /// <summary>
        /// Refreshes the pending change. Returns true if the state was modified, otherwise false
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Refresh(SvnItem item, bool isDirty)
        {
            return false;
        }

        /// <summary>
        /// Creates if pending.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="isDirty">if set to <c>true</c> [is dirty].</param>
        /// <param name="pc">The pc.</param>
        /// <returns></returns>
        public static bool CreateIfPending(SvnItem item, bool isDirty, out PendingChange pc)
        {
            pc = null;
            return false;
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
            return Dictionary.TryGetValue(key, out value);
        }
    }
}
