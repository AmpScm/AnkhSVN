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
using System.Text;
using System.IO;

namespace Utils
{
    public static class FileUtils
    {
        /// <summary>
        /// Copies a directory recursively.
        /// </summary>
        /// <param name="source">The source directory. This must exist.</param>
        /// <param name="target">The target directory. This must not exist.</param>
        public static void CopyDirectory( string source, string target )
        {
            if ( !Directory.Exists(source) )
            {
                throw new ArgumentException( "Directory does not exist: " + source, "source" );
            }

            if ( Directory.Exists(target) )
            {
                throw new ArgumentException( "Directory already exists: " + target, "target" );
            }

            RecursivelyCopyDirectory( source, target );
        }

        private static void RecursivelyCopyDirectory( string source, string target )
        {
            Directory.CreateDirectory( target );

            foreach ( string file in Directory.GetFiles( source ) )
            {
                string fileName = Path.GetFileName( file );
                string sourcePath = Path.Combine( source, fileName );
                string destPath = Path.Combine( target, fileName );

                File.Copy( sourcePath, destPath );
            }

            foreach ( string dir in Directory.GetDirectories( source ) )
            {
                string dirName = Path.GetFileName( dir );
                string sourcePath = Path.Combine( source, dirName );
                string destPath = Path.Combine( target, dirName );

                RecursivelyCopyDirectory( sourcePath, destPath ); 
            }
        }
    }
}
