using System;
using Ankh.UI;

using System.IO;
using System.ComponentModel;
using Utils;
using System.Collections;
using SharpSvn;

namespace Ankh.RepositoryExplorer
{
    /// <summary>
    /// Define some extra properties we need to track.
    /// </summary>
    public interface INode : Ankh.UI.RepositoryExplorer.IRepositoryTreeNode
    {
        INode Parent
        {
            get;
        }

        string Url
        {
            get;
        }

        SvnRevision Revision
        {
            get;
        }
    }
    /// <summary>
    /// Represents a normal node in the repository treeview.
    /// </summary>
    public class Node : INode
    {  
        public Node( INode parent, SvnListEventArgs entry )
        {
            this.parent = parent;
            this.entry = entry;
        }

        #region IRepositoryTreeNode Members
        [Browsable(false)]
        public object Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                this.tag = value;
            }
        }

        [Category("Subversion")]
        public bool IsDirectory
        {
            get{ return this.entry.Entry.NodeKind == SvnNodeKind.Directory; }
        }

        [Category("Subversion")]
        public string Name
        {
            get{ return this.entry.Path; }
        }
        #endregion

        [Category("Subversion")]
        public string Url
        {
            get{ return UriUtils.Combine( this.Parent.Url, this.entry.Path );}
        }

        [Category("Subversion")]
        public string LastAuthor
        {
            get{ return this.entry.Entry.Author;}
        }

        [Category("Subversion")]
        public bool HasProperties
        {
            get{ ;return this.entry.Entry.HasProperties; }
        }

        [Category("Subversion")]
        public long CreatedRevision
        {
            get{ return this.entry.Entry.Revision; }
        }

        [Category("Subversion")]
        public long Size
        {
            get{ return this.entry.Entry.FileSize; }
        }

        [Category("Subversion")]
        public DateTime Time
        {
            get{ return this.entry.Entry.Time; }
        }

        [Browsable(false)]
        public INode Parent
        {
            get{ return this.parent; }
        }

        [Category("Subversion")]
        public SvnRevision Revision
        {
            get{ return this.Parent.Revision; }
        }

        

        private object tag;
        private SvnListEventArgs entry;
        private INode parent;
    }

    /// <summary>
    /// Represents a root node in the treeview. This holds some information the others
    /// don't.
    /// </summary>
    public class RootNode : INode
    {
        public RootNode( string url, SvnRevision revision )
        {
            this.url = url;
            this.revision = revision;
        }
    
        #region IRepositoryTreeNode Members
        [Browsable(false)]
        public object Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }

        [Category("Subversion")]
        public bool IsDirectory
        {
            get{ return true; }
        }

        [Category("Subversion")]
        public string Name
        {
            get{ return this.url; }
        }

        
        #endregion

        [Browsable(false)]
        public INode Parent
        {
            get{ return null; }
        }

        [Category("Subversion")]
        public string Url
        {
            get{ return this.url; }
        }

        [Category("Subversion")]
        public SvnRevision Revision
        {
            get{ return this.revision; }
        }

        private object tag;
        private string url;
        private SvnRevision revision;
    }
}
