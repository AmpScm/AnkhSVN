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

	BinaryDiff.cs
	Copyright (c) 2002-2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

	BMenees	11.02.2003	Fixed a bug where Burns's pseudo-code said to use 
						ver_pos, but it really needed to use 
						hashtable[baseh].offset.  This caused bytes from 
						the version file to be lost.
-----------------------------------------------------------------------------*/

#endregion

using System;
using System.IO;
using System.Diagnostics;

namespace Ankh.Diff.DiffUtils
{
    /// <summary>
    /// This class implements a binary differencing algorithm to return a
    /// near-optimal set of differences.  It uses the algorithm from "A
    /// Linear Time, Constant Space Differencing Algorithm" by Randal C.
    /// Burns and Darrell D. E. Long.  It is based on the pseudo-code from
    /// Randal C. Burns's master thesis entitled "Differential Compression:
    /// A Generalized Solution For Binary Files".
    /// 
    /// Finding an optimal set of differences requires quadratic time
    /// relative to the input size, so it becomes unusable very quickly.
    /// This near-optimal linear algorithm is good enough for most cases.
    /// 
    /// As the Burns/Long paper suggested, this implementation uses a
    /// Karp-Rabin hashing scheme so that sequential footprints can be
    /// easily determined based on the previous hash and the next byte.
    /// 
    /// The base implementation of this class returns the diffs in an
    /// AddCopyList.  If for performance reasons you need the diffs in a
    /// different format, then inherit from BinaryDiff and provide
    /// overrides for EmitAdd and EmitCopy.  Then you can dump the diffs
    /// out however you need (e.g. to a member Stream called m_DiffFile).
    /// </summary>
    public class BinaryDiff
    {
        #region Constructors

        public BinaryDiff()
        {
        }

        #endregion

        #region Public Methods and Properties

        /// <summary>
        /// Does a binary diff on the two streams and returns an <see cref="AddCopyList"/>
        /// of the differences.
        /// </summary>
        /// <param name="BaseFile">The base file.</param>
        /// <param name="VerFile">The version file.</param>
        /// <returns>An AddCopyList that can be used later to construct the version file from the base file.</returns>
        public AddCopyList Execute(Stream BaseFile, Stream VerFile)
        {
            if (!BaseFile.CanSeek || !VerFile.CanSeek)
            {
                throw new ArgumentException("The Base and Version streams must support seeking.");
            }

            TableEntry[] arTable = new TableEntry[m_iTableSize];
            AddCopyList List = new AddCopyList();

            BaseFile.Seek(0, SeekOrigin.Begin);
            VerFile.Seek(0, SeekOrigin.End);

            int iVerPos = 0;
            int iBasePos = 0;
            int iVerStart = 0;
            bool bBaseActive = true;
            uint uiVerHash = 0;
            uint uiBaseHash = 0;
            int iLastVerHashPos = 0;
            int iLastBaseHashPos = 0;

            while (iVerPos <= (VerFile.Length - m_iFootprintLength))
            {
                //The GetTableEntry procedure will add the entry if it isn't already there.
                //This gives us a default behavior of favoring the first match.
                uiVerHash = Footprint(VerFile, iVerPos, ref iLastVerHashPos, uiVerHash);
                TableEntry VerEntry = GetTableEntry(arTable, uiVerHash, VerFile, iVerPos);

                TableEntry BaseEntry = null;
                if (bBaseActive)
                {
                    uiBaseHash = Footprint(BaseFile, iBasePos, ref iLastBaseHashPos, uiBaseHash);
                    BaseEntry = GetTableEntry(arTable, uiBaseHash, BaseFile, iBasePos);
                }

                if (BaseFile == VerEntry.File && Verify(BaseFile, VerEntry.Offset, VerFile, iVerPos))
                {
                    int iLength = EmitCodes(VerEntry.Offset, iVerPos, iVerStart, BaseFile, VerFile, List);
                    iBasePos = VerEntry.Offset + iLength;
                    iVerPos += iLength;
                    iVerStart = iVerPos;
                    FlushTable(arTable);
                    continue;
                }
                else if (m_bFavorLastMatch)
                {
                    VerEntry.Offset = iVerPos;
                    VerEntry.File = VerFile;
                }

                bBaseActive = bBaseActive && (iBasePos <= (BaseFile.Length - m_iFootprintLength));
                if (bBaseActive)
                {
                    if (VerFile == BaseEntry.File && Verify(VerFile, BaseEntry.Offset, BaseFile, iBasePos)
                        && iVerStart <= BaseEntry.Offset)
                    {
                        int iLength = EmitCodes(iBasePos, BaseEntry.Offset, iVerStart, BaseFile, VerFile, List);
                        iVerPos = BaseEntry.Offset + iLength;
                        iBasePos += iLength;
                        iVerStart = iVerPos;
                        FlushTable(arTable);
                        continue;
                    }
                    else if (m_bFavorLastMatch)
                    {
                        BaseEntry.Offset = iBasePos;
                        BaseEntry.File = BaseFile;
                    }
                }

                iVerPos++;
                iBasePos++;
            }

            EmitCodes((int)BaseFile.Length, (int)VerFile.Length, iVerStart, BaseFile, VerFile, List);

            Debug.Assert(List.TotalByteLength == (int)VerFile.Length, "The total byte length of the AddCopyList MUST equal the length of the version file!");

            return List;
        }

