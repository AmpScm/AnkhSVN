// $Id$
using System;
using System.Collections;
using NSvn;
using NSvn.Core;
using EnvDTE;
using System.Diagnostics;

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
            this.projectItem = (ProjectItem)item.Object;
            this.FindResources();
                        
        }

        public IList Resources
        {
            get{ return this.resources; }
        }

        /// <summary>
        /// Accept an INodeVisitor.
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept( INodeVisitor visitor )
        {
            visitor.VisitProjectItem( this );
        }

        public override void Refresh()
        {
            this.FindResources();
            base.Refresh();
        }
 

        protected override StatusKind GetStatus()
        {
            // any resources at all?
            if ( this.resources.Count == 0 )
                return StatusKind.None;

            // go through the resources belonging to this node
            foreach( ILocalResource resource in this.resources )
            {
                StatusKind status = StatusFromResource( resource );
                if ( status != StatusKind.Normal )
                    return status;
            }

            // ok - now check the children
            ModifiedVisitor v = new ModifiedVisitor();
            this.VisitChildResources( v );
            if ( v.Modified )
                return StatusKind.Modified;
                                
            return StatusKind.Normal;            
        }

        public override void VisitResources( ILocalResourceVisitor visitor, bool recursive )
        {
            if ( recursive )
                this.VisitChildResources( visitor );

            foreach( ILocalResource resource in this.resources )
                resource.Accept( visitor );
        }

        protected void FindResources()
        {
            this.resources = new ArrayList();
            try
            {
                for( short i = 1; i <= this.projectItem.FileCount; i++ ) 
                {
                    ILocalResource res = SvnResource.FromLocalPath( this.projectItem.get_FileNames(i) );
                    // does this resource exist?
                    res.Context = this.Explorer.Context;
                    this.resources.Add( res );
                }
                this.Explorer.AddResource( this.projectItem, this );                    
            }
            catch( NullReferenceException )
            {
                Debug.WriteLine( "NullReferenceException thrown in ProjectItemNode" );
                //swallow
            }   
            catch( System.Runtime.InteropServices.SEHException )
            {
                Debug.WriteLine( "SEHException thrown: " + this.projectItem.Name );
                System.Windows.Forms.MessageBox.Show( "SEHException: " + this.projectItem.Name );
            }
        }

        private ProjectItem projectItem;
        private IList resources;
    }    

}
