namespace Ankh.Tools

import System
import System.IO
import System.Diagnostics
import System.Text.RegularExpressions


# args: REPOSPATH REVISION URL

def RunCommand( file as string, args as string ) as string:
    proc = Process( )
    proc.StartInfo.FileName = file
    proc.StartInfo.Arguments = args
    proc.StartInfo.RedirectStandardOutput = true
    proc.StartInfo.UseShellExecute = false
    
    proc.Start()
    output = proc.StandardOutput.ReadToEnd()
    proc.WaitForExit()
    if proc.ExitCode != 0:
        raise Exception( "${file} returned an error: ${args}" )
    return output

def CheckoutWc( url as string, root as string ) as string:    
    checkouturl = "${url}/${root}" 
    checkoutpath = ""
    
    # find a dir to check out to
    i = 0
    while true:
        checkoutpath = Path.Combine( "E:\\tmp", "vsconvertwc-${i}" )
        if not (Directory.Exists( checkoutpath ) and File.Exists( checkoutpath )):
            break
        i += 1
    print ("Checking out ${checkouturl} to ${checkoutpath}")
    RunCommand( "svn", "co ${checkouturl} ${checkoutpath}" )
    return checkoutpath

def RecursiveDelete( path as string ):
    for dir in Directory.GetDirectories( path ):
        RecursiveDelete( dir )
    
    for file in Directory.GetFiles( path ):
        File.SetAttributes( file, FileAttributes.Normal )
    
    File.SetAttributes( path, FileAttributes.Normal )
    Directory.Delete( path, true )
    
try:
    _, repospath, revision, url, user, passwd = Environment.GetCommandLineArgs()
    files = RunCommand( "svnlook", "changed -r ${revision} ${repospath}" )
    print (files)
    regex = Regex("\\w\\s*(?'root'((trunk)|(branches/[^/]+))/src)/(?'path'\\S*?2003\\S*(?:(?:proj)|(?:sln)))")
    if regex.IsMatch( files ):
        
        # check out the WC
        wcpath = CheckoutWc( url, regex.Match(files).Groups["root"].Value )
        
        # update each affected project files
        for match as Match in regex.Matches(files):
            path = match.Groups["path"].Value.Replace( "/", "\\" )
            path = Path.Combine( wcpath, path )
            outpath = path.Replace( "2003.", "" )
            print ( "From ${path} to ${outpath}"  )
            if path =~ ".*sln":
                VsconvertModule.ConvertSolution( path, outpath, "2003.", "" )
            else:
                VsconvertModule.ConvertProject( path, outpath )
        
        # commit the change
        message = "Automatic update to 2002 project files"
        print ("Committing ${wcpath}")
        print (RunCommand( "svn", "commit ${wcpath} -m \"${message}\" --username ${user} --password ${passwd}" ))

        print ("Deleting")
        RecursiveDelete( wcpath )
        
    else:
        print ("No match")
except ex as Exception: 
    print ( "Exception: " )
    Console.Error.WriteLine( ex.Message )




