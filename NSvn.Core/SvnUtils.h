#pragma once
#include "stdafx.h"

namespace NSvn
{
    namespace Core
    {
        /// <summary>
        /// Contains utility methods for SVN
        /// TODO: rename to SvnUtils
        /// </summary>
        __gc public class SvnUtils
        {
        public:
            /// <summary>
            /// Takes a full path and strips off all leading directories that are not
            /// working copies.
            /// </summary>
            /// <param name="path">The path.</param>
            /// <returns>The top level working copy.</returns>        
            static System::String* GetWorkingCopyRootedPath( System::String* path );

            static System::String* GetParentDir( System::String* path );

            /// <summary>
            /// Determines whether a given path is a working copy.
            /// </summary>
            /// <param name="path">The path to check.</param>
            /// <returns>True if the path is/is in a working copy.</returns>
            static bool IsWorkingCopyPath( System::String* path );

        private:
            SvnUtils(void)
            {;}
        };
    }
}
