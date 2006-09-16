// $Id$
using System;

using NSvn.Core;
using EnvDTE;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace Ankh.Solution
{
    public class ProjectNode : SolutionExplorerTreeNode
    {
        public ProjectNode( UIHierarchyItem item, IntPtr hItem, Explorer explorer,
            SolutionExplorerTreeNode parent, Project project ) : 
            base( item, hItem, explorer, parent )
        {
            this.project = project;
            this.modeled=true;

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

            // add deleted items.
            foreach( SvnItem item in this.deletedResources )
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
            this.deletedResources.Clear();
            this.AddDeletions( this.projectFolder.Path, this.deletedResources,
                new EventHandler( this.ChildOrResourceChanged ) );
        }

        protected override void CheckForSvnDeletions()
        {
            // if the project folder is deleted, make sure the children are as well.
            if ( this.projectFolder.IsDeleted )
            {
                this.SvnDelete();
            }
        }

        public override void Refresh(bool rescan)
        {
            if (rescan)
            {
                if (this.Solution != null)
                    this.Solution.Parser.Refresh();
            }
            base.Refresh(rescan);
        }

        protected override bool RemoveTreeNodeIfResourcesDeleted()
        {
            // VS will never delete projects.
            return false;
        }

        protected override void DoDispose()
        {
            UnhookEvents( new SvnItem[] { this.projectFile, this.projectFolder }, new EventHandler( this.ChildOrResourceChanged ) );
            UnhookEvents( this.deletedResources, new EventHandler(this.ChildOrResourceChanged) );
        }

        private void FindProjectResources(Explorer explorer)
        {
            // find the directory containing the project
            string fullname = null;
            try
            {
                fullname = this.project.FullName;
            }
            catch(NotImplementedException)  //deal with unmodeled projects (database)
            {
                fullname=((SolutionNode)this.Parent).Parser.GetProjectFile(project.Name);
                this.modeled=false;
            }

            this.deletedResources = new ArrayList();

            // special treatment for VDs
            if (String.Compare(this.project.Kind, ProjectNode.VDPROJKIND, true) == 0)
                fullname += ".vdproj";

            EventHandler del = new EventHandler(this.ChildOrResourceChanged);
            // the Solution Items project has no path
            if (fullname != string.Empty && File.Exists(fullname))
            {
                string parentPath = Path.GetDirectoryName(fullname);
                this.projectFolder = this.Explorer.Context.StatusCache[parentPath];
                this.projectFile = this.Explorer.Context.StatusCache[fullname];

                this.Explorer.AddResource(this.project, this, fullname);
                this.Explorer.AddResource(this.uiItem.Object, this, fullname);

                // attach event handlers                
                this.projectFolder.Changed += del;
                this.projectFile.Changed += del;

                // we also want deleted items in this folder
                this.AddDeletions( this.projectFolder.Path,
                    this.deletedResources, new EventHandler(this.DeletedItemStatusChanged) );

            }
            // web projects in VS 2005 have no project files
            else if (System.IO.Directory.Exists(fullname))
            {
                this.projectFolder = this.Explorer.Context.StatusCache[fullname];
                this.projectFile = SvnItem.Unversionable;
                this.Explorer.AddResource(project, this, fullname);
                this.Explorer.AddResource(this.uiItem.Object, this, fullname);

                this.projectFolder.Changed += del;
                this.AddDeletions( this.projectFolder.Path, this.deletedResources, new EventHandler(this.DeletedItemStatusChanged) );
            }
            else
            {
                this.projectFile = SvnItem.Unversionable;
                this.projectFolder = SvnItem.Unversionable;

                this.Explorer.AddResource(this.project, this, fullname);
                this.Explorer.AddResource(this.uiItem.Object, this, fullname);

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
        /// True if this project is supported via Visual Studio Automation
        /// </summary>
        public bool Modeled
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get{ return this.modeled; }
        }

        /// <summary>
        /// Name of the project
        /// </summary>
        public string Name
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get{ return this.project.Name; }
        }

        /// <summary>
        /// The Visual Studio Automation project object
        /// </summary>
        public Project Project
        {
            [System.Diagnostics.DebuggerStepThrough()]
            get { return this.project; }
        }

        /// <summary>
        /// The status of this node, not including children.
        /// </summary>
        /// <returns></returns>
        protected override NodeStatus ThisNodeStatus()
        {
            // check status on the project folder
            return MergeStatuses(
                MergeStatuses(this.projectFolder, this.projectFile),
                MergeStatuses(this.deletedResources));

        }

        protected override IList DeletedItems
        {
            get { return this.deletedResources; }
        }

        private bool modeled;
        private SvnItem projectFolder;
        private SvnItem projectFile;
        private IList deletedResources;
        private Project project;

        private const string VDPROJKIND = @"{54435603-DBB4-11D2-8724-00A0C9A8B90C}";

    }  

}
