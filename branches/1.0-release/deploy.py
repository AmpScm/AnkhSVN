import win32com.client

import sys, os, os.path
import glob
import shutil
import urllib
import tarfile


vcprojectengine = "VisualStudio.VCProjectEngine.7"

# The URL to build from
ANKHSVN_ROOT = "http://ankhsvn.com/svn/finalproject/branches/1.0-release"

# version numbers to put in the filename
MAJOR, MINOR, PATCH, LABEL = 1, 0, 1, "Final"

# The URL of the Subversion version
SUBVERSION = "http://svn.collab.net/repos/svn/tags/1.4.2/"
SUBVERSION_VERSION="1.4.2"
subversion_dir=""

# The URL of neon
NEON = "http://www.webdav.org/neon/neon-0.25.5.tar.gz"
NEON_VERSION="0.25.5"
neon_dir=""

# Berkeley DB
BDB = "ftp://ftp.sleepycat.com/releases/db-4.4.20.tar.gz"
BDB_DIR = "db-4.4.20"
BDB_VERSION="4.4.20"

# OpenSSL
OPENSSL = "http://www.openssl.org/source/openssl-0.9.8a.tar.gz"
OPENSSL_VERSION="0.9.8-a"
openssl_target_dir = ""

# ZLIB
ZLIB = "http://www.zlib.net/zlib-1.2.3.tar.gz"
ZLIB_VERSION="1.2.3"
zlib_target_dir = ""

# Whether to build openssl as a static library
# Must be an environment variable so that neon.mak picks up on it
os.environ[ "OPENSSL_STATIC" ] = "1"

# The build directory
BUILDDIR = "build"

# The location of the vsvars file
VSVARS="I:\\Program Files\\Microsoft Visual Studio .NET\\Common7\\Tools\\vsvars32.bat"

# APR
APACHE_CVS = ":pserver:anoncvs@cvs.apache.org:/home/cvspublic"

APR = "http://svn.apache.org/repos/asf/apr/apr/tags/0.9.12/"
APR_VERSION="0.9.12"

APR_UTIL = "http://svn.apache.org/repos/asf/apr/apr-util/tags/0.9.12/"
APR_UTIL_VERSION="0.9.12"

APR_ICONV = "http://svn.apache.org/repos/asf/apr/apr-iconv/tags/0.9.7/"
APR_ICONV_VERSION="0.9.7"


# Whether to use nant to build Ankh
USE_NANT = 1


TMP = "D:\\tmp\\"

# setting CONFIG to the special value "__ALL__" will cause both Debug and Release to be built
CONFIG = "Release"

ErrorContinue = 0


def run( cmd ):
    print "***CMD***: " + cmd
    if os.system( cmd ):
        raise Exception( "Command returned non-zero value" )
    
location = []
def push_location():
    global location
    location.append( os.getcwd() )

def pop_location():
    global location
    os.chdir( location.pop() )
    


#~ def showpath
#~ {
    #~ push-location
    #~ env:path.split(";") | foreach {
        #~ if ( test-path _ )
        #~ {
            #~ set-location _
            #~ ls *.exe
        #~ }
    #~ }

    #~ pop-location

#~ }

#~ def showvars()
#~ {
    
#~ }

def checkout( url, dir = "" ):
    run( "svn co %s %s" % (url, dir) )

def copy_glob( srcpattern, targetdir ):
    files = glob.glob( srcpattern )
    for file in files:
        shutil.copy( file, targetdir )
    
def convertToVcProj( dspFiles ):
    vcproj = win32com.client.Dispatch(vcprojectengine)
    for dsp in dspFiles:
        dspFile = os.path.abspath( dsp )
        root, ext = os.path.splitext(dspFile)
        vcprojFile = root + ".vcproj"
        
        print "Converting %s to %s" % (dspFile, vcprojFile)
        project = vcproj.LoadProject( dspFile )
        

        project.ProjectFile = vcprojFile
        project.Save()    



def create_build_directory():
    dir = BUILDDIR
    num = 0
    while os.path.exists( dir ):
        dir = "%s-%s" % (BUILDDIR, num)
        num += 1

    return dir

#~ def get_env_from_bat( batfile )
#~ {
    #~ run_bat = combine-path TMP "run_bat.cmd"
    #~ echo "call %1" > run_bat
    #~ echo "set" >> run_bat
    
    #~ newenv = cmd.exe "/c" run_bat "batfile"
    
    #~ newenv -match "=" | foreach{
       #~ var, value = _.Split("=")
        #~ "Setting environment variable var to value"
        #~ set-env var value
    #~ }

    #~ remove-item run_bat
#~ }



