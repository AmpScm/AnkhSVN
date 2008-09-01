import win32com.client
import getopt
import sys
import os, os.path
import filebrowserhelpers
from Ft.Xml.Domlette import PrettyPrint
com = win32com.client

PATH = "doc/uml"
NAME="UML Diagrams"
PERMISSIONS="group, supervisor, collabnet"
PNGTYPE=1

class Diagram:
    def __init__( self, type, guid, notes ):
        self.type = type
        self.guid = guid
        self.notes = notes

def getDiagrams( repos ):
    """returns a dictionary of diagram name->diagram guid mappings"""
    def doGetDiagrams( package, map ):
        #get the diagrams in this package
        for i in range(0, package.Diagrams.Count):
            diagram = package.Diagrams.GetAt(i)
            
            #no empty diagrams
            if diagram.DiagramObjects.Count > 0:
                map[ diagram.Name ] = Diagram( type = diagram.Type, 
                                               guid = diagram.DiagramGUID,
                                               notes = package.Notes )

        #recursively process subpackages
        for i in range(0, package.Packages.Count):
            doGetDiagrams( package.Packages.GetAt(i), map )

    map = {}
    #start at the model level
    for i in range(0, repos.Models.Count):
        doGetDiagrams( repos.Models.GetAt(i), map )

    return map

def saveDiagrams( project, map, outputdir ):
    for name, diagram in map.items():
        #we dont want any spaces in the name
        name = name.replace(" ", "_" ) + ".png" 
        name = os.path.abspath( os.path.join( outputdir, name ) )
        print "Saving " + name
        guid = project.GUIDtoXML( diagram.guid )
        project.PutDiagramImageToFile( guid, name, PNGTYPE )

def createXmlFile( map, xmlfile ):
    doc, directory = filebrowserhelpers.createDocAndNode( """
<directories>
    <directory path="umldiagrams">
        <name>UML Diagrams</name>
        <permissions>group, collabnet, supervisor</permissions>
    </directory>
</directories>""" )

    
    
    for name, diagram in map.items():
        filename = name.replace( " ", "_" ) + ".png"
        realpath = os.path.join( PATH, filename ).replace( "\\", "/" )
        nodeObj = filebrowserhelpers.FileNode(  path =filename,
                                                name = name,
                                                description = "%s\n%s" %
                                                    (diagram.type, diagram.notes),
                                                realpath = realpath )
        directory.appendChild( filebrowserhelpers.createXmlFileNode( doc, nodeObj ) )

    out = open( xmlfile, "w" )
    PrettyPrint( doc, out )
    out.close()
                                                
    
    
        

def exportDiagrams():
    args, rest = getopt.getopt( sys.argv[1:], "x:o:", ("xml", "outputdir") )

    #default to the current directory for outputting the images
    outputdir="."
    xmlfile = ""
    for opt, value in args:
        if opt in ( "-o", "--outputdir" ):
            outputdir = value
        elif opt in( "-x", "--xml" ):
            xmlfile = value
        else:
            print "Unrecognized option " + opt
            sys.exit(1)

    #create the EA object
    repos = com.Dispatch( "EA.Repository" )

    #'rest' should be the eap file
    file = os.path.join( os.getcwd(), " ".join(rest) )
    repos.OpenFile( file )

    map = getDiagrams( repos )
    print "\n".join( ["%s: %s@%s" %(name, dia.type, dia.guid) for (name, dia) 
        in map.items() ] )

    project = com.Dispatch( "EA.Project" )
    saveDiagrams( project, map, outputdir )

    if xmlfile:
        createXmlFile( map, xmlfile )


    
    del repos
    del project

if __name__=="__main__":
    exportDiagrams()

