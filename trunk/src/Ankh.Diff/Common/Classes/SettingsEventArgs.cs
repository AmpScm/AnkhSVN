#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Copyright © 2005 Bill Menees.  All rights reserved.
	Bill@Menees.com

	$History: SettingsEventArgs.cs $
	
	*****************  Version 1  *****************
	User: Bill         Date: 11/06/05   Time: 12:55p
	Created in $/CSharp/Menees/Classes
	Pulled out of the FormSaver.cs file.

-----------------------------------------------------------------------------*/

#endregion

#region Using Directives

using System;
using System.ComponentModel;

#endregion

namespace Ankh.Diff
{
    [Description("A delegate type for hooking up LoadSettings and SaveSettings notifications.")]
    public delegate void SettingsEventHandler(object sender, SettingsEventArgs e);

    [Description("The event arguments for the LoadSettings and SaveSettings events.")]
    public sealed class SettingsEventArgs : EventArgs
    {
        #region Constructors

        internal SettingsEventArgs(SettingsKey Key)
        {
            m_Key = Key;
        }

        #endregion

        #region Public Properties

        public SettingsKey SettingsKey
        {
            get { return m_Key; }
        }

        #endregion

        #region Private Data Members

        private SettingsKey m_Key;

        #endregion
    }
}
