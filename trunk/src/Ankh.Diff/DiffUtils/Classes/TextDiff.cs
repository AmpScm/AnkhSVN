#region Copyright And Revision History

/*---------------------------------------------------------------------------

	TextDiff.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
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