def parse_args():
    # go through any var=val args on the command line
    
    for arg in [ a for a in sys.argv[1:] if "=" in a ]:
        var, value = arg.split("=")
        print "Setting %s to %s" % (var, value)
        globals()[var] = value
   






# Downloads and extracts a zip or a tarball
def download_and_extract(url, dir, targetname):
    push_location()

    # move to the dir where we want it
    if not os.path.exists( dir):
        os.makedirs( dir)
    os.chdir( dir )    

    cwd = os.getcwd()
    print "Downloading %s to %s" % (url,  cwd)

    basename = os.path.basename(url)
    target = "%s\%s" % (cwd, basename)

    # get the file
    urllib.urlretrieve(url, target)

    basename = os.path.basename(url)

    # extract it
    dirname = basename

    # what type of file?
    if "tar.gz" in basename:
        print "Unzipping %s" % basename

        tar = tarfile.open(target, 'r:gz')
        for item in tar:
            tar.extract(item)
        tar.close()

        dirname = basename[0:-6]
    elif ".zip" in basename:        
        print "Extracting %s using unzip" % basename
        run( "unzip %s" % basename )
    else:
        raise Exception( "%s: unrecognized filetype" % basename )

    # if provided, rename the directory
    if dirname != targetname and targetname:
        print "Moving %s to %s" % (dirname, targetname)
        shutil.move( dirname, targetname )

    # we don't need the tarball(zipball) any more
    #remove-item basename
    pop_location()

def do_neon():
    global neon_dir
    if neon_dir:
        shutil.copytree( neon_dir, "./neon" )
    else:
        download_and_extract( NEON, ".", "neon" )
    
def do_zlib():    
    global zlib_target_dir
    if ( "zlib_target_dir" in globals() and zlib_target_dir ):
        print "Already have zlib. Not doing it"
        return

    push_location()
    os.chdir("subversion")
    #os.mkdir( "zlib" )
    #os.chdir("zlib")
    download_and_extract( ZLIB, ".", "zlib" )
    zlib_target_dir = os.path.abspath( "zlib" )
    pop_location()

def do_openssl():
    global openssl_target_dir
    if  "openssl_target_dir" in globals() and openssl_target_dir:
        print "Already have OpenSSL. Not doing it"
        return
        
    push_location()    
    os.chdir( "subversion" )
    download_and_extract( OPENSSL, ".", "openssl" )
    
    print """
    
    "** Building OpenSSL **"
    ""
    """

    # now build it
    openssl_target_dir = os.path.abspath( "openssl" )

    os.chdir(openssl_target_dir)
    run( "perl Configure VC-WIN32" )
    run( "ms\\do_masm" )
    if os.environ.has_key( "OPENSSL_STATIC" ) and os.environ["OPENSSL_STATIC"]:
        print "Building static OpenSSL libraries"
        run ( "nmake -f ms\\nt.mak" )
    else:
        print "Building dynamic OpenSSL libraries"
        run ( "nmake -f ms\\ntdll.mak" )

    pop_location()
    

def do_berkeley():
    push_location()
    
    print """
    ""
    "** Berkeley DB **"
    ""
    """
    
    bdb_src = os.path.join(TMP, BDB_DIR)
    print "Looking for BDB sources in %s" % bdb_src
    if not os.path.exists(bdb_src):
        print "Not found"
        download_and_extract(BDB, TMP, BDB_DIR)

    slnfile = os.path.join(os.path.join(bdb_src, "build_win32"), "Berkeley_DB.sln")
    print "Looking for %s" % slnfile

    if not os.path.exists( slnfile ):
        print "Not found. Now convert the Berkeley DB workspace to a VS.NET solution"
        sys.exit(1)

    bdb_build_dir = os.path.join(".", BDB_DIR)
    
    print "Copying %s to %s" % (bdb_src, bdb_build_dir)
    shutil.copytree( bdb_src, bdb_build_dir )
    
    
    slnfile = os.path.join( os.path.join(bdb_build_dir, "build_win32" ), "Berkeley_DB.sln" )
    
    if CONFIG == "__ALL__":
        print "Building %s Debug" % slnfile
        run( "devenv %s /Build Debug /project build_all" % slnfile )
        
        print "Building %s Release" % slnfile
        run( "devenv %s /Build Release /project build_all" % slnfile )
    else:
        print "Building %s %s" % (slnfile, CONFIG)
        run( "devenv %s /Build %s /project build_all" % (slnfile, CONFIG) )

    global bdb_target_dir
    bdb_target_dir = os.path.abspath("db4-win32")
    print "Creating %s" % bdb_target_dir
    os.mkdir( bdb_target_dir )
    
    for dir in ( "include", "lib", "bin" ):
        dir = "%s\\%s" % (bdb_target_dir, dir) 
        print "Creating %s" % dir
        os.mkdir( "%s" % dir )

    print "Copying header files to %s\include" % bdb_target_dir
    copy_glob( "%s\\build_win32\\*.h" % bdb_build_dir, 
        "%s\\include" % bdb_target_dir )

    if CONFIG == "__ALL__":
        print "Copying lib files to %s\\lib" % bdb_target_dir
        copy_glob( "%s\\build_win32\\Debug\\*.lib" % bdb_build_dir, 
            "%s\\lib" % bdb_target_dir )
        copy_glob( "%s\\build_win32\\Release\\*.lib" %bdb_build_dir, 
            "%s\\lib" % bdb_target_dir )

        print "Copying DLL files to %s\\bin" % bdb_target_dir
        copy_glob( "%s\\build_win32\\Debug\\*.dll" % bdb_build_dir, 
            "%s\\bin" % bdb_target_dir )
        copy_glob( "%s\\build_win32\\Release\\*.dll" %bdb_build_dir, 
            "%s\\bin" % bdb_target_dir )
    else:
        print "Copying lib files to %s\\lib" % bdb_target_dir
        copy_glob( "%s\\build_win32\\%s\\*.lib" %(bdb_build_dir, CONFIG), 
            "%s\\lib" % bdb_target_dir )

        print "Copying DLL files to %s\\bin" % bdb_target_dir
        copy_glob( "%s\\build_win32\\%s\\*.dll" %(bdb_build_dir, CONFIG),
            "%s\\bin" % bdb_target_dir )

    pop_location()
    

