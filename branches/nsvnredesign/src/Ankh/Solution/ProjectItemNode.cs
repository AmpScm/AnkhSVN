// $Id$
using System;
using System.Collections;

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

            this.FindChildren();                        
        }

        /// <summary>
        /// The status of this node, not including children.
        /// </summary>
        /// <returns></returns>
        protected override StatusKind NodeStatus()
        {
            // any resources at all?
            if ( this.resources.Count == 0 )
                return StatusKind.None;

            // go through the resources belonging to this node
            foreach( SvnItem item in this.resources )
            {
                StatusKind status = GenerateStatus(item.Status);
                if ( status != StatusKind.Normal )
                    return status;
            }                   
            return StatusKind.Normal;            
        }

        /// <summary>
        /// The folder path if this is a folder item, or the directory
        /// in which the file resides if it is a file.
        /// </summary>
        protected override string Directory
        {
            get
            {
                // is one of the resources a directory?
                foreach( SvnItem item in this.resources )
                {
                    if ( System.IO.Directory.Exists( item.Path ) )
                        return item.Path;
                }

                // just return the dir component of the first item
                return Path.GetDirectoryName( ((SvnItem)this.resources[0]).Path );
            }
        }


        protected void FindResources()
        {
            this.resources = new ArrayList();
            try
            {
                for( short i = 1; i <= this.projectItem.FileCount; i++ ) 
                {
                    string path = this.projectItem.get_FileNames(i);
                    if ( File.Exists( path ) || System.IO.Directory.Exists( path ) )
                    {
                        SvnItem item = this.Explorer.StatusCache[path];
                        this.resources.Add( item );
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
