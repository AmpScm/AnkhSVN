#include "stdafx.h"
#include "utils.h"


using namespace std;

string GetDirectory( const string& path )
{
    string::size_type  lastSlash = path.find_last_of( '\\' );
    return path.substr( 0, lastSlash );
}

string Join( const string& path1, const string& path2 )
{
    if ( path1[path1.length()] != '\\' && path2[0] != '\\' )
        return path1 + "\\" + path2;
    else
        return path1 + path2;
}

string GetModuleDirectory()
{
    TCHAR buffer[1000];

    ::GetModuleFileName( NULL, buffer, 999 );
    return GetDirectory( buffer );
}