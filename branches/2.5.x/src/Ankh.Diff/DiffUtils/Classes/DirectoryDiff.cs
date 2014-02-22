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

	DirectoryDiff.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	11.16.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.IO;
using System.Collections;

namespace Ankh.Diff.DiffUtils
{
    public class DirectoryDiff
    {
        #region Public Members

        public DirectoryDiff(bool bShowOnlyInA, bool bShowOnlyInB, bool bShowDifferent, bool bShowSame, bool bRecursive, bool bIgnoreDirectoryComparison, DirectoryDiffFileFilter Filter)
        {
            m_bShowOnlyInA = bShowOnlyInA;
            m_bShowOnlyInB = bShowOnlyInB;
            m_bShowDifferent = bShowDifferent;
            m_bShowSame = bShowSame;
            m_bRecursive = bRecursive;
            m_bIgnoreDirectoryComparison = bIgnoreDirectoryComparison;
            m_Filter = Filter;
        }

        public DirectoryDiffResults Execute(string strA, string strB)
        {
            return Execute(new DirectoryInfo(strA), new DirectoryInfo(strB));
        }

        public DirectoryDiffResults Execute(DirectoryInfo A, DirectoryInfo B)
        {
            //Create a faux base entry to pass to Execute
            DirectoryDiffEntry Entry = new DirectoryDiffEntry("", false, true, true, false);

            //If the base paths are the same, we don't need to check for file differences.
            bool bCheckIfFilesAreDifferent = String.Compare(A.FullName, B.FullName, true) != 0;

            Execute(A, B, Entry, bCheckIfFilesAreDifferent);

            DirectoryDiffResults Results = new DirectoryDiffResults(A, B, Entry.SubEntries, m_bRecursive, m_Filter);
            return Results;
        }

        #endregion

        #region Private Methods

        private void Execute(DirectoryInfo A, DirectoryInfo B, DirectoryDiffEntry Entry, bool bCheckIfFilesAreDifferent)
        {
            //Get the arrays of files
            FileInfo[] arAFileInfos, arBFileInfos;
            if (m_Filter == null)
            {
                arAFileInfos = A.GetFiles();
                arBFileInfos = B.GetFiles();
                //Sort them
                Array.Sort(arAFileInfos, FileSystemInfoComparer.Comparer);
                Array.Sort(arBFileInfos, FileSystemInfoComparer.Comparer);
            }
            else
            {
                arAFileInfos = m_Filter.Filter(A);
                arBFileInfos = m_Filter.Filter(B);
            }

            //Diff them
            DiffFileSystemInfos(arAFileInfos, arBFileInfos, Entry, true, bCheckIfFilesAreDifferent);

            //Get the arrays of subdirectories
            DirectoryInfo[] arADirInfos = A.GetDirectories();
            DirectoryInfo[] arBDirInfos = B.GetDirectories();
            //Sort them
            Array.Sort(arADirInfos, FileSystemInfoComparer.Comparer);
            Array.Sort(arBDirInfos, FileSystemInfoComparer.Comparer);
            //Diff them
            DiffFileSystemInfos(arADirInfos, arBDirInfos, Entry, false, bCheckIfFilesAreDifferent);
        }

        private void DiffFileSystemInfos(FileSystemInfo[] arA, FileSystemInfo[] arB, DirectoryDiffEntry Entry, bool bIsFile, bool bCheckIfFilesAreDifferent)
        {
            int iAIndex = 0;
            int iBIndex = 0;
            int iNumA = arA.Length;
            int iNumB = arB.Length;
            while (iAIndex < iNumA && iBIndex < iNumB)
            {
                FileSystemInfo A = arA[iAIndex];
                FileSystemInfo B = arB[iBIndex];

                int iCompareResult = String.Compare(A.Name, B.Name, true);

                if (iCompareResult == 0)
                {
                    //The item is in both directories
                    if (m_bShowDifferent || m_bShowSame)
                    {
                        bool bDifferent = false;
                        DirectoryDiffEntry NewEntry = new DirectoryDiffEntry(A.Name, bIsFile, true, true, false);

                        if (bIsFile)
                        {
                            if (bCheckIfFilesAreDifferent)
                            {
                                try
                                {
                                    bDifferent = Functions.AreFilesDifferent((FileInfo)A, (FileInfo)B);
                                }
                                catch (Exception ex)
                                {
                                    NewEntry.Error = ex.Message;
                                }
                                NewEntry.Different = bDifferent;
                            }

                            if ((bDifferent && m_bShowDifferent) || (!bDifferent && m_bShowSame))
                            {
                                Entry.SubEntries.Add(NewEntry);
                            }
                        }
                        else
                        {
                            if (m_bRecursive)
                            {
                                Execute((DirectoryInfo)A, (DirectoryInfo)B, NewEntry, bCheckIfFilesAreDifferent);
                            }

                            if (m_bIgnoreDirectoryComparison)
                            {
                                NewEntry.Different = false;
                            }
                            else
                            {
                                bDifferent = NewEntry.Different;
                            }

                            if (m_bIgnoreDirectoryComparison || (bDifferent && m_bShowDifferent) || (!bDifferent && m_bShowSame))
                            {
                                Entry.SubEntries.Add(NewEntry);
                            }
                        }

                        if (bDifferent)
                        {
                            Entry.Different = true;
                        }
                    }
                    iAIndex++;
                    iBIndex++;
                }
                else if (iCompareResult < 0)
                {
                    //The item is only in A
                    if (m_bShowOnlyInA)
                    {
                        Entry.SubEntries.Add(new DirectoryDiffEntry(A.Name, bIsFile, true, false, false));
                        Entry.Different = true;
                    }
                    iAIndex++;
                }
                else //iCompareResult > 0
                {
                    //The item is only in B
                    if (m_bShowOnlyInB)
                    {
                        Entry.SubEntries.Add(new DirectoryDiffEntry(B.Name, bIsFile, false, true, false));
                        Entry.Different = true;
                    }
                    iBIndex++;
                }
            }

            //Add any remaining entries
            if (iAIndex < iNumA && m_bShowOnlyInA)
            {
                for (int i = iAIndex; i < iNumA; i++)
                {
                    Entry.SubEntries.Add(new DirectoryDiffEntry(arA[i].Name, bIsFile, true, false, false));
                    Entry.Different = true;
                }
            }
            else if (iBIndex < iNumB && m_bShowOnlyInB)
            {
                for (int i = iBIndex; i < iNumB; i++)
                {
                    Entry.SubEntries.Add(new DirectoryDiffEntry(arB[i].Name, bIsFile, false, true, false));
                    Entry.Different = true;
                }
            }
        }

        #endregion

        #region Private Data Members

        private bool m_bShowOnlyInA;
        private bool m_bShowOnlyInB;
        private bool m_bShowDifferent;
        private bool m_bShowSame;
        private bool m_bRecursive;
        private bool m_bIgnoreDirectoryComparison;
        private DirectoryDiffFileFilter m_Filter;

        #endregion
    }
}
