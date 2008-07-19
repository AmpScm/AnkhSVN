using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Ankh.Selection;
using System.Diagnostics;

namespace Ankh.Scc
{
    [DebuggerDisplay("File={FullPath}, Status={Status}")]
    public class SvnItemData : CustomTypeDescriptor
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnItem _item;
        public SvnItemData(IAnkhServiceProvider context, SvnItem item)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (item == null)
                throw new ArgumentNullException("item");

            _context = context;
            _item = item;
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
        }

        [DisplayName("Project"), Category("Visual Studio")]
        public string Project
        {
            get 
            {
                IProjectFileMapper mapper = _context.GetService<IProjectFileMapper>();

                if (mapper != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (SvnProject p in mapper.GetAllProjectsContaining(FullPath))
                    {
                        ISvnProjectInfo info = mapper.GetProjectInfo(p);

                        if (info == null)
                        {
                            if (string.Equals(FullPath, mapper.SolutionFilename, StringComparison.OrdinalIgnoreCase))
                                return "<Solution>";
                        }
                        else
                        {
                            if (sb.Length > 0)
                                sb.Append(';');

                            sb.Append(info.UniqueProjectName);
                        }
                    }

                    return sb.ToString();
                }
                return ""; 
            }
        }

        [DisplayName("Status"), Category("Subversion")]
        public string Status
        {
            get 
            {
                return _item.Status.LocalContentStatus.ToString(); 
            }
        }

        public override string GetComponentName()
        {
            return Name;
        }

        public override string GetClassName()
        {
            return "Path Status";
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
