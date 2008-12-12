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

	DiffViewLines.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using System.Collections;
using Ankh.Diff.DiffUtils;
using System.Diagnostics;
using System.Collections.Generic;

namespace Ankh.Diff.DiffUtils.Controls
{
    public class DiffViewLines : ReadOnlyCollectionBase
    {
        #region Public Members

        public DiffViewLines(IList<string> StringList, EditScript Script, bool bUseA)
        {
            int iCurrentLine = 0;

            int iTotalEdits = Script.Count;
            m_arDiffStartLines = new int[iTotalEdits];
            m_arDiffEndLines = new int[iTotalEdits];

            for (int e = 0; e < iTotalEdits; e++)
            {
                Edit E = Script[e];

                //Get the starting line for this Edit
                int iStartingLine = -1;
                bool bDummyLine = false;
                if (bUseA)
                {
                    bDummyLine = (E.Type == EditType.Insert);
                    iStartingLine = E.StartA;
                }
                else
                {
                    bDummyLine = (E.Type == EditType.Delete);
                    iStartingLine = E.StartB;
                }

                //Put in unedited lines up to this point
                iCurrentLine += AddUneditedLines(StringList, iCurrentLine, iStartingLine);

                //Record where the next diff starts and ends
                m_arDiffStartLines[e] = Count;
                m_arDiffEndLines[e] = Count + E.Length - 1;

                //Since Inserts occur after the current line
                //instead of at it, we have to decrement the index.
                for (int i = 0; i < E.Length; i++)
                {
                    //A - Shows Deletes and Changes
                    //B - Shows Inserts and Changes
                    string strText = "";
                    int iNumber = -1;
                    if (!bDummyLine)
                    {
                        iNumber = iStartingLine + i;
                        strText = (string)StringList[iNumber];
                        iCurrentLine++;
                    }

                    AddLine(strText, iNumber, E.Type, true);
                }
            }

            //Put in any remaining unedited lines
            AddUneditedLines(StringList, iCurrentLine, StringList.Count);
        }

        public DiffViewLines(DiffViewLine LineOne, DiffViewLine LineTwo)
        {
            AddLine(LineOne);
            AddLine(LineTwo);

            m_arDiffStartLines = new int[0];
            m_arDiffEndLines = new int[0];
        }

        public DiffViewLine this[int iIndex]
        {
            get
            {
                return (DiffViewLine)InnerList[iIndex];
            }
        }

        public int LongestStringLength
        {
            get
            {
                return m_iLongestLength;
            }
        }

        //This should be called when DiffOptions.SpacesPerTab changes.
        public void RecheckLongestStringLength()
        {
            m_iLongestLength = 0;
            foreach (DiffViewLine L in InnerList)
            {
                CheckLongestLength(L.Text);
            }
        }

        public int[] DiffStartLines
        {
            get
            {
                return m_arDiffStartLines;
            }
        }

        public int[] DiffEndLines
        {
            get
            {
                return m_arDiffEndLines;
            }
        }

        public int MaxLineNumber
        {
            get
            {
                return m_iMaxLineNumber;
            }
        }

        #endregion

        #region Private Members

        private int AddUneditedLines(IList<string> StringList, int iCurrent, int iEnd)
        {
            for (int i = iCurrent; i < iEnd; i++)
            {
                AddLine((string)StringList[i], i, (EditType)0, false);
            }

            return iEnd - iCurrent;
        }

        private void AddLine(string strText, int iNumber, EditType eType, bool bEdited)
        {
            AddLine(new DiffViewLine(strText, iNumber, eType, bEdited));
        }

        private void AddLine(DiffViewLine Line)
        {
            CheckLongestLength(Line.Text);

            if (Line.Number >= 0 && Line.Number > m_iMaxLineNumber)
            {
                m_iMaxLineNumber = Line.Number;
            }

            InnerList.Add(Line);
        }

        private void CheckLongestLength(string strText)
        {
            int iTabStringLength = GetTabStringLength(strText);
            if (iTabStringLength > m_iLongestLength)
            {
                m_iLongestLength = iTabStringLength;
            }
        }

        private int GetTabStringLength(string strText)
        {
            int iLength = strText.Length;

            if (iLength > 0)
            {
                int iNumTabs = 0;
                for (int i = 0; i < iLength; i++)
                {
                    if (strText[i] == '\t')
                    {
                        iNumTabs++;
                    }
                }

                //We have to subtract 1 because the tab character has
                //already been counted once by String.Length.
                iLength += (iNumTabs * (DiffOptions.SpacesPerTab - 1));
            }

            return iLength;
        }

        private int m_iLongestLength = 0;
        private int[] m_arDiffStartLines;
        private int[] m_arDiffEndLines;
        private int m_iMaxLineNumber = 1;

        #endregion
    }

    public class DiffViewLine
    {
        #region Public Members

        public DiffViewLine(string strText, int iNumber, EditType Type, bool bEdited)
        {
            m_strText = strText;
            m_iNumber = iNumber;
            m_Type = Type;
            m_bEdited = bEdited;
        }

        public string Text { get { return m_strText; } }
        public int Number { get { return m_iNumber; } }
        public EditType EditType { get { return m_Type; } }
        public bool Edited { get { return m_bEdited; } }

        #endregion

        #region Private Members

        private int m_iNumber;
        private string m_strText;
        private EditType m_Type;
        private bool m_bEdited;

        #endregion
    }
}
