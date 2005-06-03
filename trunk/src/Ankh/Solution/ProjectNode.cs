// $Id$
using System;

using NSvn.Core;
using EnvDTE;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace Ankh.Solution
{
    public class ProjectNode : TreeNode
    {
        public ProjectNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer,
            TreeNode parent ) : 
            base( item, hItem, explorer, parent )
        {
            this.project = (Project)item.Object;

            this.FindProjectResources(explorer);
        }

        public override void GetResources( System.Collections.IList list, 
            bool getChildItems, ResourceFilterCallback filter )
        {
            if ( filter == null || filter( this.projectFolder ) )
                list.Add (this.projectFolder );
            if ( filter == null || filter( this.projectFile ) )
                list.Add( this.projectFile );

            // add deleted items.
            foreach( SvnItem item in this.additionalResources )
            {
                if ( filter == null || filter( item ) )
                    list.Add( item );
            }

            this.GetChildResources( list, getChildItems, filter );
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.VisitProject(this);
        }

        protected override void RescanHook()
        {
            this.additionalResources.Clear();
            this.AddDeletions( this.projectFolder.Path, this.additionalResources, 
                new StatusChanged(this.ChildOrResourceChanged) );
        }




        private void FindProjectResources(Explorer explorer)
        {
            // find the directory containing the project
            string fullname = project.FullName;

            this.additionalResources = new ArrayList();

            // special treatment for VDs
            if (String.Compare(project.Kind, ProjectNode.VDPROJKIND, true) == 0)
                fullname += ".vdproj";

            StatusChanged del = new StatusChanged(this.ChildOrResourceChanged);
            // the Solution Items project has no path
            if (fullname != string.Empty && File.Exists(fullname))
            {
                string parentPath = Path.GetDirectoryName(fullname);
                this.projectFolder = this.Explorer.Context.StatusCache[parentPath];
                this.projectFile = this.Explorer.Context.StatusCache[fullname];

                this.Explorer.AddResource(project, this, fullname);

                // attach event handlers                
                this.projectFolder.Changed += del;
                this.projectFile.Changed += del;

                // we also want deleted items in this folder
                this.AddDeletions(this.projectFolder.Path,
                    this.additionalResources, del);
            }
            // web projects in VS 2005 have no project files
            else if (System.IO.Directory.Exists(fullname))
            {
                this.projectFolder = this.Explorer.Context.StatusCache[fullname];
                this.projectFile = SvnItem.Unversionable;
                this.Explorer.AddResource(project, this, fullname);

                this.projectFolder.Changed += del;
                this.AddDeletions(this.projectFolder.Path, this.additionalResources, del);
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
        public override string Directory
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
            return this.MergeStatuses(
                this.MergeStatuses(this.projectFolder, this.projectFile),
                this.MergeStatuses(this.additionalResources));

               
        }                    

        private SvnItem projectFolder;
        private SvnItem projectFile;
        private IList additionalResources;
        private Project project;

        private const string VDPROJKIND = @"{54435603-DBB4-11D2-8724-00A0C9A8B90C}";

    }  

}
