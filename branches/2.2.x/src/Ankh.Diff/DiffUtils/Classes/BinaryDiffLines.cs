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

	BinaryDiffLines.cs
	Copyright (c) 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	11.2.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace Ankh.Diff.DiffUtils
{
    /// <summary>
    /// This class provides a way to get a set of text lines that
    /// can be diffed to display the difference between two binary
    /// files.
    /// </summary>
    public class BinaryDiffLines
    {
        #region Constructors

        public BinaryDiffLines(Stream BaseFile, AddCopyList List, int iBytesPerLine)
        {
            m_iBytesPerLine = iBytesPerLine;

            BaseFile.Seek(0, SeekOrigin.Begin);

            int iEntries = List.Count;
            for (int i = 0; i < iEntries; i++)
            {
                object oEntry = List[i];
                Addition A = oEntry as Addition;
                if (A != null)
                {
                    //Add A.arBytes to VerLines
                    int iLength = A.arBytes.Length;
                    using (MemoryStream M = new MemoryStream(A.arBytes, false))
                    {
                        AddBytesFromStream(M, 0, iLength, false, true);
                    }

                    //Move the ver position.
                    m_iVerPos += iLength;
                }
                else
                {
                    Copy C = (Copy)oEntry;

                    if (m_iBasePos < C.iBaseOffset)
                    {
                        //Add bytes to BaseLines from m_iBasePos to C.iBaseOffset-1
                        int iLength = C.iBaseOffset - m_iBasePos;
                        AddBytesFromStream(BaseFile, m_iBasePos, iLength, true, false);
                        m_iBasePos += iLength;
                    }

                    //Add copied bytes to both sets of lines.
                    AddBytesFromStream(BaseFile, C.iBaseOffset, C.iLength, true, true);

                    //Move the base and version positions.
                    m_iBasePos = C.iBaseOffset + C.iLength;
                    m_iVerPos += C.iLength;
                }
            }

            int iBaseLength = (int)BaseFile.Length;
            if (m_iBasePos < iBaseLength)
            {
                //Add bytes to BaseLines from m_iBasePos to m_Base.Length
                AddBytesFromStream(BaseFile, m_iBasePos, iBaseLength - m_iBasePos, true, false);
            }
        }

        #endregion

        #region Public Properties

        public Collection<string> BaseLines
        {
            get
            {
                return m_BaseLines;
            }
        }

        public Collection<string> VerLines
        {
            get
            {
                return m_VerLines;
            }
        }

        public int LeadingCharactersToIgnore
        {
            get
            {
                //This magic number comes from the 8 hex-digit position marker
                //and the 4 whitespace characters at the beginning of each line.
                return 12;
            }
        }

        #endregion

        #region Private Methods

        private void AddBytesFromStream(Stream S, int iPosition, int iLength, bool bAddToBase, bool bAddToVer)
        {
            S.Seek(iPosition, SeekOrigin.Begin);

            //Figure out the number of lines we'll have
            int iNumLines = iLength / m_iBytesPerLine;
            if (iLength % m_iBytesPerLine != 0) iNumLines++;

            //Keep up with different positions for each line type
            int iBaseLinePos = m_iBasePos;
            int iVerLinePos = m_iVerPos;

            //Build each line and add it to the appropriate collections.
            int iRemainingLength = iLength;
            for (int iLine = 0; iLine < iNumLines; iLine++)
            {
                //Get the line text.
                int iLineLength = Math.Min(iRemainingLength, m_iBytesPerLine);
                string strLine = GetLineString(S, iLineLength);
                iRemainingLength -= iLineLength;

                //Add it to the BaseLines
                if (bAddToBase)
                {
                    m_BaseLines.Add(GetPositionString(iBaseLinePos) + strLine);
                    iBaseLinePos += iLineLength;
                }

                //Add it to the VerLines
                if (bAddToVer)
                {
                    m_VerLines.Add(GetPositionString(iVerLinePos) + strLine);
                    iVerLinePos += iLineLength;
                }
            }
        }

        private string GetLineString(Stream S, int iLength)
        {
            //The magic number 3 appears in this method because each
            //byte takes two hex characters plus a space after it.

            StringBuilder sbHex = new StringBuilder(iLength * 3);
            StringBuilder sbChars = new StringBuilder(iLength);

            for (int i = 0; i < iLength; i++)
            {
                int iByte = S.ReadByte();
                if (iByte == -1)
                {
                    sbHex.Append("   ");
                }
                else
                {
                    byte by = (byte)iByte;
                    sbHex.AppendFormat("{0:X2} ", by);
                    char ch = (char)by;
                    sbChars.Append(char.IsControl(ch) ? '.' : ch);
                }
            }

            while (sbHex.Length < 3 * m_iBytesPerLine)
            {
                sbHex.Append("   ");
            }

            return string.Concat("    ", sbHex.ToString(), "   ", sbChars.ToString());
        }

        private string GetPositionString(int iPosition)
        {
            return string.Format("{0:X8}", iPosition);
        }

        #endregion

        #region Private Data Members

        private int m_iBytesPerLine;
        private Collection<string> m_BaseLines = new Collection<string>();
        private Collection<string> m_VerLines = new Collection<string>();
        private int m_iBasePos;
        private int m_iVerPos;

        #endregion
    }
}
