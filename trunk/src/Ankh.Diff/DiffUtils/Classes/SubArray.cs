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

	SubArray.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Text;
using System.Diagnostics;

namespace Ankh.Diff.DiffUtils
{
    /// <summary>
    /// Allows 1..M access for a selected portion of an int array.
    /// </summary>
    internal sealed class SubArray
    {
        private int[] m_arData;
        private int m_iOffset; //Stores the 0-based offset into m_arData where this subarray should start
        private int m_iLength; //Stores the length of this subarray

        public SubArray(int[] arData)
        {
            m_arData = arData;
            m_iOffset = 0;
            m_iLength = m_arData.Length;
        }

        public SubArray(SubArray arData, int iOffset, int iLength)
        {
            m_arData = arData.m_arData;
            //Subtract 1 here because iOffset will be 1-based
            m_iOffset = arData.m_iOffset + iOffset - 1;
            m_iLength = iLength;
        }

        public int Length
        {
            get
            {
                return m_iLength;
            }
        }

        public int this[int iIndex]
        {
            get
            {
                //Subtract 1 because iIndex will be 1-based.
                return m_arData[m_iOffset + iIndex - 1];
            }
        }

        public int Offset
        {
            get
            {
                //Return the 0-based offset into the main int array
                return m_iOffset;
            }
        }

        public override string ToString()
        {
            int iLength = Length;
            StringBuilder B = new StringBuilder(3 * iLength);
            for (int i = 0; i < iLength; i++)
            {
                B.AppendFormat("{0} ", m_arData[m_iOffset + i]);
            }

            return B.ToString();
        }
    }
}
