#include ".\svnutils.h"
#include "Client.h"
#using <mscorlib.dll>


using namespace System;
using namespace System::IO;
using namespace NSvn::Common;

  

/// <summary>
/// Takes a full path and strips off all leading directories that are not
/// working copies.
/// </summary>
/// <param name="path">The path.</param>
/// <returns>The top level working copy.</returns>
String* NSvn::Core::SvnUtils::GetWorkingCopyRootedPath( String* path )
{
    if ( IsWorkingCopyPath( path ) && !IsWorkingCopyPath( GetParentDir( path ) ) )
        return __box(Path::DirectorySeparatorChar)->ToString();

    String* retPath = path;
    int separator = retPath->IndexOf( Path::DirectorySeparatorChar );;
    while( true )
    {
        if ( separator == -1 )
            throw new SvnException( "Path not part of a working copy" );

        String* dir = path->Substring( 0, separator + 1 );                

        // is our current path a working copy?               
        if ( IsWorkingCopyPath( dir ) )
            break;

        // find the next subcomponent
        separator = path->IndexOf( Path::DirectorySeparatorChar, separator + 1 );
    }

    return path->Substring( separator, path->Length - separator );
}

String* NSvn::Core::SvnUtils::GetParentDir( String* path )
{
    if ( path->Length == 1 )
        return 0;
    else return path->get_Chars( path->Length - 1 ) == Path::DirectorySeparatorChar ?
        Path::GetDirectoryName( path->Substring( 0, path->Length - 1 ) ) :
        Path::GetDirectoryName( path );
}

/// <summary>
/// Determines whether a given path is a working copy.
/// </summary>
/// <param name="path">The path to check.</param>
/// <returns>True if the path is/is in a working copy.</returns>
bool NSvn::Core::SvnUtils::IsWorkingCopyPath( String* path )
{
    String* dir;
    if ( !Directory::Exists( path ) )
        dir = Path::GetDirectoryName( path );
    else 
        dir = path;
    return Directory::Exists( Path::Combine( dir, Client::AdminDirectoryName ) );
}