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

	GoToDlg.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.27.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.Diff.DiffUtils.Controls
{
	/// <summary>
	/// Summary description for GoToDlg.
	/// </summary>
	public partial class GoToDlg : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label lblLineNumber;
		private Ankh.Diff.NumericTextBox edtLineNumber;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GoToDlg()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		

		public bool Execute(IWin32Window owner, int iMaxLineNumber, out int iLine)
		{
			lblLineNumber.Text = String.Format("&Line Number (1-{0}):", iMaxLineNumber);
			edtLineNumber.MaxValue = iMaxLineNumber;
			if (ShowDialog(owner) == DialogResult.OK)
			{
				iLine = edtLineNumber.IntValue;
				return true;
			}

			iLine = 0;
			return false;
		}
	}
}
