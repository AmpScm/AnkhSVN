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

        public override void GetResources( System.Collections.IList list, 
            bool getChildItems, ResourceFilterCallback filter )
        {
            if ( filter == null || filter( this.projectFolder ) )
                list.Add (this.projectFolder );
            if ( filter == null || filter( this.projectFile ) )
                list.Add( this.projectFile );

            this.GetChildResources( list, getChildItems, filter );
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

                // attach event handlers
                StatusChanged del = new StatusChanged( this.ChildOrResourceChanged );
                this.projectFolder.Changed += del;
                this.projectFolder.Changed += del;                                   
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
        protected override NodeStatus ThisNodeStatus()
        {            
            // check status on the project folder
            return this.MergeStatuses( this.projectFolder, this.projectFile );
        }                    

        private SvnItem projectFolder;
        private SvnItem projectFile;
        private Project project;
    }  

}
