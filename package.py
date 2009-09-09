################################################################################
# Description: This script will build AnkhSVN and all necessary dependencies.
################################################################################
# Module imports
import datetime, getopt, glob, os, shutil, sys, tarfile, threading, struct
import time, urllib, win32api, win32com.client, win32con, win32gui, zipfile
import fileinput

from cStringIO import StringIO
from subprocess import call
################################################################################

# Global Variables
# These variables are specific to Visual Studio.NET and the .NET Framework SDK
# that will be used during this build process.  The script will work with 2005
# by default but can be ran in a way to work with 2008 as well.
VCPROJECTENGINE = "VisualStudio.VCProjectEngine.7"
VSDTE = "VisualStudio.DTE.7"
VSNETVER = "2002"
DOTNETVER = "1.0"

# The URL to build from
ANKHSVN_SRC_URL = "http://ankhsvn.open.collab.net/svn/ankhsvn/branches/1.X"

# Version numbers to put in the filename
MAJOR, MINOR, PATCH, LABEL = 1, 0, 6, "Release"

# The URL of the Subversion version
SUBVERSION = "http://subversion.tigris.org/downloads/subversion-1.6.5.tar.gz"
SUBVERSION_VERSION = "1.6.5"

# The URL of neon
NEON = "http://www.webdav.org/neon/neon-0.28.5.tar.gz"
NEON_VERSION = "0.28.5"

# Berkeley DB
BDB = "ftp://ftp.sleepycat.com/releases/db-4.4.20.tar.gz"
BDB_VERSION = "4.4.20"

# OpenSSL
OPENSSL = "http://www.openssl.org/source/openssl-0.9.8k.tar.gz"
OPENSSL_VERSION = "0.9.8-k"

# SQLite
SQLITE = "http://www.sqlite.org/sqlite-amalgamation-3.6.17.tar.gz"
SQLITE_VERSION = "3.6.17"

# Whether to build openssl as a static library
# Must be an environment variable so that neon.mak picks up on it
OPENSSL_STATIC = 1
if OPENSSL_STATIC:
    os.environ["OPENSSL_STATIC"] = str(OPENSSL_STATIC)

# ZLIB
ZLIB = "http://www.zlib.net/zlib-1.2.3.tar.gz"
ZLIB_VERSION = "1.2.3"

# APR
APR = "http://archive.apache.org/dist/apr/apr-1.3.8-win32-src.zip"
APR_VERSION = "1.3.8"

# APR-UTIL
APR_UTIL = "http://archive.apache.org/dist/apr/apr-util-1.3.9-win32-src.zip"
APR_UTIL_VERSION = "1.3.9"

# The build directory
BUILDDIR = os.path.join(os.path.dirname(__file__), "build")
BUILDDIR_CUSTOM = False

# The logs directory
LOGDIR = os.path.join(BUILDDIR, "logs")

# setting CONFIG to the special value "__ALL__" will cause both Debug and Release to be built
CONFIG = "Release"

# Build types.  Valid options are "developer" and "release".
BUILD_TYPE = "release"

# Verbosity
VERBOSE = False

################################################################################

