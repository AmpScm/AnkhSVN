using System;
using Ankh.UI;
using NSvn.Core;
using System.IO;

using System.Collections;

namespace Ankh.RepositoryExplorer
{
    /// <summary>
    /// Define some extra properties we need to track.
    /// </summary>
    internal interface INode : IRepositoryTreeNode
    {
        INode Parent
        {
            get;
        }

        string Url
        {
            get;
        }

        Revision Revision
        {
            get;
        }
    }
    /// <summary>
    /// Represents a normal node in the repository treeview.
    /// </summary>
    internal class Node : INode
    {  
        public Node( INode parent, DirectoryEntry entry )
        {
            this.parent = parent;
            this.entry = entry;
        }

        #region IRepositoryTreeNode Members
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

        public bool IsDirectory
        {
            get
            {
                return this.entry.NodeKind == NodeKind.Directory;
            }
        }

        public string Name
        {
            get
            {
                return this.entry.Path;
            }
        }
        #endregion

        public string Url
        {
            get
            {
                return UriCombine( this.Parent.Url, this.entry.Path );
            }
        }

        public INode Parent
        {
            get{ return this.parent; }
        }

        public Revision Revision
        {
            get{ return this.Parent.Revision; }
        }

        /// <summary>
        /// Concatenates two URI segments, placing a / in between as appropriate.
        /// </summary>
        /// <param name="uri1"></param>
        /// <param name="uri2"></param>
        /// <returns></returns>
        private static string UriCombine( string uri1, string uri2 )
        {
            bool uri1HasSlash = uri1.EndsWith("/");
            bool uri2HasSlash = uri2.StartsWith( "/" );

            if ( uri1HasSlash && uri2HasSlash )
                return uri1.Substring(0, uri1.Length-1) + uri2;
            else if ( uri1HasSlash ^ uri2HasSlash )
                return uri1 + uri2;
            else
                return uri1 + "/" + uri2;
        }

        private object tag;
        private DirectoryEntry entry;
        private INode parent;
    }

    /// <summary>
    /// Represents a root node in the treeview. This holds some information the others
    /// don't.
    /// </summary>
    internal class RootNode : INode
    {
        public RootNode( string url, Revision revision )
        {
            this.url = url;
            this.revision = revision;
        }
    
        #region IRepositoryTreeNode Members
        public object Tag
        {
            get { return this.tag; }
            set { this.tag = value; }
        }

        public bool IsDirectory
        {
            get{ return true; }
        }

        public string Name
        {
            get{ return this.url; }
        }

        
        #endregion
        public INode Parent
        {
            get{ return null; }
        }

        public string Url
        {
            get{ return this.url; }
        }

        public Revision Revision
        {
            get{ return this.revision; }
        }

        private object tag;
        private string url;
        private Revision revision;
    }
}
