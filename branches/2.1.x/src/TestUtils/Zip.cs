// $Id$
//
// Copyright 2006-2008 The AnkhSVN Project
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

using System;
using System.IO;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;

namespace TestUtils
{
    /// <summary>
    /// Summary description for Zip.
    /// </summary>
    public class Zip
    {
        private Zip()
        {
            // empty
        }

        /// <summary>
        /// Extracts a ZIP file that is embedded as a resource.
        /// </summary>
        /// <param name="destinationPath">The directory to extract to.</param>
        /// <param name="type">The type that contains the resource.</param>
        /// <param name="resourceName">The name of the embedded resource.</param>
        public static void ExtractZipResource( string destinationPath, Type type, 
            string resourceName )
        {
            Directory.CreateDirectory( destinationPath );
            
            //extract to the temp folder
            Stream zipStream = type.Assembly.GetManifestResourceStream( resourceName );
            ExtractZipStream( zipStream, destinationPath );
        }

        /// <summary>
        /// Extract a zip file
        /// </summary>
        /// <param name="zipFile">Path to the zip file</param>
        /// <param name="parentPath">Path to the folder in which we want to extract the
        /// zip file</param>
        public static void ExtractZipStream( Stream zipStream, string parentPath )
        {
            ZipFile zip = new ZipFile( zipStream );

            //Go through all the entries in the zip file
            foreach( ZipEntry entry in zip )
            {
                string destPath = Path.Combine( parentPath, entry.Name );

                //t'is a directory?
                if ( entry.IsDirectory )
                    Directory.CreateDirectory( destPath );
                else
                {  
                    // nope - this is a file
                    // EXTRACT IT!

                    //make sure the parent path exists
                    string parent = Path.GetDirectoryName( destPath );
                    if ( !Directory.Exists( parent ) )
                        Directory.CreateDirectory( parent );

                    Stream instream = zip.GetInputStream( entry );
                    using( Stream outstream = new FileStream( destPath, FileMode.Create ) )
                    {
                        byte[] buffer = new byte[ BUF_SIZE ];
                        int count = 0;
                        do
                        {
                            count = instream.Read( buffer, 0, BUF_SIZE );
                            if( count > 0 )
                                outstream.Write( buffer, 0, count );
                        } while( count > 0 );
                    } // using
                } // else
            } // foreach
        }

        private const int BUF_SIZE = 4096;

    }
}
