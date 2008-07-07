#region Copyright And Revision History

/*---------------------------------------------------------------------------

	DiagonalVector.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Diagnostics;

namespace Ankh.Diff.DiffUtils
{
    /// <summary>
    /// Implements a vector from -MAX to MAX
    /// </summary>
    internal sealed class DiagonalVector
    {
        int m_iMax;
        int[] m_arData;

        public DiagonalVector(int N, int M)
        {
            int iDelta = N - M;

            //We have to add Delta to support reverse vectors, which are
            //centered around the Delta diagonal instead of the 0 diagonal.
            m_iMax = N + M + Math.Abs(iDelta);

            //Create an array of size 2*MAX+1 to hold -MAX..+MAX.
            m_arData = new int[2 * m_iMax + 1];
        }

        public int this[int iUserIndex]
        {
            get
            {
                return m_arData[GetActualIndex(iUserIndex)];
            }
            set
            {
                m_arData[GetActualIndex(iUserIndex)] = value;
            }
        }

        private int GetActualIndex(int iUserIndex)
        {
            int iIndex = iUserIndex + m_iMax;
            Debug.Assert(iIndex >= 0 && iIndex < m_arData.Length);
            return iIndex;
        }
    }
}
