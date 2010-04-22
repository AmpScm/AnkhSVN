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

	DirectoryDiffFileFilter.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	11.26.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.IO;
using System.Collections;

namespace Ankh.Diff.DiffUtils
{
    public class DirectoryDiffFileFilter
    {
        public DirectoryDiffFileFilter(string strFilter, bool bInclude)
        {
            m_strFilter = strFilter;
            m_bInclude = bInclude;
            m_arFilters = strFilter.Split(';');
            for (int i = 0; i < m_arFilters.Length; i++)
            {
                m_arFilters[i] = m_arFilters[i].Trim();
            }
        }

        public bool Include
        {
            get
            {
                return m_bInclude;
            }
        }

        public string FilterString
        {
            get
            {
                return m_strFilter;
            }
        }

        public FileInfo[] Filter(DirectoryInfo Dir)
        {
            //Get all the files that match the filters
            ArrayList arFiles = new ArrayList();
            foreach (string strFilter in m_arFilters)
            {
                FileInfo[] arFilterFiles = Dir.GetFiles(strFilter);
                arFiles.AddRange(arFilterFiles);
            }

            //Sort them
            arFiles.Sort(FileSystemInfoComparer.Comparer);

            //Throw out duplicates
            FileInfo PreviousFile = null;
            for (int i = 0; i < arFiles.Count; /*Incremented in the loop*/)
            {
                FileInfo CurrentFile = (FileInfo)arFiles[i];
                if (PreviousFile != null && FileSystemInfoComparer.Comparer.Compare(CurrentFile, PreviousFile) == 0)
                {
                    arFiles.RemoveAt(i);
                    //Don't increment i;
                }
                else
                {
                    PreviousFile = CurrentFile;
                    i++;
                }
            }

            //Exclude these files if necessary
            if (m_bInclude)
            {
                return (FileInfo[])arFiles.ToArray(typeof(FileInfo));
            }
            else
            {
                FileInfo[] arAllFiles = Dir.GetFiles();
                Array.Sort(arAllFiles, FileSystemInfoComparer.Comparer);

                ArrayList arFilesToInclude = new ArrayList();
                int iNumExcludes = arFiles.Count;
                int iNumTotal = arAllFiles.Length;
                int e = 0;
                for (int a = 0; a < iNumTotal; a++)
                {
                    int iCompareResult = -1;
                    FileInfo A = arAllFiles[a];
                    if (e < iNumExcludes)
                    {
                        FileInfo E = (FileInfo)arFiles[e];
                        iCompareResult = FileSystemInfoComparer.Comparer.Compare(A, E);
                    }

                    if (iCompareResult == 0)
                    {
                        //Don't put this match in the results.
                        e++;
                    }
                    else
                    {
                        arFilesToInclude.Add(A);
                    }
                }

                return (FileInfo[])arFilesToInclude.ToArray(typeof(FileInfo));
            }
        }

        private string m_strFilter;
        private string[] m_arFilters;
        private bool m_bInclude;
    }
}
