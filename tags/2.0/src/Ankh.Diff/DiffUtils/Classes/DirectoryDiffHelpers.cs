#region Copyright And Revision History

/*---------------------------------------------------------------------------

	DirectoryDiffHelpers.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	11.16.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace Ankh.Diff.DiffUtils
{
    public class DirectoryDiffEntry
    {
        private string m_strName;
        private bool m_bIsFile;
        private bool m_bInA;
        private bool m_bInB;
        private bool m_bDifferent;
        private DirectoryDiffEntries m_SubEntries;
        private string m_strError = null;
        //These members are strictly for DirDiffTreeView use.
        private TreeNode m_NodeA;
        private TreeNode m_NodeB;

        public DirectoryDiffEntry(string strName, bool bIsFile, bool bInA, bool bInB, bool bDifferent)
        {
            m_strName = strName;
            m_bIsFile = bIsFile;
            m_bInA = bInA;
            m_bInB = bInB;
            m_bDifferent = bDifferent;
        }

        public string Name { get { return m_strName; } }
        public bool IsFile { get { return m_bIsFile; } }
        public bool InA { get { return m_bInA; } }
        public bool InB { get { return m_bInB; } }
        public bool Different { get { return m_bDifferent; } set { m_bDifferent = value; } }
        public string Error { get { return m_strError; } set { m_strError = value; } }
        internal TreeNode NodeA { get { return m_NodeA; } set { m_NodeA = value; } }
        internal TreeNode NodeB { get { return m_NodeB; } set { m_NodeB = value; } }

        public DirectoryDiffEntries SubEntries
        {
            get
            {
                if (m_SubEntries == null && !m_bIsFile)
                {
                    m_SubEntries = new DirectoryDiffEntries();
                }

                return m_SubEntries;
            }
            set
            {
                m_SubEntries = value;
            }
        }
    }

    public class DirectoryDiffEntries : ReadOnlyCollectionBase
    {
        internal int Add(DirectoryDiffEntry E)
        {
            return InnerList.Add(E);
        }

        public DirectoryDiffEntry this[int iIndex]
        {
            get
            {
                return (DirectoryDiffEntry)InnerList[iIndex];
            }
            set
            {
                InnerList[iIndex] = value;
            }
        }
    }

    public class DirectoryDiffResults
    {
        public DirectoryDiffResults(DirectoryInfo A, DirectoryInfo B, DirectoryDiffEntries Entries, bool bRecursive, DirectoryDiffFileFilter Filter)
        {
            m_A = A;
            m_B = B;
            m_Entries = Entries;
            m_bRecursive = bRecursive;
            m_Filter = Filter;
        }

        public DirectoryInfo A { get { return m_A; } }
        public DirectoryInfo B { get { return m_B; } }
        public DirectoryDiffEntries Entries { get { return m_Entries; } }
        public bool Recursive { get { return m_bRecursive; } }
        public DirectoryDiffFileFilter Filter { get { return m_Filter; } }

        private DirectoryInfo m_A;
        private DirectoryInfo m_B;
        private DirectoryDiffEntries m_Entries;
        private bool m_bRecursive;
        private DirectoryDiffFileFilter m_Filter;
    }

    public class FileSystemInfoComparer : IComparer
    {
        public int Compare(object X, object Y)
        {
            FileSystemInfo fsX = (FileSystemInfo)X;
            FileSystemInfo fsY = (FileSystemInfo)Y;

            return String.Compare(fsX.Name, fsY.Name, true);
        }

        public readonly static FileSystemInfoComparer Comparer = new FileSystemInfoComparer();
    }

    public sealed class DifferenceEventArgs : EventArgs
    {
        internal DifferenceEventArgs(string strA, string strB)
        {
            m_strA = strA;
            m_strB = strB;
        }

        public string A { get { return m_strA; } }
        public string B { get { return m_strB; } }

        private string m_strA;
        private string m_strB;
    }

    public delegate void DifferenceEventHandler(object sender, DifferenceEventArgs e);
}