def do_apr():
    print """
    ""
    "** APR **"
    """""
    
    print "Checking out " + APR
    checkout( APR, "apr" )

    print "Checking out " + APR_UTIL
    checkout( APR_UTIL, "apr-util" )

    print "Checking out " + APR_ICONV
    checkout( APR_ICONV, "apr-iconv" )

def do_subversion():

    push_location()
    
    global subversion_dir

    global subversion_dir
    if "subversion_dir" in globals() and subversion_dir:
        print "Already have a subversion directory. Not doing it."
        return
    else:
        print "No subversion dir. Doing it"
    
    print "Creating directory 'subversion'"
    os.mkdir("subversion")
    
    do_openssl()
    do_zlib()
    
    checkout( SUBVERSION, "subversion" )
    os.chdir( "subversion" )
    
    do_apr()

    
    print "Generating APR .vcproj files"
    convertToVcProj( ( \
        "apr\\libapr.dsp", \
        "apr-iconv\\libapriconv.dsp", \
        "apr-iconv\\ccs\libapriconv_ccs_modules.dsp", \
        "apr-iconv\\ces\libapriconv_ces_modules.dsp", \
        "apr-util\\libaprutil.dsp", \
        "apr-util\\uri\\gen_uri_delims.dsp", \
        "apr-util\\xml\\expat\\lib\\xml.dsp") )

    do_berkeley()

    print """
    ""
    ""
    "*** And finally - Subversion! ***"""
    
    do_neon()
    #echo "bdb_target_dir | openssl_target_dir | APR_MODULE | APR_ICONV_MODULE |  APR_UTIL_MODULE"  
    
    gen_make = os.path.abspath( "gen-make.py" )
    
    opts="-t vcproj "
    opts+= "--with-berkeley-db=%s --with-openssl=%s --with-zlib=%s" % \
        (bdb_target_dir, openssl_target_dir, zlib_target_dir)
    
    #showpath
    print "Generating VC++ solution files"
    run ("python %s %s" % (gen_make, opts) )
	

    
    "Building Subversion"
    if CONFIG == "__ALL__":
        run ( "devenv subversion_vcnet.sln /Build Debug /project svn" )
        run ( "devenv subversion_vcnet.sln /Build Release /project svn" )
    else:
        run ( "devenv subversion_vcnet.sln /Build %s /project svn" % CONFIG )

    pop_location()

    # so that the Ankh  build will pick up on it.
    subversion_dir = os.path.abspath("subversion")
     
def get_revision(dir):
    ostream = os.popen( "svnversion %s" % dir, "r" )
    return ostream.readline()

def add_version( name, version ):
    print "version: %s" % version
    tag = ""
    ver = ""
    if "-" in version: 
        ver, tag = version.split("-")
    else:
        ver = version
    rev = "0"
    parts = ver.split(".")
    if len(parts) == 4:
        major, minor, patch, rev = parts
    else:
        major, minor, patch = parts
        
    params = "\"%s\", %s, %s, %s, Tag=\"%s\"" % (name, major, minor, patch, tag )
    #params = "`"`", {1}, {2}, {3}, Tag=`"{4}`"" -f `
    #   name, maj, min, r, tag 

    open( "AssemblyInfo.cs", "a").write( "[assembly:Utils.VersionAttribute( %s )]\n" % params )
    open( "AssemblyInfo.cpp", "a").write( "[assembly:Utils::VersionAttribute( %s )];\n" % params )

