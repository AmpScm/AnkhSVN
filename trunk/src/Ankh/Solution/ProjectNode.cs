// $Id$
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
                this.projectFile = SvnResource.FromLocalPath( fullname );

                explorer.AddResource( project, this );                    
            }
            else
            {
                this.projectFile = SvnResource.Unversionable;
                this.projectFolder = SvnResource.Unversionable;
            }

            this.projectFile.Context = explorer.Context;
            this.projectFolder.Context = explorer.Context;
        }

        /// <summary>
        /// The folder this project is contained in.
        /// </summary>
        public ILocalResource ProjectFolder
        {
            get{ return this.projectFolder; }
        }

        /// <summary>
        /// The project file itself.
        /// </summary>
        public ILocalResource ProjectFile
        {
            get{ return this.projectFile; }
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
            this.projectFolder.Accept( visitor );
            this.projectFile.Accept( visitor );
            if ( recursive )
                this.VisitChildResources( visitor );
        } 
            
        protected override StatusKind GetStatus()
        {
            
            // check status on the project folder
            StatusKind folderStatus = StatusFromResource( this.projectFolder );
            StatusKind fileStatus = StatusFromResource( this.projectFile );
            if ( fileStatus != StatusKind.Normal )
                return fileStatus;
            else if (  folderStatus != StatusKind.Normal )
                return folderStatus;
            else
            {
                // check statuses on child resources
                ModifiedVisitor v = new ModifiedVisitor();
                this.VisitChildResources( v );
                return v.Modified ? StatusKind.Modified : StatusKind.Normal;
            }
        }                    

        private ILocalResource projectFolder;
        private ILocalResource projectFile;
    }  

}
