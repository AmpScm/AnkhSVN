using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Ankh.Scc;
using Ankh.Selection;

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
    }
}
