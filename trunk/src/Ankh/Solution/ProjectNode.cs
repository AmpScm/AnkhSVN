using System;
using NSvn;
using NSvn.Core;
using EnvDTE;
using System.IO;

namespace Ankh.Solution
{
    internal class ProjectNode : TreeNode
    {
        public ProjectNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer,
            TreeNode parent ) : 
            base( item, hItem, explorer, parent )
        {
            Project project = (Project)item.Object;

            // find the directory containing the project
            string fullname = project.FullName;
            // the Solution Items project has no path
            if ( fullname != string.Empty )
            {
                string parentPath = Path.GetDirectoryName( fullname );
                this.projectFolder = (WorkingCopyDirectory)SvnResource.FromLocalPath( parentPath );
                explorer.AddResource( project, this );                    
            }
            if ( this.projectFolder != null )
                this.projectFolder.Context = explorer.Context;
        }

        public WorkingCopyDirectory ProjectFolder
        {
            get{ return this.projectFolder; }
        }

        /// <summary>
        /// Accept an INodeVisitor.
        /// </summary>
        /// <param name="visitor"></param>
        public override void Accept( INodeVisitor visitor )
        {
            visitor.VisitProject( this );
        }

        

        public override void VisitResources( ILocalResourceVisitor visitor, bool recursive )
        {
            if ( this.projectFolder != null )
                this.projectFolder.Accept( visitor );
            if ( recursive )
                this.VisitChildResources( visitor );
        } 
            
        protected override StatusKind GetStatus()
        {
            if ( this.projectFolder == null )
                return StatusKind.None;               
            else
            {
                // check status on the project folder
                StatusKind folderStatus = StatusFromResource( this.projectFolder );
                if (  folderStatus != StatusKind.Normal )
                    return folderStatus;
                else
                {
                    // check statuses on child resources
                    ModifiedVisitor v = new ModifiedVisitor();
                    this.VisitChildResources( v );
                    return v.Modified ? StatusKind.Modified : StatusKind.Normal;
                }
            }
        }                    

        private WorkingCopyDirectory projectFolder;
    }  

}
