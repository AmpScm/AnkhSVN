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

	TextDiff.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Collections;

namespace Ankh.Diff.DiffUtils
{
    /// <summary>
    /// This class uses the MyersDiff helper class to difference two
    /// string lists.  It hashes each string in both lists and then 
    /// differences the resulting integer arrays. 
    /// </summary>
    public class TextDiff
    {
        #region Public Members

        public TextDiff(HashType eHashType, bool bIgnoreCase, bool bIgnoreWhiteSpace)
            : this(eHashType, bIgnoreCase, bIgnoreWhiteSpace, 0)
        {
        }

        public TextDiff(HashType eHashType, bool bIgnoreCase, bool bIgnoreWhiteSpace, int iLeadingCharactersToIgnore)
        {
            m_Hasher = new StringHasher(eHashType, bIgnoreCase, bIgnoreWhiteSpace, iLeadingCharactersToIgnore);
        }

        public EditScript Execute(IList A, IList B)
        {
            int[] arHashA = HashStringList(A);
            int[] arHashB = HashStringList(B);

            MyersDiff Diff = new MyersDiff(arHashA, arHashB);
            return Diff.Execute();
        }

        #endregion

        #region Private Methods

        private int[] HashStringList(IList Lines)
        {
            int iNumLines = Lines.Count;
            int[] arResult = new int[iNumLines];

            for (int i = 0; i < iNumLines; i++)
            {
                arResult[i] = m_Hasher.GetHashCode((string)Lines[i]);
            }

            return arResult;
        }

        #endregion

        #region Private Member Variables

        private StringHasher m_Hasher;

        #endregion
    }
}
