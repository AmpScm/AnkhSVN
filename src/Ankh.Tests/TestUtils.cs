using System;
using EnvDTE;
using Microsoft.Win32;

namespace Ankh.Tests
{
	/// <summary>
	/// Contains utility functions for all the tests in here.
	/// </summary>
	public class TestUtils
	{
        /// <summary>
        /// Retrieve an AddIn object by name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dte"></param>
        /// <returns></returns>
        public static AddIn GetAddin( string name, _DTE dte )
        {
            foreach( AddIn addin in dte.AddIns )
            {
                if ( addin.ProgID == name )
                {
                    return addin;
                }
            }
            return null;
        }

        /// <summary>
        /// Enable or disable loading of Ankh.
        /// </summary>
        /// <param name="enable"></param>
        /// <param name="version"></param>
        public static void ToggleAnkh( bool enable, string version )
        {
            /*string keyname = String.Format( Key, version );
            int val = enable ? 1 : 0;
            // HKCU?
            RegistryKey key = Registry.CurrentUser.OpenSubKey( keyname, true );
            if ( key != null )
                key.SetValue( "LoadBehavior", val );

            // HKLM
            try
            {
                key = Registry.LocalMachine.OpenSubKey( keyname, true );
                if ( key != null )
                    key.SetValue( "LoadBehavior", val );
            }
            catch( System.Security.SecurityException )
            {
                // user doesn't have access to the key(shouldn't happen)
                // swallow
            }*/
        }

        private const string Key = 
            @"Software\Microsoft\VisualStudio\{0}\AddIns\Ankh";
	}
}
