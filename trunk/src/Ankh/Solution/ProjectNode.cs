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
                this.projectFolder = SvnResource.FromLocalPath( parentPath );
                explorer.AddResource( project, this );                    
            }
            if ( this.projectFolder != null )
                this.projectFolder.Context = explorer.Context;
        }

        public override void VisitResources( ILocalResourceVisitor visitor )
        {
            if ( this.projectFolder != null )
                this.projectFolder.Accept( visitor );
            this.VisitChildren( visitor );
        } 
            
        protected override StatusKind GetStatus()
        {
            if ( this.projectFolder == null )
                return StatusKind.None;               
            else
                return StatusFromResource( this.projectFolder );
        }                    

        private ILocalResource projectFolder;
    }  

}
