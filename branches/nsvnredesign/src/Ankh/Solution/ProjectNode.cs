// $Id$
using System;

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
            this.project = (Project)item.Object;

            this.FindProjectResources(explorer);

            this.FindChildren();
        }

        private void FindProjectResources(Explorer explorer)
        {
            // find the directory containing the project
            string fullname = project.FullName;
            // the Solution Items project has no path
            if ( fullname != string.Empty && File.Exists( fullname ) )
            {
                string parentPath = Path.GetDirectoryName( fullname );
                this.projectFolder = this.Explorer.StatusCache[ parentPath ];
                this.projectFile = this.Explorer.StatusCache[ fullname ];

                this.Explorer.AddResource( project, this );                    
            }
            else
            {
                this.projectFile = SvnItem.Unversionable;
                this.projectFolder = SvnItem.Unversionable;
            }
        }

        /// <summary>
        /// The directory this project resides in.
        /// </summary>
        protected override string Directory
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return this.projectFolder.Path; }
        }


        /// <summary>
        /// The status of this node, not including children.
        /// </summary>
        /// <returns></returns>
        protected override StatusKind NodeStatus()
        {            
            // check status on the project folder
            StatusKind folderStatus = this.GenerateStatus(this.projectFolder.Status);
            StatusKind fileStatus = this.GenerateStatus(this.projectFile.Status);
            if ( fileStatus != StatusKind.Normal )
                return fileStatus;
            else if (  folderStatus != StatusKind.Normal )
                return folderStatus;
            else
                return StatusKind.Normal;
        }                    

        private SvnItem projectFolder;
        private SvnItem projectFile;
        private Project project;
    }  

}
