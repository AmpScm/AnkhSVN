import getopt
import sys
import os, os.path
import shutil

COLORIZE = "c:/bin/colorize.exe"

def printUsage():
    print """Usage:
generator INPUTDIRECTORY OUTPUTDIRECTORY
"""

                
class Handler:
    def indexEntry( self ):
        filename = os.path.basename( self.entry )
        return "<a href='%(filename)s.html' target='source'>%(filename)s</a> <br />\n" \
            % locals()

class NullHandler( Handler ):
    def indexEntry( self ):
        return ""
    def process( self, outputdir ):
        pass
        
class ColorizeHandler( Handler ):
    def doColorize( self, outputdir, args = ""):
        file = self.entry
        print "Colorizing %(file)s to directory %(outputdir)s" % \
            locals()
        colorize = COLORIZE
        args = ("%(colorize)s -d \"%(outputdir)s\" %(args)s \"%(file)s\"" % locals())
        print args
        if os.system( args ):
            print "Failed to process " + file
        
class SourceHandler(ColorizeHandler):
    def process( self, outputdir ):   
        self.doColorize( outputdir )
        

class XmlHandler( ColorizeHandler ):
    def process( self, outputdir ): 
        self.doColorize( outputdir, "-f xml" )

class TextHandler( ColorizeHandler ):
    def process( self, outputdir ):
        self.doColorize( outputdir, "-f txt" )

        
    

class ImageHandler( Handler ):
    def process( self, outputdir ):
        targetname = os.path.join( outputdir, os.path.basename(self.entry) )
        shutil.copyfile( self.entry, targetname )
    
    def indexEntry( self ):
        filename = os.path.basename( self.entry )
        return "<a href='%(filename)s' target='source'>%(filename)s</a> <br />\n" \
            % locals()

class MiscHandler( Handler ):
    def process( self, outputdir ):
        pass

    def indexEntry( self ):
        filename = os.path.basename( self.entry )
        return "%s <br/>\n" % filename
            
        


class DirectoryHandler(Handler):
    def indexEntry( self ):
        dirname = os.path.basename(self.entry)
        return "<a href='%(dirname)s/list.html'>%(dirname)s/</a> <br />\n" % locals()

    def process( self, outputdir ):   
        dir = Directory()
        
        print "Processing input directory %s" % self.entry
        for entry in os.listdir( self.entry ):
            handler = getHandler( os.path.join( self.entry, entry ) )
            print "%s: %s" %( entry, handler.__class__.__name__ )
            dir.addHandler( handler )
    
        dir.process( os.path.join( outputdir, os.path.basename( self.entry ) ) )


HandlerMap = {  ".cs" : SourceHandler,
                ".cpp" : SourceHandler,
                ".h" : SourceHandler,
                ".png" : ImageHandler,
                ".bmp" : ImageHandler,
                ".jpg" : ImageHandler,
                ".gif" : ImageHandler,
                ".dll" : MiscHandler,
                ".tlb" : MiscHandler,
                ".exe" : MiscHandler,
                ".xml" : XmlHandler,
                ".xsl" : XmlHandler,
                ".sln" : TextHandler,
                ".html" : XmlHandler,
                ".build" : XmlHandler,
                ".resx" : XmlHandler,
                ".aspx" : XmlHandler,
                ".config" : XmlHandler,
                ".txt" : TextHandler,
                ".reg" : TextHandler,
                ".vcproj" : XmlHandler,
                ".csproj" : XmlHandler }
                

ExcludeDirs = [ "bin", "obj", "build", ".svn", "Debug", "Release" ]


                

def getHandler( entry ):
    handler = MiscHandler()
    if os.path.isdir( entry ):
        if ExcludeDirs.count( os.path.basename(entry) ) == 0:
            handler = DirectoryHandler()
        else:
            handler = NullHandler()
    else:
        ext = os.path.splitext(entry)[1]        
        if HandlerMap.has_key( ext ):
            handler = HandlerMap[ext]()
        

    handler.entry = entry
    return handler

def compFunc( handler1, handler2 ):
    if isinstance( handler1, DirectoryHandler ) \
        and not isinstance( handler2, DirectoryHandler ):           
            return -1
    else:
        if isinstance( handler2, DirectoryHandler ) and not \
            isinstance( handler1, DirectoryHandler ):
                return 1

    return cmp( handler1.entry.lower(), handler2.entry.lower() )
        
            
        
        

        
class Directory:
    INDEXNAME="list.html"
    
    def __init__( self ):
        self.__handlers = []

    def addHandler( self, handler ):
        self.__handlers.append( handler )
        

    def process( self, outputdir ):
        print outputdir
        if not os.path.exists( outputdir ):
            print "Creating output directory %s" % outputdir
            os.mkdir( outputdir )
        
        index = open( os.path.join( outputdir, Directory.INDEXNAME ), "w" )
        basename = os.path.basename( outputdir )
        print >> index, """
        <html>
            <head>
            <style type="text/css">
            BODY
            {
                font-size: 10pt;
                font-family: 'Arial Narrow';
            }
                <title>%(basename)s
                </title>
            </head>
        <body>
        <img src="src\Help.doc\img\logo(250x68).png"/>
        <h3>%(basename)s</h3>
        <a href="../list.html">Up</a> <br/> <br/>
        """ % locals()

        self.__handlers.sort( compFunc )

        for handler in self.__handlers:
            print >> index, handler.indexEntry(),
            handler.process( outputdir )        

        print >> index, "</body></html>"
        index.close()
        print "Finished in directory %s" % outputdir

        

def generate():

    if len( sys.argv ) < 3:
        printUsage()
        sys.exit( 1 )

    inputdir=sys.argv[1]
    outputdir=sys.argv[2]

    framesetname = os.path.join( outputdir, os.path.basename( inputdir ), 
        "index.html" )
    frameset = open( framesetname, "w" )
    print >> frameset, """<html><head><title>Source code</title></head>
    <frameset cols="20%,*">
        <frame frameborder="1" name="list" src="list.html"/>
        <frame frameborder="1" name="source" src=""/>
    </frameset></html>"""
    frameset.close()

    handler = DirectoryHandler()
    handler.entry = inputdir
    
    handler.process( outputdir )


if __name__=="__main__":
    generate()



