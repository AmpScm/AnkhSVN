//using System;
//using Ankh.UI;
//
//using System.Collections;
//
//namespace Ankh.RepositoryExplorer
//{
//	/// <summary>
//	/// Represents a node in the repository treeview.
//	/// </summary>
//	internal class Node : IRepositoryTreeNode
//	{	
//        public Node( RepositoryResource resource )
//        {
//            this.resource = resource;
//        }
//
//        /// <summary>
//        /// The resource associated with this node.
//        /// </summary>
//        public RepositoryResource Resource
//        {
//            [System.Diagnostics.DebuggerStepThrough]
//            get{ return this.resource; }
//        }
//	
//        #region IRepositoryTreeNode Members
//
//        public object Tag
//        {
//            [System.Diagnostics.DebuggerStepThrough]
//            get {  return this.tag;     }
//            [System.Diagnostics.DebuggerStepThrough]
//            set {  this.tag = value;  }
//        }
//
//        public bool IsDirectory
//        {
//            [System.Diagnostics.DebuggerStepThrough]
//            // TODO: Implement IsDirectory in RepositoryResource
//            get  { return resource is RepositoryDirectory; }
//        }
//
//        public string Name
//        {
//            [System.Diagnostics.DebuggerStepThrough]
//            get{ return resource.Name; }
//        }
//
//        public System.Collections.ICollection GetChildren()
//        {
//            try
//            {
//                if ( !this.IsDirectory )
//                    return new object[]{};
//                else
//                {
//                    ArrayList list = new ArrayList();
//                    foreach( RepositoryResource res in ((RepositoryDirectory)this.resource).GetChildren().Values )
//                        list.Add( new Node( res ) );
//                    return list;
//                }
//            }
//            catch( Exception ex )
//            {
//                Error.Handle( ex );
//                throw;
//            }
//        }
//
//        #endregion
//
//
//        private object tag;
//        private RepositoryResource resource;
//    }
//}
