// $Id$
using System;
using System.Collections;
using NSvn;
using NSvn.Core;
using EnvDTE;
using System.Diagnostics;
using System.IO;

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

            this.UpdateStatus( false, false );
                        
        }

        public IList Resources
        {
            [System.Diagnostics.DebuggerStepThrough]
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

        /// <summary>
        /// The project item associated with this node.
        /// </summary>
        public ProjectItem ProjectItem
        {
            get{ return this.projectItem; }
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
                    string filename = this.projectItem.get_FileNames(i);
                    if ( File.Exists( filename ) || Directory.Exists( filename ) )
                    {
                        ILocalResource res = SvnResource.FromLocalPath( filename);
                        // does this resource exist?
                        res.Context = this.Explorer.Context;
                        this.resources.Add( res );
                    }                    
                }
                this.Explorer.AddResource( this.projectItem, this );                    
            }
            catch( NullReferenceException )
            {
                Debug.WriteLine( "NullReferenceException thrown in ProjectItemNode" );
                //swallow
            }   
            catch( System.Runtime.InteropServices.SEHException sex )
            {
                Debug.WriteLine( "SEHException thrown: " + this.projectItem.Name );
                System.Windows.Forms.MessageBox.Show( "SEHException: " + this.projectItem.Name + sex.Message );
            }
        }

        private ProjectItem projectItem;
        private IList resources;
    }    

}
