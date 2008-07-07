#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: VisualStyles.cs $
	
	*****************  Version 2  *****************
	User: Bill         Date: 11/06/05   Time: 12:54p
	Updated in $/CSharp/Menees/Classes
	Updated to use new .NET 2.0 functionality.

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	6.1.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;

#endregion

namespace Ankh.Diff
{
	public static class VisualStyles
	{
		#region Public Properties And Methods

		public static bool EnableApplication()
		{
			//Visual styles are only on XP and up.
			if (!SupportedByOS) return false;

			//See if the XP user even wants styles enabled.
			s_bEnabled = AllowEnabled();

			//Even if the user doesn't want styles, we still
			//need to do this.  If the user toggles the
			//Theme setting while our app is still running,
			//we need to be able to switch between styles.
			//That means we have to load ComCtr32 V6 up front.
			Application.EnableVisualStyles();

			return s_bEnabled;
		}

		public static Form MainForm
		{
			get
			{
				return s_frmMain;
			}
			set
			{
				if (s_frmMain != value)
				{
					if (s_frmMain != null)
					{
						s_frmMain.SystemColorsChanged -= s_SystemColorsChangedEventHandler;
					}

					s_frmMain = value;

					if (s_frmMain != null)
					{
						if (s_SystemColorsChangedEventHandler == null)
						{
							s_SystemColorsChangedEventHandler = new EventHandler(SystemColorsChanged);
						}

						s_frmMain.SystemColorsChanged += s_SystemColorsChangedEventHandler;
					}
				}
			}
		}

		public static void EnableControls(Control Ctrl)
		{
			if (s_bEnabled)
			{
				TabPage Tab = Ctrl as TabPage; 
				if (Tab != null) 
				{
					Tab.UseVisualStyleBackColor = true;
				}

				foreach (Control SubCtrl in Ctrl.Controls) 
				{ 
					EnableControls(SubCtrl); 
				} 
			}
		}

		public static bool Enabled
		{
			get
			{
				return s_bEnabled;
			}
		}

		public static bool SupportedByOS
		{
			get
			{
				return OSFeature.Feature.IsPresent(OSFeature.Themes);
			}
		}

		#endregion

		#region Private Methods

		private static void SystemColorsChanged(object sender, EventArgs e)
		{
			s_bEnabled = AllowEnabled();
		}

		private static bool AllowEnabled()
		{
			return VisualStyleInformation.IsEnabledByUser;
		}
				
		#endregion

		#region Private Data Members

		private static Form s_frmMain;
		private static bool s_bEnabled;
		private static EventHandler s_SystemColorsChangedEventHandler;

		#endregion
	}
}
