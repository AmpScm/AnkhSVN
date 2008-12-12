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

	MyersDiff.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace Ankh.Diff.DiffUtils
{
    /// <summary>
    /// This class implements the differencing algorithm from
    /// "An O(ND) Difference Algorithm And Its Variations" by
    /// Eugene W. Myers.  It is the standard algorithm used by
    /// the UNIX diff utilities.
    /// 
    /// This implementation diffs two integer arrays.  It is
    /// typically used with arrays of hash values where each
    /// hash corresponds to a line of text.  Then it can be
    /// used to diff two text files on a line-by-line basis.
    /// 
    /// Since this implementation diffs integer arrays you can
    /// also do a character-by-character diff by making each
    /// integer equal to a Unicode character value.  This will
    /// waste 2 bytes of each int, but it will work. 
    /// </summary>
    public class MyersDiff
    {
        #region Public Methods

        public MyersDiff(int[] arA, int[] arB)
        {
            m_arA = arA;
            m_arB = arB;

            int N = arA.Length;
            int M = arB.Length;

            m_vecForward = new DiagonalVector(N, M);
            m_vecReverse = new DiagonalVector(N, M);
        }

        /// <summary>
        /// Returns an EditScript instance that gives all the Edits
        /// necessary to transform A into B.
        /// </summary>
        public EditScript Execute()
        {
            ArrayList MatchPoints = new ArrayList();

            SubArray A = new SubArray(m_arA);
            SubArray B = new SubArray(m_arB);

            GetMatchPoints(A, B, MatchPoints);

            Debug.Assert(MatchPoints.Count == GetLCSLength(), "The number of match points must equal the LCS length.");

            EditScript Script = ConvertMatchPointsToEditScript(A.Length, B.Length, MatchPoints);
            Debug.Assert(Script.TotalEditLength == GetSESLength());

            return Script;
        }

        /// <summary>
        /// Returns the longest common subsequence from A and B.
        /// </summary>
        public int[] GetLCS()
        {
            ArrayList Output = new ArrayList();

            GetLCS(new SubArray(m_arA), new SubArray(m_arB), Output);

            return (int[])Output.ToArray(typeof(int));
        }

        /// <summary>
        /// Calculates the length that the LCS should be without
        /// actually determining the LCS.
        /// </summary>
        public int GetLCSLength()
        {
            //Per Myers's paper, we should always have 
            //D+2L == N+M.  So L == (N+M-D)/2.
            return (m_arA.Length + m_arB.Length - GetSESLength()) / 2;
        }

        /// <summary>
        /// Returns a similary index between 0 and 1 inclusive.
        /// 0 means A and B are completely different.  1 means
        /// they are exactly alike.  The similarity index is
        /// calculated as twice the length of the LCS divided
        /// by the sum of A and B's lengths.
        /// </summary>
        public double GetSimilarity()
        {
            return (2.0 * GetLCSLength()) / (double)(m_arA.Length + m_arB.Length);
        }

        /// <summary>
        /// Calculates the length of the "shortest edit script"
        /// as defined in Myers's paper.
        /// 
        /// Note: This may not be the same as the Count property
        /// of an EditScript instance returned by Execute().  If
        /// an EditScript instance has any Edits with Length > 1, 
        /// then those groupings will make EditScript.Count less
        /// than GetSESLength().  Similarly, an Edit with EditType
        /// Change should be thought of as a combined Delete and
        /// Insert for the specified Length.
        /// </summary>
        public int GetSESLength()
        {
            SubArray A = new SubArray(m_arA);
            SubArray B = new SubArray(m_arB);

            if (SetupFictitiousPoints(A, B))
            {
                int N = m_arA.Length;
                int M = m_arB.Length;

                for (int D = 0; D <= N + M; D++)
                {
                    for (int k = -D; k <= D; k += 2)
                    {
                        int x = GetForwardDPaths(A, B, D, k);
                        int y = x - k;
                        if (x >= N && y >= M)
                        {
                            return D;
                        }
                    }
                }

                //We should never get here if the algorithm is coded correctly.
                Debug.Assert(false);
                return -1;
            }
            else if (m_arA.Length == 0)
            {
                return m_arB.Length;
            }
            else
            {
                return m_arA.Length;
            }
        }

        /// <summary>
        /// Gets the length of the "shortest edit script"
        /// by running the algorithm in reverse.  We should
        /// always have GetSESLength() == GetReverseSESLength().
        /// </summary>
        public int GetReverseSESLength()
        {
            SubArray A = new SubArray(m_arA);
            SubArray B = new SubArray(m_arB);

            if (SetupFictitiousPoints(A, B))
            {
                int N = m_arA.Length;
                int M = m_arB.Length;
                int iDelta = N - M;

                for (int D = 0; D <= N + M; D++)
                {
                    for (int k = -D; k <= D; k += 2)
                    {
                        int x = GetReverseDPaths(A, B, D, k, iDelta);
                        int y = x - (k + iDelta);
                        if (x <= 0 && y <= 0)
                        {
                            return D;
                        }
                    }
                }

                //We should never get here if the algorithm is coded correctly.
                Debug.Assert(false);
                return -1;
            }
            else if (m_arA.Length == 0)
            {
                return m_arB.Length;
            }
            else
            {
                return m_arA.Length;
            }
        }

        #endregion

        #region Private Methods

        private bool SetupFictitiousPoints(SubArray A, SubArray B)
        {
            if (A.Length > 0 && B.Length > 0)
            {
                //Setup some "fictious" endpoints for initial forward
                //and reverse path navigation.
                m_vecForward[1] = 0;
                int N = A.Length;
                int M = B.Length;
                int iDelta = N - M;
                m_vecReverse[iDelta + 1] = N + 1;

                return true;
            }
            else
                return false;
        }

        private int GetForwardDPaths(SubArray A, SubArray B, int D, int k)
        {
            DiagonalVector V = m_vecForward;

            int x;
            if ((k == -D) || (k != D && V[k - 1] < V[k + 1]))
            {
                x = V[k + 1];
            }
            else
            {
                x = V[k - 1] + 1;
            }
            int y = x - k;

            while (x < A.Length && y < B.Length && A[x + 1] == B[y + 1])
            {
                x++;
                y++;
            }

            V[k] = x;

            return x;
        }

        private int GetReverseDPaths(SubArray A, SubArray B, int D, int k, int iDelta)
        {
            DiagonalVector V = m_vecReverse;

            int p = k + iDelta;

            int x;
            if ((k == -D) || (k != D && V[p + 1] <= V[p - 1]))
            {
                x = V[p + 1] - 1;
            }
            else
            {
                x = V[p - 1];
            }
            int y = x - p;

            while (x > 0 && y > 0 && A[x] == B[y])
            {
                x--;
                y--;
            }

            V[p] = x;

            return x;
        }

        private int FindMiddleSnake(SubArray A, SubArray B, out int iPathStartX, out int iPathEndX, out int iPathK)
        {
            //We don't have to check the result of this because the calling procedure
            //has already check the length preconditions.
            SetupFictitiousPoints(A, B);

            iPathStartX = -1;
            iPathEndX = -1;
            iPathK = 0;

            int iDelta = A.Length - B.Length;
            int iCeiling = (int)Math.Ceiling((A.Length + B.Length) / 2.0);

            for (int D = 0; D <= iCeiling; D++)
            {
                for (int k = -D; k <= D; k += 2)
                {
                    //Find the end of the furthest reaching forward D-path in diagonal k.
                    GetForwardDPaths(A, B, D, k);
                    //If iDelta is odd (i.e. remainder == 1 or -1) and ...
                    if ((iDelta % 2 != 0) && (k >= (iDelta - (D - 1)) && k <= (iDelta + (D - 1))))
                    {
                        //If the path overlaps the furthest reaching reverse (D-1)-path in diagonal k.
                        if (m_vecForward[k] >= m_vecReverse[k])
                        {
                            //The last snake of the forward path is the middle snake.
                            iPathK = k;
                            iPathEndX = m_vecForward[k];
                            iPathStartX = iPathEndX;
                            int iPathStartY = iPathStartX - iPathK;
                            while (iPathStartX > 0 && iPathStartY > 0 && A[iPathStartX] == B[iPathStartY])
                            {
                                iPathStartX--;
                                iPathStartY--;
                            }
                            //Length of an SES is 2D-1.
                            return 2 * D - 1;
                        }
                    }
                }

                for (int k = -D; k <= D; k += 2)
                {
                    //Find the end of the furthest reaching reverse D=path in diagonal k+iDelta
                    GetReverseDPaths(A, B, D, k, iDelta);
                    //If iDelta is even and ...
                    if ((iDelta % 2 == 0) && ((k + iDelta) >= -D && (k + iDelta) <= D))
                    {
                        //If the path overlaps the furthest reaching forward D-path in diagonal k+iDelta.
                        if (m_vecReverse[k + iDelta] <= m_vecForward[k + iDelta])
                        {
                            //The last snake of the reverse path is the middle snake.
                            iPathK = k + iDelta;
                            iPathStartX = m_vecReverse[iPathK];
                            iPathEndX = iPathStartX;
                            int iPathEndY = iPathEndX - iPathK;
                            while (iPathEndX < A.Length && iPathEndY < B.Length && A[iPathEndX + 1] == B[iPathEndY + 1])
                            {
                                iPathEndX++;
                                iPathEndY++;
                            }
                            //Length of an SES is 2D.
                            return 2 * D;
                        }
                    }
                }
            }

            //We should never get here if the algorithm is coded correctly.
            Debug.Assert(false);
            return -1;
        }

        private void GetLCS(SubArray A, SubArray B, ArrayList Output)
        {
            if (A.Length > 0 && B.Length > 0)
            {
                //Find the length D and the middle snake from (x,y) to (u,v)
                int x, u, k;
                int D = FindMiddleSnake(A, B, out x, out u, out k);
                int y = x - k;
                int v = u - k;

                if (D > 1)
                {
                    GetLCS(new SubArray(A, 1, x), new SubArray(B, 1, y), Output);

                    for (int i = x + 1; i <= u; i++)
                    {
                        Output.Add(A[i]);
                    }

                    GetLCS(new SubArray(A, u + 1, A.Length - u), new SubArray(B, v + 1, B.Length - v), Output);
                }
                else if (B.Length > A.Length)
                {
                    for (int i = 1; i <= A.Length; i++)
                    {
                        Output.Add(A[i]);
                    }
                }
                else
                {
                    for (int i = 1; i <= B.Length; i++)
                    {
                        Output.Add(B[i]);
                    }
                }
            }
        }

        private void GetMatchPoints(SubArray A, SubArray B, ArrayList MatchPoints)
        {
            if (A.Length > 0 && B.Length > 0)
            {
                //Find the middle snake from (x,y) to (u,v)
                int x, u, k;
                int D = FindMiddleSnake(A, B, out x, out u, out k);
                int y = x - k;
                int v = u - k;

                if (D > 1)
                {
                    GetMatchPoints(new SubArray(A, 1, x), new SubArray(B, 1, y), MatchPoints);

                    for (int i = x + 1; i <= u; i++)
                    {
                        //Output absolute X and Y (not relative to the current subarray)
                        MatchPoints.Add(new Point(i + A.Offset, i - k + B.Offset));
                    }

                    GetMatchPoints(new SubArray(A, u + 1, A.Length - u), new SubArray(B, v + 1, B.Length - v), MatchPoints);
                }
                else
                {
                    //If there are no differences, we have to output all of the points.
                    //If there's only one difference, we have to output all of the
                    //match points, skipping the single point that is different.
                    Debug.Assert(D == 0 || Math.Abs(A.Length - B.Length) == 1, "A and B's lengths must differ by 1 if D == 1");

                    //Only go to the minimum of the two lengths since that's the 
                    //most that can possibly match between the two subsequences.
                    int N = A.Length;
                    int M = B.Length;
                    if (M > N)
                    {
                        //Output A[1..N] as match points
                        int iCurrY = 1;
                        for (int i = 1; i <= N; i++)
                        {
                            //We must skip the one difference when we hit it
                            if (A[i] != B[iCurrY])
                            {
                                iCurrY++;
                            }

                            MatchPoints.Add(new Point(i + A.Offset, iCurrY + B.Offset));
                            iCurrY++;
                        }
                    }
                    else
                    {
                        //Output B[1..M] as match points
                        int iCurrX = 1;
                        for (int i = 1; i <= M; i++)
                        {
                            //We must skip the one difference when we hit it
                            if (A[iCurrX] != B[i])
                            {
                                iCurrX++;
                            }
                            MatchPoints.Add(new Point(iCurrX + A.Offset, i + B.Offset));
                            iCurrX++;
                        }
                    }
                }
            }
        }

        private EditScript ConvertMatchPointsToEditScript(int N, int M, ArrayList MatchPoints)
        {
            EditScript Script = new EditScript();

            int iCurrX = 1;
            int iCurrY = 1;

            //Add a fictitious match point at (N+1, M+1) so we're guaranteed to
            //pick up all edits with a single loop.
            MatchPoints.Add(new Point(N + 1, M + 1));

            //NOTE: When we create new Edit instances, we'll store iCurrX and iCurrY
            //minus 1 because we want to convert them back to 0-based indexes for
            //the user.  The user shouldn't have to know that internally we use any
            //1-based types.

            foreach (Point Pt in MatchPoints)
            {
                int iMatchX = Pt.X;
                int iMatchY = Pt.Y;

                //A one-to-one grouping of inserts and deletes will be considered a change.
                if (iCurrX < iMatchX && iCurrY < iMatchY)
                {
                    int iChangeLength = Math.Min(iMatchX - iCurrX, iMatchY - iCurrY);
                    Script.Add(new Edit(EditType.Change, iCurrX - 1, iCurrY - 1, iChangeLength));
                    iCurrX += iChangeLength;
                    iCurrY += iChangeLength;
                }

                if (iCurrX < iMatchX)
                {
                    Script.Add(new Edit(EditType.Delete, iCurrX - 1, iCurrY - 1, iMatchX - iCurrX));
                }

                if (iCurrY < iMatchY)
                {
                    Script.Add(new Edit(EditType.Insert, iCurrX - 1, iCurrY - 1, iMatchY - iCurrY));
                }

                iCurrX = iMatchX + 1;
                iCurrY = iMatchY + 1;
            }

            return Script;
        }

        #endregion

        #region Private Member Variables

        private int[] m_arA; //Sequence A
        private int[] m_arB; //Sequence B
        private DiagonalVector m_vecForward;
        private DiagonalVector m_vecReverse;

        #endregion
    }
}