        /// <summary>
        /// The length of bytes to hash together.  This defaults to 8
        /// and must be between 1 and 31.
        /// </summary>
        public int FootprintLength
        {
            get
            {
                return m_iFootprintLength;
            }
            set
            {
                if (value >= 1 && value <= 31)
                {
                    m_iFootprintLength = value;
                    //Computes d = 2^(m-1) with the left-shift operator
                    m_uiDPower = 1;
                    for (int i = 1; i < m_iFootprintLength; i++)
                    {
                        m_uiDPower = (m_uiDPower << 1);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("FootprintLength", value, "The value must be between 1 and 31.");
                }
            }
        }

        /// <summary>
        /// Sets the size of the hashtable to use.  This defaults to 1009.
        /// </summary>
        /// <remarks>
        /// The hash table size should be a prime number.
        /// </remarks>
        public int TableSize
        {
            get
            {
                return m_iTableSize;
            }
            set
            {
                if (value >= 1)
                {
                    m_iTableSize = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("TableSize", value, "The value must be greater than or equal to one.");
                }
            }
        }

        /// <summary>
        /// Whether the first or last match is favored if two segments
        /// hash to the same entry in the hashtable.  This defaults to
        /// false.
        /// </summary>
        public bool FavorLastMatch
        {
            get
            {
                return m_bFavorLastMatch;
            }
            set
            {
                m_bFavorLastMatch = value;
            }
        }

        #endregion

        #region Private Member Functions

        private TableEntry GetTableEntry(TableEntry[] arTable, uint uiHash, Stream File, int iPos)
        {
            int iIndex = (int)(uiHash % arTable.Length);
            TableEntry Result = arTable[iIndex];
            if (Result == null)
            {
                Result = new TableEntry();
                Result.File = File;
                Result.Offset = iPos;
                arTable[iIndex] = Result;
            }

            return Result;
        }

        private int EmitCodes(int iBasePos, int iVerPos, int iVerStart, Stream BaseFile, Stream VerFile, AddCopyList List)
        {
            if (iVerPos > iVerStart)
            {
                EmitAdd(iVerStart, iVerPos - iVerStart, VerFile, List);
            }

            int iCopyLength = ExtendMatch(BaseFile, iBasePos, VerFile, iVerPos);
            if (iCopyLength > 0)
            {
                EmitCopy(iBasePos, iCopyLength, BaseFile, List);
            }

            return iCopyLength;
        }

        protected virtual void EmitAdd(int iVerStart, int iLength, Stream VerFile, AddCopyList List)
        {
            Addition Add = new Addition();
            VerFile.Seek(iVerStart, SeekOrigin.Begin);
            Add.arBytes = new byte[iLength];
            VerFile.Read(Add.arBytes, 0, iLength);
            List.Add(Add);
        }

        private int ExtendMatch(Stream BaseFile, int iBasePos, Stream VerFile, int iVerPos)
        {
            BaseFile.Seek(iBasePos, SeekOrigin.Begin);
            VerFile.Seek(iVerPos, SeekOrigin.Begin);
            int iLength = 0;
            int iByte = 0;
            while ((iByte = BaseFile.ReadByte()) == VerFile.ReadByte() && iByte != -1) iLength++;
            return iLength;
        }

        protected virtual int EmitCopy(int iBasePos, int iLength, Stream BaseFile, AddCopyList List)
        {
            Copy C = new Copy();
            C.iBaseOffset = iBasePos;
            C.iLength = iLength;
            List.Add(C);
            return iLength;
        }

        private void FlushTable(TableEntry[] arTable)
        {
            for (int i = 0; i < arTable.Length; i++)
            {
                arTable[i] = null;
            }
        }

        private uint Footprint(Stream File, int iPos, ref int iLastPos, uint uiLastHash)
        {
            uint uiHash = 0;

            unchecked //We must allow rollovers
            {
                if (iPos == iLastPos + 1)
                {
                    //Rehash using a Karp-Rabin rehashing scheme.
                    File.Seek(iLastPos, SeekOrigin.Begin);
                    int iPrevByte = File.ReadByte();
                    File.Seek(iPos + m_iFootprintLength - 1, SeekOrigin.Begin);
                    int iNextByte = File.ReadByte();
                    return (uint)(((uiLastHash - iPrevByte * m_uiDPower) << 1) + iNextByte);
                }
                else
                {
                    //Generate a new hash
                    File.Seek(iPos, SeekOrigin.Begin);
                    for (int i = 0; i < m_iFootprintLength; i++)
                    {
                        uiHash = (uint)((uiHash << 1) + File.ReadByte());
                    }
                }
            }

            iLastPos = iPos;

            return uiHash;
        }

        private bool Verify(Stream BaseFile, int iBasePos, Stream VerFile, int iVerPos)
        {
            BaseFile.Seek(iBasePos, SeekOrigin.Begin);
            VerFile.Seek(iVerPos, SeekOrigin.Begin);
            return BaseFile.ReadByte() == VerFile.ReadByte();
        }

        #endregion

        #region Private Member Variables

        private int m_iFootprintLength = 8;
        private uint m_uiDPower = 128;
        private int m_iTableSize = 1009;
        private bool m_bFavorLastMatch = false;

        #endregion

        #region Helper Classes

        private class TableEntry
        {
            public Stream File;
            public int Offset;
        }

        #endregion
    }
}
