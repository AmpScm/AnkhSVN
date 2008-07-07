#region Copyright And Revision History

/*---------------------------------------------------------------------------

	EditDataTypes.cs
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
	public enum EditType { Delete = 1, Insert = 2, Change = 3 };

	public class Edit
	{
		private EditType m_eType;
		private int m_iStartA;	//Where to Delete, Insert, or Change in the "A" sequence
		private int m_iStartB;	//Where to Insert or Change in the "B" sequence
		private int m_iLength;

		public EditType Type { get { return m_eType; } }
		public int StartA { get { return m_iStartA; } }
		public int StartB { get { return m_iStartB; } }
		public int Length { get { return m_iLength; } }

		public Edit(EditType eType, int iStartA, int iStartB, int iLength)
		{
			m_eType = eType;
			m_iStartA = iStartA;
			m_iStartB = iStartB;
			m_iLength = iLength;
		}
	}

	public class EditScript : ReadOnlyCollectionBase
	{
		internal int Add(Edit E)
		{
			if (E.Type == EditType.Change)
				m_iLength += 2*E.Length;
			else
				m_iLength += E.Length;

			return InnerList.Add(E);
		}

		public Edit this[int iIndex]
		{
			get
			{
				return (Edit)InnerList[iIndex];
			}
			set
			{
				InnerList[iIndex] = value;
			}
		}

		public int TotalEditLength
		{
			get
			{
				return m_iLength;
			}
		}

		private int m_iLength = 0;
	}
}
