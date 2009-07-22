// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

#region Copyright And Revision History

/*---------------------------------------------------------------------------

	DirDiffTreeView.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	11.16.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.IO;
using System.Windows.Forms;
using Ankh.Diff.DiffUtils;
using System.Diagnostics;
using Ankh.Diff;

/*
 * Expose V and H scroll pos
 * SetData to bind 
 * Set default property values
 */
namespace Ankh.Diff.DiffUtils.Controls
{
    public class DirDiffTreeView : TreeView
    {
        #region Public Members

        public DirDiffTreeView()
        {
            HideSelection = false;
            ShowLines = false;
            FullRowSelect = true;

            m_OptionsChangedHandler = new EventHandler(DiffOptionsChanged);
            DiffOptions.OptionsChanged += m_OptionsChangedHandler;
        }

        public void SetData(DirectoryDiffResults Results, bool bUseA)
        {
            m_Results = Results;
            m_bUseA = bUseA;
            PopulateTree();
        }

        public DirectoryDiffEntry GetEntryForNode(TreeNode Node)
        {
            if (Node != null)
                return Node.Tag as DirectoryDiffEntry;
            else
                return null;
        }

        public void SendVScroll(IntPtr WParam)
        {
            if (!m_bSendingVScroll)
            {
                try
                {
                    m_bSendingVScroll = true;
                    NativeMethods.SendMessage(this, NativeMethods.WM_VSCROLL, (int)WParam, 0);
                }
                finally
                {
                    m_bSendingVScroll = false;
                }
            }
        }

        public void SendKeyDown(Keys KeyCode, Keys KeyData)
        {
            if (!m_bSendingKeyDown)
            {
                try
                {
                    m_bSendingKeyDown = true;
                    NativeMethods.SendMessage(this, NativeMethods.WM_KEYDOWN, (int)KeyCode, (int)KeyData);
                }
                finally
                {
                    m_bSendingKeyDown = false;
                }
            }
        }

        public void SendMouseWheel(IntPtr WParam, IntPtr LParam)
        {
            if (!m_bSendingMouseWheel)
            {
                try
                {
                    m_bSendingMouseWheel = true;
                    NativeMethods.SendMessage(this, NativeMethods.WM_MOUSEWHEEL, (int)WParam, (int)LParam);
                }
                finally
                {
                    m_bSendingMouseWheel = false;
                }
            }
        }

        public string GetFullNameForNode(TreeNode Node)
        {
            string strNodePath = Node.FullPath;
            string strBasePath = m_bUseA ? m_Results.A.FullName : m_Results.B.FullName;
            if (!strBasePath.EndsWith("\\"))
            {
                strBasePath += "\\";
            }
            return strBasePath + strNodePath;
        }

        public bool CanView
        {
            get
            {
                TreeNode Node = SelectedNode;
                if (Node != null)
                {
                    DirectoryDiffEntry Entry = GetEntryForNode(Node);
                    if (Entry != null)
                    {
                        return (m_bUseA && Entry.InA) || (!m_bUseA && Entry.InB);
                    }
                }

                return false;
            }
        }

        public void View()
        {
            if (!CanView) return;

            TreeNode Node = SelectedNode;
            string strFullName = GetFullNameForNode(Node);

            DirectoryDiffEntry Entry = GetEntryForNode(Node);
            string strVerb = "open";
            if (!Entry.IsFile)
            {
                string strDefaultVerb = Utilities.DefaultFolderOpenAction;
                if (strDefaultVerb.Length > 0)
                {
                    strVerb = strDefaultVerb;
                }
            }

            Utilities.ShellExecute(this, strFullName, strVerb);
        }

        #endregion

        #region Public Events

        public event EventHandler<Win32MessageEventArgs> VScroll;
        public event EventHandler<Win32MessageEventArgs> MouseWheelMsg;

        #endregion

