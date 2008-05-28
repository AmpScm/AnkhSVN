using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.RepositoryExplorer
{
    sealed class RepositoryExplorerItem : CustomTypeDescriptor, ISvnRepositoryItem
    {
        RepositoryTreeNode _tn;
        RepositoryListItem _li;
        string _name;
        Uri _uri;

        public RepositoryExplorerItem(RepositoryTreeNode tn)
        {
            if (tn == null)
                throw new ArgumentNullException("tn");

            _tn = tn;
            _uri = tn.RawUri;
            _name = tn.Text;
        }

        public RepositoryExplorerItem(RepositoryListItem li)
        {
            if (li == null)
                throw new ArgumentNullException("li");

            _li = li;
            _uri = li.RawUri;
            _name = li.Text;
        }

        #region Property View Conversion
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

        public override string GetComponentName()
        {
            return Name;
        }

        public override string GetClassName()
        {
            return "Repository Item";
        }
        #endregion

        public Uri Uri
        {
            get { return _uri; }
        }

        public string Name
        {
            get { return _name; }
        }

        [Browsable(false)]
        internal RepositoryListItem ListViewItem
        {
            get { return _li; }
        }

        [Browsable(false)]
        internal RepositoryTreeNode TreeNode
        {
            get { return _tn; }
        }

        #region ISvnRepositoryItem Members

        SharpSvn.SvnNodeKind ISvnRepositoryItem.NodeKind
        {
            get
            {
                Uri uri = Uri;

                if (uri != null)
                {
                    if (uri.ToString().EndsWith("/"))
                        return SvnNodeKind.Directory;
                    else
                        return SvnNodeKind.File;
                }

                return SvnNodeKind.Unknown;
            }
        }

        SharpSvn.SvnRevision ISvnRepositoryItem.Revision
        {
            get { return null; }
        }
        #endregion
    }
}