def build_ankhsvn():
    """Builds AnkhSVN."""
    subversion_source_root = os.path.join(BUILDDIR,"subversion")
    ankhsvn_source_root = os.path.join(BUILDDIR, "ankhsvn")
    ankhsvn_src_dir = os.path.join(ankhsvn_source_root, "src")
    ankhsvn_tools_dir = os.path.join(ankhsvn_source_root, "tools")
    source_location = ANKHSVN_SRC_URL
    
    if BUILD_TYPE == "developer":
        source_location = os.path.abspath(os.path.join(os.path.dirname(__file__)))
    
    REVISION = get_revision(source_location).strip()
    msi_path = os.path.join(BUILDDIR, "AnkhSetup-%s.%s.%s.%s-%s.msi" % (MAJOR, MINOR, PATCH, REVISION, LABEL))
    log_file = os.path.join(LOGDIR, "ankhsvn.log")
    nant_binary = os.path.join(ankhsvn_tools_dir, "nant", "NAnt.exe")
    wix_binary_dir = os.path.join(ankhsvn_tools_dir, "WiX")
    nant_call = [nant_binary, "/t:net-%s" % DOTNETVER, "/v", "wix", "-D:svndir=%s" % subversion_source_root, "-D:vs_version=%s" % "vs" + VSNETVER,
                 "-D:wix-binary.dir=%s" % wix_binary_dir]
    devenv_call = ["devenv", "src.sln", "/build", "CONFIG", "/project", "AnkhSetup"]
    svn_call = ["svn", "export", "--quiet", "--force", source_location, ankhsvn_source_root]
    
    open(log_file, 'w').close()
    
    print "Building AnkhSVN..."
    
    if os.path.exists(msi_path):
        if VERBOSE:
            print "    AnkhSVN already built"
            
            return
    
    if VERBOSE:
        print "    Exporting AnkhSVN source from %s to %s" % (source_location, ankhsvn_source_root)
    
    tempfile = open(log_file, 'a')
    
    if call(svn_call, cwd=BUILDDIR, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
        print "Problem exporting AnkhSVN source.  Details can be found in %s" % log_file
        sys.exit(1)
    
    tempfile.close()
    
    zip_svn_tree(ankhsvn_source_root, subversion_source_root, log_file)

    create_assemblyinfo(REVISION, ankhsvn_source_root)
    
    tempfile = open(log_file, 'a')
    
    if VERBOSE:
        print "    Compiling (%s)" % CONFIG
    
    if call(nant_call, cwd=ankhsvn_src_dir, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
        print "Problem building AnkhSVN.  Details can be found in %s" % log_file
        sys.exit(1)
    
    tempfile.close()
    
    if VERBOSE:
        print "    Creating MSI at %s" % msi_path
    
    shutil.copy(os.path.join(ankhsvn_src_dir, "installsource", "Ankh.msi"), msi_path)

def patchfiles(directories, suffixes, searches):
    for dir in directories:
        for root, dirs, files in os.walk(dir):
            for name in files:
                if name.endswith(".hw"):
                    print name
                for suffix in suffixes:
                    if name.endswith(suffix[0]):
                        filename = os.path.join(root, name)
                        newfile = filename.replace(suffix[0], suffix[1])
                        
                        if filename != newfile:
                            shutil.copy(filename, newfile)
                        

                        #print "Going to patch %s" % newfile
                        for line in fileinput.input(os.path.join(root, newfile), inplace=1):
                            for search in searches:
                                if line.find(search[0]) > -1:
                                    line = line.replace(search[0], search[1])
                                    #if search[0] == "SVN_USE_WIN32_CRASHHANDLER;":
                                    #    print >> sys.stderr, line
                            print line,
                        
    
def build_subversion():
    """Builds Subversion."""
    subversion_source_root = os.path.join(BUILDDIR,"subversion")
    subversion_dir_base = SUBVERSION.split("/")[-1][:-7]
    subversion_sln_file = os.path.join(subversion_source_root, "subversion_vcnet.sln")
    log_file = os.path.join(LOGDIR, "subversion.log")
    svn_gen_make_call = ["python", "gen-make.py", "-t", "vcproj", "--disable-shared", "--vsnet-version", VSNETVER,
                         "--with-berkeley-db=%s" % os.path.join(subversion_source_root, "db4-win32"),
                         "--with-openssl=%s" % os.path.join(subversion_source_root, "openssl"),
                         "--with-zlib=%s" % os.path.join(subversion_source_root, "zlib"),
                         "--with-sqlite=%s" % os.path.join(subversion_source_root, "sqlite")]
    svn_build_call = ["devenv", subversion_sln_file, "/Build", CONFIG, "/project", "svn"]
    svn_debug_call = ["devenv", subversion_sln_file, "/Build", "Debug", "/project", "svn"]
    svn_release_call = ["devenv", subversion_sln_file, "/Build", "Release", "/project", "svn"]
    
    print "Building Subversion..."
    
    if os.path.exists(os.path.join(subversion_source_root, CONFIG, "subversion", "svn", "svn.exe")):
        if VERBOSE:
            print "    Subversion already built"
            
            return
    
    download_and_extract(SUBVERSION, subversion_dir_base)
    
    aprDir = os.path.join(subversion_source_root, "apr")
    aprUtilDir = os.path.join(subversion_source_root, "apr-util")
    
    patchfiles(
        (aprDir, aprUtilDir), 
        ((".hw", ".h"), (".hw", ".hw"), ("expat.h.in", "expat.h")), 
        (("#define APU_HAVE_APR_ICONV", "#define APU_HAVE_APR_ICONV     0 //"),
        ("#define APR_HAVE_IPV6", "#define APR_HAVE_IPV6 0 //")
        ))

    
    if os.path.exists(os.path.join(subversion_source_root, subversion_dir_base)):
        copy_glob(os.path.join(subversion_source_root, subversion_dir_base) + "/*",
              subversion_source_root)
        
        shutil.rmtree(os.path.join(subversion_source_root, subversion_dir_base))
    
    if VERBOSE:
        print "    Generating Subversion VS.NET solution file"    
    
    tempfile = open(log_file, 'w')
    
    if VERBOSE:
        print "    Running gen-make.py"
    
    download_and_extract(SQLITE, "sqlite")

    if call(svn_gen_make_call, cwd=subversion_source_root, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
        print "Problem building Subversion.  Details can be found in %s" % log_file
    
    tempfile.close()
    
    vcprojDir = os.path.join(subversion_source_root, "build", "win32", "vcnet-vcproj")
    patchfiles(
        (vcprojDir,),
        ((".vcproj", ".vcproj"),),
        (("SVN_USE_WIN32_CRASHHANDLER;",""),
        ('				Optimization="2"', '				Optimization="1"'),
        ('				FavorSizeOrSpeed="1"', '				FavorSizeOrSpeed="2"')
        ))
    
    bdb_inc_dir = os.path.join(subversion_source_root, "db4-win32", "include")
    bdb_lib_dir = os.path.join(subversion_source_root, "db4-win32", "lib")
    apr_util_fix_call = ["perl", os.path.join(subversion_source_root, "apr-util", "build", "w32locatedb.pl"), "dll", bdb_inc_dir, bdb_lib_dir]
    apr_release_call = ["devenv", os.path.join(subversion_source_root, "apr", "libapr.vcproj"), "/Build", "Release", "/project", "libapr"]
    apr_util_release_call = ["devenv", os.path.join(subversion_source_root, "apr-util", "aprutil.sln"), "/Build", "Release", "/project", "libaprutil"]
    tempfile = open(log_file, 'a')
    
    if call(apr_util_fix_call, cwd=subversion_source_root, stderr=tempfile, stdout=tempfile, env=os.environ, shell=True):
        print "Problem running w32locatedb.pl. Details can be found in %s" % log_file
        sys.exit(1)

    convert_dsw_to_sln(os.path.join(subversion_source_root, "apr-util", "aprutil.dsw"))
    
    if call(apr_release_call, cwd=subversion_source_root, stderr=tempfile, stdout=tempfile, env=os.environ, shell=True):
        print "Problem building APR.  Details can be found in %s" % log_file
        sys.exit(1)

    if call(apr_util_release_call, cwd=subversion_source_root, stderr=tempfile, stdout=tempfile, env=os.environ, shell=True):
        print "Problem building APR-util.  Details can be found in %s" % log_file
        sys.exit(1)

    tempfile.close()

    
    # Build Subversion
    if CONFIG == "__ALL__":
        if VERBOSE:
            print "    Compiling (Debug)"
        
        tempfile = open(log_file, 'a')
        
        if call(svn_debug_call, cwd=subversion_source_root, stderr=tempfile, stdout=tempfile, env=os.environ, shell=True):
            print "Problem building Subversion.  Details can be found in %s" % log_file
            #sys.exit(1)
        
        tempfile.close()
        
        if VERBOSE:
            print "    Compiling (Release)"
        
        tempfile = open(log_file, 'a')
        
        if call(svn_release_call, cwd=subversion_source_root, stderr=tempfile, stdout=tempfile, env=os.environ, shell=True):
            print "Problem building Subversion.  Details can be found in %s" % log_file
            #sys.exit(1)
        
        tempfile.close()
    else:
        if VERBOSE:
            print "    Compiling (%s)" % CONFIG
        
        tempfile = open(log_file, 'a')
        
        if call(svn_build_call, cwd=subversion_source_root, stderr=tempfile, stdout=tempfile, env=os.environ, shell=True):
            print "Problem building Subversion.  Details can be found in %s" % log_file
            #sys.exit(1)
        
        tempfile.close()

def build_neon():
    """Builds Neon. (Really doesn't build neon but it does download them so Subversion can.)"""
    neon_source_root = os.path.join(BUILDDIR,"subversion", "neon")
    neon_mk_path = os.path.join(neon_source_root, "neon.mak")
    
    print "Staging Neon..."
    
    download_and_extract(NEON, "neon")
    
    if VERBOSE:
        print "    Patching %s" % neon_mk_path
    
    # Patch neon.mak to remove double quotes around $(MAKE) calls
    # http://svn.haxx.se/users/archive-2007-01/0080.shtml and others
    search_and_replace_file(neon_mk_path, None, "\"$(MAKE)\"", "$(MAKE)")

def build_berkeley_db():
    """Builds Berkeley-DB and stages a directory structure for Subversion to use."""
    subversion_source_root = os.path.join(BUILDDIR,"subversion")
    bdb_source_root = os.path.join(subversion_source_root, "db-4.4.20")
    bdb_target_root = os.path.join(subversion_source_root, "db4-win32")
    bdb_sln_file = os.path.join(bdb_source_root, "build_win32", "Berkeley_DB.sln")
    bdb_dsw_file = os.path.join(bdb_source_root, "build_win32", "Berkeley_DB.dsw")
    log_file = os.path.join(LOGDIR, "berkeley_db.log")
    bdb_build_call = ("devenv", bdb_sln_file, "/Build", CONFIG, "/project", "build_all")
    bdb_debug_call = ("devenv", bdb_sln_file, "/Build", "Debug", "/project", "build_all")
    bdb_release_call = ("devenv", bdb_sln_file, "/Build", "Release", "/project", "build_all")
    build_debug = True
    build_release = True
    build_target = True
    copy_dlls = True
    copy_headers = True
    copy_libs = True
    
    print "Building Berkeley DB..."
    
    download_and_extract(BDB, "db-4.4.20")
    
    if not os.path.exists(bdb_sln_file):
        if VERBOSE:
            print "    Converting Berkeley DB VS workspace to VS.NET solution"
        convert_dsw_to_sln(bdb_dsw_file)
    else:
        if VERBOSE:
            print "    Berkeley DB already converted.  Using %s" % bdb_sln_file
    
    if (os.path.exists(os.path.join(bdb_source_root, "build_win32", "Release", "libdb44.lib")) and
        os.path.exists(os.path.join(bdb_source_root, "build_win32", "Release", "libdb44s.lib")) and
        os.path.exists(os.path.join(bdb_source_root, "build_win32", "Release", "libdb44.dll"))):
        build_release = False
    
    if (os.path.exists(os.path.join(bdb_source_root, "build_win32", "Debug", "libdb44d.lib")) and
        os.path.exists(os.path.join(bdb_source_root, "build_win32", "Debug", "libdb44sd.lib")) and
        os.path.exists(os.path.join(bdb_source_root, "build_win32", "Debug", "libdb44d.dll"))):
        build_debug = False
    
    # Create log file
    open(log_file, 'w').close()
    
    if 1 == 1:
        # Handle Debug build
        if build_debug:
            if VERBOSE:
                print "    Compiling (Debug)"
        
            tempfile = open(log_file, 'a')
        
            if call(bdb_debug_call, cwd=bdb_source_root, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
                print "Problem building Berkeley DB.  Details can be found in %s" % log_file
                sys.exit(1)
            
            tempfile.close()
        else:
            if VERBOSE:
                print "    Berkeley DB (Debug) already built.  Using %s" % bdb_source_root
        
        # Handle Release build
        if build_release:
            if VERBOSE:
                print "    Compiling (Release)"
            
            tempfile = open(log_file, 'a')
            
            if call(bdb_release_call, cwd=bdb_source_root, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
                print "    Problem building Berkeley DB.  Details can be found in %s" % log_file
                sys.exit(1)
            
            tempfile.close()
        else:
            if VERBOSE:
                print "    Berkeley DB (Release) already built.  Using %s" % bdb_source_root
    else:
        # Handle Release build
        if (build_release and CONFIG == "Release") or CONFIG != "Release":
            if VERBOSE:
                print "    Compiling (%s)" % CONFIG
            
            tempfile = open(log_file, 'a')
            
            if call(bdb_build_call, cwd=bdb_source_root, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
                print "Problem building Berkeley DB.  Details can be found in %s" % log_file
                sys.exit(1)
            
            tempfile.close()
        else:
            if VERBOSE:
                print "    Berkeley DB (%s) already built.  Using %s" % (CONFIG, bdb_source_root)
    
    if not os.path.exists(bdb_target_root):
        if VERBOSE:
            print "    Creating Berkeley DB target directory %s" % bdb_target_root
        
        os.mkdir(bdb_target_root)
        
        for dir in ( "include", "lib", "bin" ):
            full_dir_path = os.path.join(bdb_target_root, dir)
            
            if VERBOSE:
                print "    Creating Berkeley DB %s directory %s" % (dir, full_dir_path)
            
            os.mkdir(full_dir_path)
    else:
        if VERBOSE:
            print "    Berkeley DB target directory already found. Using %s" % bdb_target_root
    
    if (len(glob.glob(os.path.join(bdb_source_root, "build_win32") + "/*.h")) ==
        len(glob.glob(os.path.join(bdb_target_root, "include") + "/*.h"))):
        if VERBOSE:
            print "    Berkeley DB header files already found.  Using %s" % os.path.join(bdb_target_root, "include")
    else:
        if VERBOSE:
            print "    Copying header files to %s" % os.path.join(bdb_target_root, "include")
        
        copy_glob(os.path.join(bdb_source_root, "build_win32") + "/*.h", 
            os.path.join(bdb_target_root, "include"))

    if 1 == 1:
        d_dlls = len(glob.glob(os.path.join(bdb_source_root, "build_win32", "Debug") + "/*.dll"))
        r_dlls = len(glob.glob(os.path.join(bdb_source_root, "build_win32", "Release") + "/*.dll"))
        t_dlls = len(glob.glob(os.path.join(bdb_target_root, "bin") + "/*.dll"))
        d_libs = len(glob.glob(os.path.join(bdb_source_root, "build_win32", "Debug") + "/*.lib"))
        r_libs = len(glob.glob(os.path.join(bdb_source_root, "build_win32", "Release") + "/*.lib"))
        t_libs = len(glob.glob(os.path.join(bdb_target_root, "lib") + "/*.lib"))
        
        if (d_dlls + r_dlls) == t_dlls:
            if VERBOSE:
                print "    Berkeley DB dll files already found.  Using %s" % os.path.join(bdb_target_root, "bin")
        else:
            if VERBOSE:
                print "    Copying dll files to %s" % os.path.join(bdb_target_root, "lib")
            
            copy_glob(os.path.join(bdb_source_root, "build_win32", "Debug") + "/*.dll", 
                os.path.join(bdb_target_root, "bin"))
            
            copy_glob(os.path.join(bdb_source_root, "build_win32", "Release") + "/*.dll", 
                os.path.join(bdb_target_root, "bin"))
        
        if (d_libs + r_libs) == t_libs:
            if VERBOSE:
                print "    Berkeley DB lib files already found.  Using %s" % os.path.join(bdb_target_root, "lib")
        else:
            if VERBOSE:
                print "    Copying lib files to %s" % os.path.join(bdb_target_root, "lib")
        
            copy_glob(os.path.join(bdb_source_root, "build_win32", "Debug") + "/*.lib", 
                os.path.join(bdb_target_root, "lib"))
        
            copy_glob(os.path.join(bdb_source_root, "build_win32", "Release") + "/*.lib", 
                os.path.join(bdb_target_root, "lib"))
    else:
        b_dlls = len(glob.glob(os.path.join(bdb_source_root, "build_win32", CONFIG) + "/*.dll"))
        t_dlls = len(glob.glob(os.path.join(bdb_target_root, "bin") + "/*.dll"))
        b_libs = len(glob.glob(os.path.join(bdb_source_root, "build_win32", CONFIG) + "/*.lib"))
        t_libs = t_libs = len(glob.glob(os.path.join(bdb_target_root, "lib") + "/*.lib"))
        
        if b_dlls == t_dlls:
            if VERBOSE:
                print "    Berkeley DB dll files already found.  Using %s" % os.path.join(bdb_target_root, "bin")
        else:
            if VERBOSE:
                print "    Copying dll files to %s" % os.path.join(bdb_target_root, "bin")
        
            copy_glob(os.path.join(bdb_source_root, "build_win32", CONFIG) + "/*.dll", 
                os.path.join(bdb_target_root, "bin"))
        
        if b_libs == t_libs:
            if VERBOSE:
                print "Berkeley DB lib files already found.  Using %s" % os.path.join(bdb_target_root, "lib")
        else:
            if VERBOSE:
                print "Copying lib files to %s" % os.path.join(bdb_target_root, "lib")
            
            copy_glob(os.path.join(bdb_source_root, "build_win32", CONFIG) + "/*.lib", 
                os.path.join(bdb_target_root, "lib"))

def build_apr():
    """Builds apr and apr-util.  (Really doesn't build them but it does download them so Subversion can.)"""
    subversion_source_root = os.path.join(BUILDDIR,"subversion")
    solutions_to_convert = []
    
    print "Staging APR, APR-UTIL"
    
    download_and_extract(APR, "apr")
    download_and_extract(APR_UTIL, "apr-util")
    
    if not os.path.exists(os.path.join(subversion_source_root, "apr", "apr.sln")):
        convert_dsw_to_sln(os.path.join(subversion_source_root, "apr", "apr.dsw"))
    else:
        if VERBOSE:
            print "    Project already converted.  Using %s" % os.path.join(subversion_source_root, "apr", "apr.sln")

    if not os.path.exists(os.path.join(subversion_source_root, "apr-util", "aprutil.sln")):
        convert_dsw_to_sln(os.path.join(subversion_source_root, "apr-util", "aprutil.dsw"))
    else:
        if VERBOSE:
            print "    Project already converted.  Using %s" % os.path.join(subversion_source_root, "apr-util", "aprutil.sln")


def build_zlib():
    """Builds zlib.  (Really doesn't build zlib but it does download so that Subversion can build.)"""
    # Patch zlib/inffas32.asm to work with VS.NET assembler
    # http://dave-programming.blogspot.com/2007/06/compling-zlib.html and others
    zlib_source_root = os.path.join(BUILDDIR,"subversion","zlib")
    patched_path = os.path.join(zlib_source_root, "contrib", "masmx86", "inffas32.asm")
    orig_path = os.path.join(zlib_source_root, "contrib", "masmx86", "inffas32.asm.orig")
    
    print "Staging Zlib..."
    
    download_and_extract(ZLIB, "zlib")
    
    if VERBOSE:
            print "    Patching %s" % patched_path
    
    if not os.path.exists(orig_path):
        os.rename(patched_path, orig_path)
    
        orig = open(orig_path, 'r')
        patched = open(patched_path, 'w')
        lineno = 1
        patch_lines = (647,649,663,720)
        patch_addition = ",dword ptr"
        for line in orig:
            if lineno in patch_lines:
                segs = line.split(",")
                
                patched.write(segs[0] + patch_addition + segs[1])
            else:
                patched.write(line)
            
            lineno += 1
        
        patched.close()
        orig.close()

def build_openssl():
    """Builds OpenSSL"""
    openssl_source_root = os.path.join(BUILDDIR,"subversion","openssl")
    log_file = os.path.join(LOGDIR, "openssl.log")
    
    print "Building OpenSSL..."
    
    # Handle static OpenSSL
    if (os.path.exists(os.path.join(openssl_source_root,"out32","libeay32.lib")) and
        os.path.exists(os.path.join(openssl_source_root,"out32","ssleay32.lib"))):
        if VERBOSE:
            print "    OpenSSL already built.  Using %s" % openssl_source_root
    # Handle shared OpenSSL
    elif (os.path.exists(os.path.join(openssl_source_root,"out32","libeay32.dll")) and
        os.path.exists(os.path.join(openssl_source_root,"out32","ssleay32.dll"))):
        if VERBOSE:
            print "    OpenSSL already built.  Using %s" % openssl_source_root
    # Build OpenSSL
    else:
        perl_call = ["perl","Configure","VC-WIN32", "-D_CRT_NONSTDC_NO_DEPRECATE", "-D_CRT_SECURE_NO_DEPRECATE", "no-dso", "no-kr5", "no-hw"]
        masm_call = [os.path.join(openssl_source_root,"ms","do_ms.bat")]
        nmake_static_call = ["nmake","-f",os.path.join(openssl_source_root,"ms","nt.mak")]
        nmake_shared_call = ["nmake","-f",os.path.join(openssl_source_root,"ms","ntdll.mak")]
        
        download_and_extract(OPENSSL, "openssl")
        
        open(log_file, 'w').close()
    
        tempfile = open(log_file, 'a')
        
        if VERBOSE:
            print "    Configuring OpenSSL"
        
        if call(perl_call, cwd=openssl_source_root, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
            print "Problem building OpenSSL.  Details can be found in %s" % log_file
            sys.exit(1)
        
        tempfile.close()
        tempfile = open(log_file, 'a')
        
        if VERBOSE:
            print "    Generating x86 OpenSSL for MASM"
        
        if call(masm_call, cwd=openssl_source_root, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
            print "Problem building OpenSSL.  Details can be found in %s" % log_file
            sys.exit(1)
        
        tempfile.close()
    
        if os.environ.has_key( "OPENSSL_STATIC" ) and os.environ["OPENSSL_STATIC"]:
            if VERBOSE:
                print "    Compiling (STATIC)"
            
            tempfile = open(log_file, 'a')
            
            if call(nmake_static_call, cwd=openssl_source_root, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
                print "Problem building OpenSSL.  Details can be found in %s" % log_file
                sys.exit(1)
        else:
            if VERBOSE:
                print "    Compiling (SHARED)"
            
            tempfile = open(log_file, 'a')
            
            if call(nmake_shared_call, cwd=openssl_source_root, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
                print "Problem building OpenSSL.  Details can be found in %s" % log_file
                sys.exit(1)
    
        tempfile.close()

################################################################################
# Visual Studio .NET Functions
################################################################################

class EnvDTEDialogClickerThread(threading.Thread):
    """Automates the clicking of the 'Visual C++ Projects' dialog that displays
when converting a multi-project dsw to a sln."""
    stdout = sys.stdout
    stderr = sys.stderr

    def run(self):
        # Reset stdout and stderr
        sys.stdout = self.stdout
        sys.stderr = self.stderr

        window_title = "Visual C++ Project"
        convertProjectsDialog = None
        poll_until = ((datetime.datetime.now()) + (datetime.timedelta(minutes=5)))
    
        while (convertProjectsDialog == None) or (datetime.datetime.now() >= poll_until):
            try:
                convertProjectsDialog = findTopWindow(wantedText=window_title)
            except WinGuiAutoError:
                # Do nothing
                pass
            
            time.sleep(2.5)
        
        if convertProjectsDialog:
            yesToAllButton = findControl(convertProjectsDialog,
                                         wantedClass="Button",
                                         wantedText="Yes To All")
            clickButton(yesToAllButton)
        else:
            print "Unable to convert to VS.NET format."

def convert_dsw_to_sln(dswfile):
    """Will take a dsw file and convert it to an sln file using a mix of EnvDTE
and COM."""
    pushd = os.getcwd()
    slnfile = dswfile[:-3] + "sln"
    poll_until = ((datetime.datetime.now()) + (datetime.timedelta(minutes=5)))
    vsdte = win32com.client.Dispatch(VSDTE)
    win32_auto_click_thread = EnvDTEDialogClickerThread()
    
    win32_auto_click_thread.stdout = sys.stdout
    win32_auto_click_thread.stderr = sys.stderr
    
    win32_auto_click_thread.start()
    
    # Open the dsw file
    vsdte.ExecuteCommand("File.OpenProject", dswfile)
    
    # Save the solution and all subprojects
    vsdte.ExecuteCommand("File.SaveAll")
    
    # Close EnvDTE
    vsdte.Quit()
    
    while not os.path.exists(slnfile) or (datetime.datetime.now() >= poll_until):
        time.sleep(2.5)
    
    if not os.path.exists(slnfile):
        print "Unable to convert the %s file" % dswfile
        sys.exit(1)
    
    os.chdir(pushd)

def convert_dsp_to_vcproj(dspFiles):
    """Will take a list of dsp files and convert each to a vcproj file using
the EnvDTE COM interface."""
    pushd = os.getcwd()
    vcproj = win32com.client.Dispatch(VCPROJECTENGINE)
    
    for dsp in dspFiles:
        try:
            dspFile = os.path.abspath(dsp)
            root, ext = os.path.splitext(dspFile)
            vcprojFile = root + ".vcproj"
            
            if VERBOSE:
                print "    Converting %s to %s" % (dspFile, vcprojFile)
            project = vcproj.LoadProject(dspFile)
            
            project.Save()
        except:
          pass
    
    os.chdir(pushd)

################################################################################

################################################################################
# Windows Automation Functions by Simon Brunning
# http://www.brunningonline.net/simon/blog/archives/winGuiAuto.py.html
################################################################################

def findTopWindow(wantedText=None, wantedClass=None, selectionFunction=None):
    '''Find the hwnd of a top level window.
    You can identify windows using captions, classes, a custom selection
    function, or any combination of these. (Multiple selection criteria are
    ANDed. If this isn't what's wanted, use a selection function.)

    Arguments:
    wantedText          Text which the required window's captions must contain.
    wantedClass         Class to which the required window must belong.
    selectionFunction   Window selection function. Reference to a function
                        should be passed here. The function should take hwnd as
                        an argument, and should return True when passed the
                        hwnd of a desired window.
                    
    Raises:
    WinGuiAutoError     When no window found.

    Usage example:      optDialog = findTopWindow(wantedText="Options")
    '''
    topWindows = findTopWindows(wantedText, wantedClass, selectionFunction)
    if topWindows:
        return topWindows[0]
    else:
        raise WinGuiAutoError("No top level window found for wantedText=" +
                               repr(wantedText) +
                               ", wantedClass=" +
                               repr(wantedClass) +
                               ", selectionFunction=" +
                               repr(selectionFunction))

def findTopWindows(wantedText=None, wantedClass=None, selectionFunction=None):
    '''Find the hwnd of top level windows.
    You can identify windows using captions, classes, a custom selection
    function, or any combination of these. (Multiple selection criteria are
    ANDed. If this isn't what's wanted, use a selection function.)

    Arguments:
    wantedText          Text which required windows' captions must contain.
    wantedClass         Class to which required windows must belong.
    selectionFunction   Window selection function. Reference to a function
                        should be passed here. The function should take hwnd as
                        an argument, and should return True when passed the
                        hwnd of a desired window.

    Returns:            A list containing the window handles of all top level
                        windows matching the supplied selection criteria.'''
    results = []
    topWindows = []
    win32gui.EnumWindows(_windowEnumerationHandler, topWindows)
    for hwnd, windowText, windowClass in topWindows:
        if wantedText and not _normaliseText(wantedText) in _normaliseText(windowText):
            continue
        if wantedClass and not windowClass == wantedClass:
            continue
        if selectionFunction and not selectionFunction(hwnd):
            continue
        results.append(hwnd)
    return results

def clickButton(hwnd):
    '''Simulates a single mouse click on a button

    Arguments:
    hwnd    Window handle of the required button.'''
    _sendNotifyMessage(hwnd, win32con.BN_CLICKED)

def _sendNotifyMessage(hwnd, nofifyMessage):
    '''Send a notify message to a control.'''
    win32gui.SendMessage(win32gui.GetParent(hwnd),
                         win32con.WM_COMMAND,
                         _buildWinLong(nofifyMessage,
                                       win32api.GetWindowLong(hwnd,
                                                              win32con.GWL_ID)),
                         hwnd)

def findControl(topHwnd,
                wantedText=None,
                wantedClass=None,
                selectionFunction=None):
    '''Find a control.
    You can identify a control using caption, classe, a custom selection
    function, or any combination of these. (Multiple selection criteria are
    ANDed. If this isn't what's wanted, use a selection function.)

    Arguments:
    topHwnd             The window handle of the top level window in which the
                        required controls reside.
    wantedText          Text which the required control's captions must contain.
    wantedClass         Class to which the required control must belong.
    selectionFunction   Control selection function. Reference to a function
                        should be passed here. The function should take hwnd as
                        an argument, and should return True when passed the
                        hwnd of the desired control.

    Returns:            The window handle of the first control matching the
                        supplied selection criteria.
                    
    Raises:
    WinGuiAutoError     When no control found.'''
    controls = findControls(topHwnd,
                            wantedText=wantedText,
                            wantedClass=wantedClass,
                            selectionFunction=selectionFunction)
    if controls:
        return controls[0]
    else:
        raise WinGuiAutoError("No control found for topHwnd=" +
                               repr(topHwnd) +
                               ", wantedText=" +
                               repr(wantedText) +
                               ", wantedClass=" +
                               repr(wantedClass) +
                               ", selectionFunction=" +
                               repr(selectionFunction))

def findControls(topHwnd,
                 wantedText=None,
                 wantedClass=None,
                 selectionFunction=None):
    '''Find controls.
    You can identify controls using captions, classes, a custom selection
    function, or any combination of these. (Multiple selection criteria are
    ANDed. If this isn't what's wanted, use a selection function.)

    Arguments:
    topHwnd             The window handle of the top level window in which the
                        required controls reside.
    wantedText          Text which the required controls' captions must contain.
    wantedClass         Class to which the required controls must belong.
    selectionFunction   Control selection function. Reference to a function
                        should be passed here. The function should take hwnd as
                        an argument, and should return True when passed the
                        hwnd of a desired control.

    Returns:            The window handles of the controls matching the
                        supplied selection criteria.'''
    def searchChildWindows(currentHwnd):
        results = []
        childWindows = []
        try:
            win32gui.EnumChildWindows(currentHwnd,
                                      _windowEnumerationHandler,
                                      childWindows)
        except win32gui.error:
            # This seems to mean that the control *cannot* have child windows,
            # i.e. not a container.
            return
        for childHwnd, windowText, windowClass in childWindows:
            descendentMatchingHwnds = searchChildWindows(childHwnd)
            if descendentMatchingHwnds:
                results += descendentMatchingHwnds

            if wantedText and \
               not _normaliseText(wantedText) in _normaliseText(windowText):
                continue
            if wantedClass and \
               not windowClass == wantedClass:
                continue
            if selectionFunction and \
               not selectionFunction(childHwnd):
                continue
            results.append(childHwnd)
        return results

    return searchChildWindows(topHwnd)

class WinGuiAutoError(Exception):
    pass

def _buildWinLong(high, low):
    '''Build a windows long parameter from high and low words.
    See http://support.microsoft.com/support/kb/articles/q189/1/70.asp
    '''
    # return ((high << 16) | low)
    return int(struct.unpack('>L',
                             struct.pack('>2H',
                                         high,
                                         low)) [0])

def _normaliseText(controlText):
    '''Remove '&' characters, and lower case.
    Useful for matching control text.'''
    return controlText.lower().replace('&', '')

def _windowEnumerationHandler(hwnd, resultList):
    '''Pass to win32gui.EnumWindows() to generate list of window handle,
    window text, window class tuples.'''
    resultList.append((hwnd,
                       win32gui.GetWindowText(hwnd),
                       win32gui.GetClassName(hwnd)))

################################################################################

################################################################################
# Utility Functions
################################################################################

def search_and_replace_file(src, dst, token, replacement):
    """Searches a file and replaces token with replacement in new file dst.  If
dst is None, the dst will be src.orig.  If dst exists, it will be overwritten."""
    if not dst:
        dst = src
        src = dst + ".orig"
        
        if os.path.exists(src):
            if VERBOSE:
                print "    File already patched %s" % src
                
            return
        
        os.rename(dst, src)
    
    s = open(src, 'r')
    d = open(dst, 'w')
    
    for line in s:
        d.write(line.replace(token, replacement))
    
    d.close()
    s.close()

def create_assemblyinfo(rev, basedir):
    """Generates the assembly information."""
    versiontuple = "%s.%s.%s.%s" % (MAJOR, MINOR, PATCH, rev)
    ankhversion="%s-%s" % (versiontuple, LABEL)
    assemblyinfo_cs_path = os.path.join(basedir, "AssemblyInfo.cs")
    assemblyinfo_cpp_path = os.path.join(basedir, "AssemblyInfo.cpp")

    if VERBOSE:
        print "    Creating %s" % assemblyinfo_cs_path
    
    file = open(assemblyinfo_cs_path, "w")
    file.write("""using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:AssemblyVersion("%s")]
        """ % versiontuple.strip())
    file.close()

    if VERBOSE:
        print "    Creating %s" % assemblyinfo_cpp_path
    
    file = open(assemblyinfo_cpp_path, "w")
    file.write("""#include "stdafx.h"

using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
[assembly:AssemblyVersionAttribute("%s")];
        """ % versiontuple.strip())
    file.close()

    add_version("Subversion", SUBVERSION_VERSION, assemblyinfo_cs_path, assemblyinfo_cpp_path)
    add_version("Neon", NEON_VERSION, assemblyinfo_cs_path, assemblyinfo_cpp_path)
    add_version("Berkeley DB", BDB_VERSION, assemblyinfo_cs_path, assemblyinfo_cpp_path)
    add_version("OpenSSL", OPENSSL_VERSION, assemblyinfo_cs_path, assemblyinfo_cpp_path)
    add_version("ZLib", ZLIB_VERSION, assemblyinfo_cs_path, assemblyinfo_cpp_path)
    add_version("apr", APR_VERSION, assemblyinfo_cs_path, assemblyinfo_cpp_path)
    add_version("apr-util", APR_UTIL_VERSION, assemblyinfo_cs_path, assemblyinfo_cpp_path)
#    add_version("apr-iconv", APR_ICONV_VERSION, assemblyinfo_cs_path, assemblyinfo_cpp_path)
    add_version("Ankh", ankhversion, assemblyinfo_cs_path, assemblyinfo_cpp_path)

    shutil.copy(assemblyinfo_cs_path, os.path.join(basedir, "src", "Ankh", "AssemblyInfo.cs"))
    shutil.copy(assemblyinfo_cs_path,  os.path.join(basedir, "src", "Ankh.UI", "AssemblyInfo.cs"))
    shutil.copy(assemblyinfo_cs_path, os.path.join(basedir, "src", "Utils", "AssemblyInfo.cs"))
    shutil.copy(assemblyinfo_cs_path, os.path.join(basedir, "src", "NSvn.Common", "AssemblyInfo.cs"))
    shutil.copy(assemblyinfo_cpp_path, os.path.join(basedir, "src", "NSvn.Core", "AssemblyInfo.cpp"))
    shutil.copy(assemblyinfo_cs_path,  os.path.join(basedir, "src", "ReposInstaller", "AssemblyInfo.cs"))

def add_version(name, version, assemblyinfo_cs_path, assemblyinfo_cpp_path):
    """Adds versions of deps to the assembly info file."""
    
    if VERBOSE:
        print "    %s version: %s" % (name, version)
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
    
    file = open(assemblyinfo_cs_path, "a")
    file.write( "[assembly:Utils.VersionAttribute( %s )]\n" % params)
    file.close()
    
    file = open(assemblyinfo_cpp_path, "a")
    file.write( "[assembly:Utils::VersionAttribute( %s )];\n" % params)
    file.close()

def zip_svn_tree(wd, basedir, log_file):
    """Creates an archive of the AnkhSVN deps."""
    zip_build_path = os.path.join(wd, "zip.build")
    nant_binary = os.path.join(os.path.abspath(os.path.dirname(__file__)), "tools", "nant", "NAnt.exe")
    nant_call = [nant_binary, "/f:%s" % zip_build_path]
    
    file = open(zip_build_path, "w")
    file.write("""<?xml version='1.0' encoding='utf-8' ?>
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
</project>""" % basedir)
    file.close()
    
    if VERBOSE:
        print "    Archiving the Subversion build to %s" % os.path.join(wd, "subversion.zip")
    
    if log_file:
        tempfile = open(log_file, 'a')
    else:
        tempfile = sys.stdout

    if call(nant_call, cwd=wd, stdout=tempfile, stderr=tempfile, env=os.environ, shell=True):
        print "Problem zipping the Subversion build environment.  Details can be found in %s" % log_file
    
    if log_file:
        tempfile.close()

def get_revision(path):
    """Gets the revision number a Subversion path.  Supports WC and URL."""
    ostream = os.popen("svn info %s" % path, "r")
    index = 0
    
    for line in ostream.readlines():
        index += 1
        
        if index == 5:
            return line.strip().split(" ")[1]
    
    ostream.close()
    
    return -1

def copy_glob(srcpattern, targetdir):
    """Takes a glob pattern and for all matching paths, copies to the targetdir."""
    files = glob.glob(srcpattern)
    
    for file in files:
        if os.path.isfile(file):
            shutil.copy(file, targetdir)
        else:
            shutil.copytree(file, os.path.join(targetdir, os.path.basename(file)))

def download_and_extract(url, targetname):
    """Downloads an archive and extracts it, if the file doesn't already exist."""
    filename = url.split("/")[-1]
    filename_base = None
    download_root_path = os.path.join(BUILDDIR, "dep_archives")
    download_path = os.path.join(download_root_path, filename)
    target_path = os.path.join(BUILDDIR, "subversion", targetname)
    
    # Check for the existence of the archive
    if os.path.exists(download_path):
        if VERBOSE:
            print "    Using %s" % download_path
        return
    
    if not os.path.exists(download_root_path):
        os.makedirs(download_root_path)
	
    if VERBOSE:
        print "    Downloading %s to %s" % (url,  download_path)

    # Download the archive
    urllib.urlretrieve(url, download_path)

    # Extract based on file type
    if filename.endswith("tar.gz"):
        if VERBOSE:
            print "    Extracting %s" % download_path

        extract_tar_file(download_path, os.path.join(BUILDDIR, "subversion"))
    elif filename.endswith("zip"):
        if VERBOSE:
            print "    Extracting %s" % download_path
		
        extract_zip_file(download_path, os.path.join(BUILDDIR, "subversion"))
    else:
        raise Exception( "%s: unrecognized filetype" % basename )

    paths = glob.glob(target_path + "*")
    
    if len(paths) == 1 and (paths[0] != target_path):
        if VERBOSE:
            print "    Renaming %s to %s" % (paths[0], target_path)
        
        shutil.move(paths[0], target_path)

def extract_tar_file(filename, dir):
    """Extracts the contents of filename to dir"""
    tf = tarfile.open(filename)

    for member in tf.getmembers():
        if member.isdir():
            os.makedirs(os.path.join(dir, member.name))
        elif member.isfile():
            parent = os.path.dirname(os.path.join(dir, member.name))
            if not os.path.exists(parent):
                os.makedirs(parent)

            #print "Extracting file %s to %s" % (member.name, parent)
            tf.extract(member, dir)

def extract_zip_file(filename, dir):
	"""Extracts the contents of filename to dir"""
	zf = zipfile.ZipFile(filename)
	namelist = zf.namelist()
	dirlist = filter(lambda x: x.endswith('/'), namelist)
	filelist = filter(lambda x: not x.endswith('/'), namelist)
	# make base
	pushd = os.getcwd()
	if not os.path.isdir(dir):
		os.mkdir(dir)
	os.chdir(dir)
	# create directory structure
	dirlist.sort()
	for dirs in dirlist:
		dirs = dirs.split('/')
		prefix = ''
		for dir in dirs:
			dirname = os.path.join(prefix, dir)
			if dir and not os.path.isdir(dirname):
				os.mkdir(dirname)
			prefix = dirname
	# extract files
	for fn in filelist:
		out = open(fn, 'wb')
		buffer = StringIO(zf.read(fn))
		buflen = 2 ** 20
		datum = buffer.read(buflen)
		while datum:
			out.write(datum)
			datum = buffer.read(buflen)
		out.close()
	
	os.chdir(pushd)

def print_environment():
    """Prints out important information about the build environment."""
    print "------------------------------------------------------------"
    print "Build Directory : %s" % BUILDDIR
    print "Build Type      : %s" % BUILD_TYPE
    print "Log Directory   : %s" % LOGDIR
    print "VS.NET Version  : %s" % VSNETVER
    print ".NET SDK Version: %s" % DOTNETVER
    print "------------------------------------------------------------"

def prepare_environment(build_dir, vsdotnet_ver, dotnet_ver, build_type, verbose):
    """Validates the build directory and VS.NET version.  Sets globals accordingly."""
    global BUILDDIR
    global BUILDDIR_CUSTOM
    global LOGDIR
    global VCPROJECTENGINE
    global VSDTE
    global VSNETVER
    global DOTNETVER
    global BUILD_TYPE
    global VERBOSE
    
    if build_type == "release" and build_dir:
        print "You cannot specify --build-dir with a --build-type of 'release'."
        
        sys.exit(0)
    
    valid_build_types = ('developer','release')
    valid_dotnet_vers = ('2.0','3.0','3.5')
    vcprojectengine_base = "VisualStudio.VCProjectEngine."
    vsdte_base = "VisualStudio.DTE."
    
    if build_dir:
        if os.path.exists(build_dir) and os.path.isdir(build_dir) and os.access(build_dir, os.W_OK):
            BUILDDIR = build_dir
            BUILDDIR_CUSTOM = True
    else:
        dir = BUILDDIR
        num = 0
        
        while os.path.exists( dir ):
            dir = "%s-%s" % (BUILDDIR, num)
            num += 1
        
        BUILDDIR = dir
    
    if vsdotnet_ver:
        if vsdotnet_ver == "2008":
            VCPROJECTENGINE = vcprojectengine_base + "9.0"
            VSDTE = vsdte_base + "9.0"
            
            if dotnet_ver:
                if dotnet_ver in valid_dotnet_vers:
                    DOTNETVER = dotnet_ver
                else:
                    print "Invalid .NET Framework version %s." % dotnet_ver
                    
                    usage()
            else:
                DOTNETVER = "3.5"
        elif vsdotnet_ver == "2005":
            pass # Do nothing since this is the default
        else:
            print "Invalid VS.NET version %s. Please see the usage below." % str(vsdotnet_ver)
            usage()
        
        VSNETVER = vsdotnet_ver
    
    subversion_build_root = os.path.join(BUILDDIR, "subversion")
    
    if not BUILDDIR_CUSTOM:
        os.mkdir(str(BUILDDIR))
    
    if not (os.path.exists(subversion_build_root)):
        os.mkdir(subversion_build_root)
    
    BUILDDIR = os.path.abspath(BUILDDIR)
    
    if build_type:
        if build_type in valid_build_types:
            BUILD_TYPE = build_type
        else:
            print "Invalid build type %s.  Please see the usage below." % build_type
            
            usage()
    
    if verbose:
        VERBOSE = True
    
    LOGDIR = os.path.join(BUILDDIR, "logs")
    
    if not os.path.exists(LOGDIR):
        os.mkdir(LOGDIR)

def parse_args():
    """Parses the command line arguments."""
    build_dir = None
    vsdotnet_ver = None
    dotnet_ver = None
    build_type = None
    verbose = False
    
    try:
        opts, args = getopt.gnu_getopt(sys.argv,
                                       "b:d:n:t:vh",
                                       ["build-dir=","vsdotnet-ver=","dotnet-ver",
                                        "build-type=","verbose","help"])
    except getopt.GetoptError:
        print "Problem parsing arguments"
        usage()

    for opt, arg in opts:
        if opt in ("-h", "--help"):
            usage()
        elif opt in ("-b", "--build-dir"):
            build_dir = os.path.abspath(arg)
        elif opt in ("-d", "--vsdotnet-ver"):
            vsdotnet_ver = arg
        elif opt in ("-n", "--dotnet-ver"):
            dotnet_ver = arg
        elif opt in ("-t", "--build-type"):
            build_type = arg
        elif opt in ("-v", "--verbose"):
            verbose = True
        else:
            print "Unsupported option '%s'.  Please see the usage below." % opt
            usage()
    
    return (build_dir, vsdotnet_ver, dotnet_ver, build_type, verbose)

def usage():
	"""Prints out the help content."""
	print """-----------------------------------------------------------------
AnkhSVN Build Script
-----------------------------------------------------------------
Builds AnkhSVN and packages the installation in an MSI.

Usage: python build.py [options]

Options:
    -b, --build-dir      The path to an existing build directory.
                         Default is 'build'.
    -d, --vsdotnet-ver   The VS.NET version to build with. Valid options:
                         [2005,2008] Default is 2005.
    -n, --dotnet-ver     The .NET Framework version to build with.  Valid
                         options are [2.0,3.0,3.5]  Default is 3.5 and this only
                         works when using 2008 as the VS.NET version.
    -t, --build-type     The type of build to do.  Valid options:
                         [developer,release] Default is developer.
    -v, --verbose        Turns on verbose output
    -h, --help           Displays the help output for this script.

-----------------------------------------------------------------
Maintained By: AnkhSVN Developers (http://ankhsvn.open.colla.net)
-----------------------------------------------------------------"""
	sys.exit()

################################################################################

# TODO: Should discuss maybe making CONFIG part of build type
# TODO: Decide when zipping the subversion build directory is required

# Script's entry point.
if __name__ == "__main__":
    options = parse_args()
    prepare_environment(options[0], options[1], options[2], options[3], options[4])
    print_environment()
    build_openssl()
    build_zlib()
    build_apr()
    build_berkeley_db()
    build_neon()
    build_subversion()
    build_ankhsvn()
