using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using System.Collections;
using System.Diagnostics;

namespace Ankh.Solution
{
    enum VSITEMID
    {
        VSITEMID_NIL = -1,
        VSITEMID_ROOT = -2,
        VSITEMID_SELECTION = -3
    }

    class HierarchyWalker
    {
        public HierarchyWalker( IVsHierarchy hierarchy )
        {
            this.hierarchy = hierarchy;
            this.project = (IVsProject3)hierarchy;

            this.parentsStack = new Stack();
        }

        public void Walk(IHierarchyVisitor visitor)
        {
            uint root = unchecked( (uint)(int)VSITEMID.VSITEMID_ROOT );
            this.GetNodeAndChildren( root, visitor );
        }

        private void GetNodeAndChildren( uint itemID, IHierarchyVisitor visitor )
        {
            object retval;

            HierarchyItem currentItem = null;

            if ( itemID == unchecked( (uint)(int)VSITEMID.VSITEMID_ROOT ) )
            {
                HierarchyProject project = new HierarchyProject( itemID, this.hierarchy, null );
                visitor.VisitProject( project );
                currentItem = project;
            }
            else
            {
                HierarchyProjectItem item = this.GetHierarchyProjectItem( itemID );
                if ( item != null )
                {
                    visitor.VisitProjectItem( this.CurrentParent, item );
                    currentItem = item; 
                }
            }

            if ( currentItem != null )
            {
                this.parentsStack.Push( currentItem ); 
            }

            if ( this.hierarchy.GetProperty( itemID, (int)__VSHPROPID.VSHPROPID_FirstChild, out retval ) == VSConstants.S_OK )
            {
                uint child = (uint)(int)retval;
                if ( child != unchecked( (uint)(int)VSITEMID.VSITEMID_NIL ) )
                {
                    this.GetNodeAndChildren( child, visitor );
                    this.GetSiblings( child, visitor );
                }
            }

            if ( currentItem != null )
            {
                this.parentsStack.Pop(); 
            }
        }

        private HierarchyProjectItem GetHierarchyProjectItem( uint itemID )
        {

            IVsHierarchy hierarchy = this.project as IVsHierarchy;

            object retval;
            if ( hierarchy.GetProperty( itemID,
                (int)__VSHPROPID.VSHPROPID_ExtObject, out retval ) == VSConstants.S_OK )
            {
                ProjectItem item = retval as ProjectItem;

                if ( item != null )
                {
                    return new HierarchyProjectItem( itemID, item ); ;
                }
            }

            return null;
        }

        private void GetSiblings( uint itemID, IHierarchyVisitor visitor )
        {
            while ( true )
            {
                object retval;
                if ( this.hierarchy.GetProperty( itemID, (int)__VSHPROPID.VSHPROPID_NextSibling, out retval ) == VSConstants.S_OK )
                {
                    uint sibling = (uint)(int)retval;
                    if ( sibling != unchecked( (uint)(int)VSITEMID.VSITEMID_NIL ) )
                    {
                        this.GetNodeAndChildren( sibling, visitor );
                    }
                    else
                    {
                        break;
                    }

                    itemID = sibling;
                }
            }
        }

        private void DumpNodeName( uint itemID )
        {
            string name;
            object retval;
            this.hierarchy.GetProperty( itemID, (int)__VSHPROPID.VSHPROPID_Name, out retval );
            name = (string)retval;
            Debug.WriteLine( name );
        }

        private HierarchyItem CurrentParent
        {
            get { return this.parentsStack.Peek() as HierarchyItem; }
        }


        private IVsHierarchy hierarchy;
        private IVsProject3 project;
        private Stack parentsStack;
    }
}