        #region Protected Methods

        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            SetNodeImage(e.Node, GetEntryForNode(e.Node));
            base.OnAfterCollapse(e);
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            SetNodeImage(e.Node, GetEntryForNode(e.Node));
            base.OnAfterExpand(e);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == NativeMethods.WM_VSCROLL && !m_bSendingVScroll && VScroll != null)
            {
                Win32MessageEventArgs e = new Win32MessageEventArgs(m.WParam, m.LParam);
                VScroll(this, e);
            }
            else if (m.Msg == NativeMethods.WM_MOUSEWHEEL && !m_bSendingMouseWheel && MouseWheelMsg != null)
            {
                Win32MessageEventArgs e = new Win32MessageEventArgs(m.WParam, m.LParam);
                MouseWheelMsg(this, e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
            {
                m_bSendingKeyDown = true;
                base.OnKeyDown(e);
            }
            finally
            {
                m_bSendingKeyDown = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DiffOptions.OptionsChanged -= m_OptionsChangedHandler;
            }
            base.Dispose(disposing);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            TreeNode Node = GetNodeAt(e.X, e.Y);
            if (Node != null)
            {
                SelectedNode = Node;
            }
        }

        #endregion

        #region Private Methods

        private void PopulateTree()
        {
            BeginUpdate();
            try
            {
                Nodes.Clear();

                foreach (DirectoryDiffEntry Entry in m_Results.Entries)
                {
                    AddEntry(Entry, null);
                }

                ExpandAll();
            }
            finally
            {
                EndUpdate();
            }
        }

        private void AddEntry(DirectoryDiffEntry Entry, TreeNode ParentNode)
        {
            TreeNode Node = new TreeNode();
            Node.Tag = Entry;

            if (m_bUseA)
            {
                Entry.NodeA = Node;
            }
            else
            {
                Entry.NodeB = Node;
            }
            Node.Expand();

            SetNodeText(Node, Entry);
            SetNodeImage(Node, Entry);
            SetNodeColor(Node, Entry);

            if (ParentNode != null)
            {
                ParentNode.Nodes.Add(Node);
            }
            else
            {
                Nodes.Add(Node);
            }

            if (!Entry.IsFile)
            {
                foreach (DirectoryDiffEntry SubEntry in Entry.SubEntries)
                {
                    AddEntry(SubEntry, Node);
                }
            }
        }

        private void SetNodeText(TreeNode Node, DirectoryDiffEntry Entry)
        {
            if ((Entry.InA && Entry.InB) || (m_bUseA && Entry.InA) || (!m_bUseA && Entry.InB))
            {
                if (Entry.Error == null)
                {
                    Node.Text = Entry.Name;
                }
                else
                {
                    Node.Text = String.Format("{0}: {1}", Entry.Name, Entry.Error);
                }
            }
        }

        private void DiffOptionsChanged(object sender, EventArgs e)
        {
            SetNodesColors(Nodes);
        }

        private void SetNodesColors(TreeNodeCollection Nodes)
        {
            foreach (TreeNode Node in Nodes)
            {
                SetNodeColor(Node, GetEntryForNode(Node));
                SetNodesColors(Node.Nodes);
            }
        }

        private void SetNodeColor(TreeNode Node, DirectoryDiffEntry Entry)
        {
            if (Entry.InA && Entry.InB)
            {
                if (Entry.Different)
                {
                    Node.BackColor = DiffOptions.ChangedColor;
                }
                else
                {
                    Node.BackColor = BackColor;
                }
            }
            else
            {
                if (Entry.InA)
                {
                    Node.BackColor = DiffOptions.DeletedColor;
                }
                else
                {
                    Node.BackColor = DiffOptions.InsertedColor;
                }
            }
        }

        private void SetNodeImage(TreeNode Node, DirectoryDiffEntry Entry)
        {
            bool bPresent = (m_bUseA && Entry.InA) || (!m_bUseA && Entry.InB);
            int iIndex = -1;

            if (Entry.Error != null)
            {
                iIndex = c_iFileError;
            }
            else if (Entry.IsFile)
            {
                if (bPresent)
                {
                    iIndex = c_iFile;
                }
                else
                {
                    iIndex = c_iFileMissing;
                }
            }
            else
            {
                if (bPresent)
                {
                    //If a folder is only present on one side, then 
                    //we should always show it closed since we haven't
                    //actually recursed into it.
                    //
                    //Also, we should only show a folder open if we're
                    //showing recursive differences.
                    if (m_Results.Recursive && Node.IsExpanded && Entry.InA && Entry.InB)
                    {
                        iIndex = c_iFolderOpen;
                    }
                    else
                    {
                        iIndex = c_iFolderClosed;
                    }
                }
                else
                {
                    iIndex = c_iFolderMissing;
                }
            }

            Node.ImageIndex = iIndex;
            Node.SelectedImageIndex = iIndex;
        }

        #endregion

        #region Private Data Members

        private DirectoryDiffResults m_Results;
        private bool m_bUseA;
        private bool m_bSendingVScroll;
        private bool m_bSendingKeyDown;
        private bool m_bSendingMouseWheel;
        private EventHandler m_OptionsChangedHandler;

        #endregion

        #region Private Constants

        private const int c_iFolderClosed = 0;
        private const int c_iFolderOpen = 1;
        private const int c_iFolderMissing = 2;
        private const int c_iFile = 3;
        private const int c_iFileMissing = 4;
        private const int c_iFileError = 5;

        #endregion
    }

    #region Helper Types

    public sealed class Win32MessageEventArgs : EventArgs
    {
        internal Win32MessageEventArgs(IntPtr WParam, IntPtr LParam)
        {
            m_WParam = WParam;
            m_LParam = LParam;
        }

        public IntPtr WParam { get { return m_WParam; } }
        public IntPtr LParam { get { return m_LParam; } }

        private IntPtr m_WParam;
        private IntPtr m_LParam;
    }

    #endregion
}