def create_assemblyinfo():
    rev = get_revision("src")
    versiontuple = "%s.%s.%s.%s" % (MAJOR, MINOR, PATCH, rev)
    ankhversion="%s-%s" % (versiontuple, LABEL)
    
    print "Creating AssemblyInfo.cs"
    print os.getcwd()
    
    open( "AssemblyInfo.cs", "w").write( """using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:AssemblyVersion("%s")]
        """ % versiontuple.strip() )


    print "Creating AssemblyInfo.cpp"
    
    open( "AssemblyInfo.cpp", "w" ).write( """#include "stdafx.h"

using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
[assembly:AssemblyVersionAttribute("%s")];
        """ % versiontuple.strip() )

    add_version ("Subversion", SUBVERSION_VERSION)
    add_version ( "Neon", NEON_VERSION )
    add_version ("Berkeley DB", BDB_VERSION )
    add_version ("OpenSSL", OPENSSL_VERSION )
    add_version ("ZLib", ZLIB_VERSION )
    add_version ("apr", APR_VERSION )
    add_version ("apr-util", APR_UTIL_VERSION )
    add_version ("apr-iconv", APR_ICONV_VERSION )
    add_version ("Ankh", ankhversion)

    shutil.copy( "AssemblyInfo.cs", "src\Ankh\AssemblyInfo.cs" )
    shutil.copy( "AssemblyInfo.cs",  "src\Ankh.UI\AssemblyInfo.cs" )
    shutil.copy( "AssemblyInfo.cs", "src\Utils\AssemblyInfo.cs" )
    shutil.copy( "AssemblyInfo.cs", "src\NSvn.Common\AssemblyInfo.cs" )
    shutil.copy( "AssemblyInfo.cpp", "src\NSvn.Core\AssemblyInfo.cpp" )
    shutil.copy( "AssemblyInfo.cs",  "src\ReposInstaller\AssemblyInfo.cs" )

def checkout_tools():
    checkout ("%s/tools/WiX" % ANKHSVN_ROOT)
    checkout ("%s/tools/nant" % ANKHSVN_ROOT )    
    global wix_binary_dir
    wix_binary_dir = os.path.abspath( "WiX" )
    print "Setting wix_binary_dir to %s" % wix_binary_dir
    global nant_binary
    nant_binary = os.path.join( os.path.abspath("nant"), "nant.exe")
    print "Setting nant_binary to %s" % nant_binary

def zip_svn_tree():
    file = open( "zip.build", "w" )
    print >> file, """<?xml version='1.0' encoding='utf-8' ?>
    <project default='zip' xmlns='http://nant.sourceforge.net/schema/'>
    <target name='zip'>
        <zip zipfile='subversion.zip' verbose='true'>
            <fileset basedir='%s'>
                <includes name='**/*.h'/>
                <includes name='**/*.lib'/>
                <exclude name='db-4*/**'/>
            </fileset>
        </zip>
    </target>
</project>""" % subversion_dir
    file.close()
    
    run( "%s /f:zip.build" % nant_binary)

def do_ankh():
    # do_subversion
    #


    do_subversion()
    
    ankhsvn_src = "%s/src" % ANKHSVN_ROOT
    checkout( ankhsvn_src, "src" )
    
    REVISION = get_revision("src").strip()

    toname = "..\\AnkhSetup-%s.%s.%s.%s-%s.msi" %  (MAJOR, MINOR, PATCH, REVISION, LABEL)

    checkout_tools()
    
    zip_svn_tree()
    

    # get the version thing done
    create_assemblyinfo()
    
    os.chdir("src")

    if USE_NANT:
        run ( "%s /t:net-1.0 /v wix -D:svndir=%s " "-D:wix-binary.dir=%s" % (nant_binary, subversion_dir, wix_binary_dir) )
        shutil.copy( "installsource\\Ankh.msi", toname )
    else:
        os.putenv( "svnsrc", subversion_dir )
        run( "devenv src.sln /build CONFIG /project AnkhSetup" )
        shutil.copy( "Ankh\\AnkhSetup\\CONFIG\\AnkhSetup.msi", toname )


#starttime = [DateTime].Now

# parse any arguments given on the command line
parse_args()

push_location()

# create the build directory
dir = create_build_directory()
os.mkdir(dir)
os.chdir(dir)

print "Created build directory " + dir

do_ankh()
pop_location()

#"{0}" -f ([DateTime].Now.Subtract(starttime))





