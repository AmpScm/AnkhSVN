#region Copyright And Revision History

/*---------------------------------------------------------------------------

	NonSelectButton.cs
	Copyright © 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.27.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Windows.Forms;
using System.Drawing;

namespace Ankh.Diff
{
	[ToolboxBitmap(typeof(Button))]
	public sealed class NonSelectButton : Button
	{
		public NonSelectButton()
		{
			SetStyle(ControlStyles.Selectable, false);
		}
	}
}
