using System;
using System.Collections;
using NSvn;
using NSvn.Core;
using EnvDTE;

namespace Ankh.Solution
{
    /// <summary>
    /// Represents a node containing a project item.
    /// </summary>
    internal class ProjectItemNode : TreeNode
    {
        public ProjectItemNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer,
            TreeNode parent ) :
            base( item, hItem, explorer, parent )
        {
            ProjectItem pitem = (ProjectItem)item.Object;
            this.resources = new ArrayList();
            try
            {
                for( short i = 1; i <= pitem.FileCount; i++ ) 
                {
                    
                    ILocalResource res = SvnResource.FromLocalPath( pitem.get_FileNames(i) );
                    // does this resource exist?
                    if ( res != null )
                    {
                        res.Context = explorer.Context;
                        this.resources.Add( res );
                    }
                }
                explorer.AddResource( pitem, this );                    
            }
            catch( NullReferenceException )
            {
                //swallow
            }                
        }

        protected override StatusKind GetStatus()
        {
            // go through the resources belonging to this node
            foreach( ILocalResource resource in this.resources )
            {
                StatusKind status = StatusFromResource( resource );
                if ( status != StatusKind.Normal )
                    return status;
            }
                                
            return StatusKind.Normal;            
        }

        public override void VisitResources( ILocalResourceVisitor visitor )
        {
            foreach( ILocalResource resource in this.resources )
                resource.Accept( visitor );

            this.VisitChildren( visitor );
        }

        private IList resources;
    }    

}
