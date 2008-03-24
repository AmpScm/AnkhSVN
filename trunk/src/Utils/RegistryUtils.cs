// $Id$
using System;
using Microsoft.Win32;

namespace Utils
{
    /// <summary>
    /// A class containing various utilities for registry manipulation.
    /// </summary>
    public sealed class RegistryUtils
    {
        private RegistryUtils()
        { }
        /// <summary>
        /// Adds a new value under the 
        /// HKCU\Software\Microsoft\Internet Explorer\TypedURL registry key.
        /// </summary>
        /// <param name="url"></param>
        public static void CreateNewTypedUrl( string url )
        {
            RegistryKey typedUrl = Registry.CurrentUser.OpenSubKey( 
                TypedUrl, true );

            if ( typedUrl == null )
                return;

            // make sure the entry isn't there already
            foreach( string val in typedUrl.GetValueNames() )
            {
                if ( String.Compare( typedUrl.GetValue(val).ToString(), url, true ) == 0 )
                    return;
            }

            // We need to find a free value name of the form "url*" with
            // * being a number
            int urlNumber = typedUrl.ValueCount + 1;

            bool done = false;
            while( !done )
            {
                string valueName = "url" + urlNumber;

                // does this name already exist?
                if ( typedUrl.GetValue( valueName ) == null )
                {
                    // nope, create it
                    typedUrl.SetValue( valueName, url );
                    typedUrl.Close();
                    done = true;
                }
                else
                    urlNumber++;
            }
        }

        private const string TypedUrl = 
            @"Software\Microsoft\Internet Explorer\TypedURLs";
    }
}
