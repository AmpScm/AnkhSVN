import os, os.path
from Ft.Xml.Domlette import NonvalidatingReader
from Ft.Xml.XPath import Evaluate
from Ft.Xml.Domlette import PrettyPrint

PATH="uml\\usecases"
FILES="files.xml"
HANDLER = "showusecase.html"

def createNode( case, doc ):
    fileNode = doc.createElementNS( None, "file" )
    fileNode.setAttributeNS( None, 'path', case )
    
    handlerNode = doc.createElementNS( None, "handler" )
    handlerNode.appendChild( doc.createTextNode( HANDLER ) )
    fileNode.appendChild( handlerNode )

    realpath = os.path.join( PATH, case ).replace("\\", "/" )
    realpathNode = doc.createElementNS( None, "realpath" )
    realpathNode.appendChild( doc.createTextNode( realpath ) )
    fileNode.appendChild( realpathNode )

    usecasedoc = NonvalidatingReader.parseUri( realpath )
    name = Evaluate( "/UseCase/Preface/Name/text()", usecasedoc.documentElement )[0].data
    nameNode = doc.createElementNS( None, "name" )
    nameNode.appendChild( doc.createTextNode( name ) )
    fileNode.appendChild( nameNode )
    
    fileNode.appendChild( doc.createElementNS( None, "description" ) )

    return fileNode


def sync():
    isuc = lambda path: os.path.isfile( os.path.join( PATH, path ) ) and \
        os.path.splitext( path )[1]==".xml"
    usecases = [ file for file in os.listdir( PATH ) if isuc( file ) ]

    doc = NonvalidatingReader.parseUri( FILES )

    usecaseRoot = Evaluate( "/root/directory[@path='design']/directory[@path='usecases']", \
        doc.documentElement )
    if not usecaseRoot:
        print "Blah at files.xml. Aborting"
        return
    usecaseRoot = usecaseRoot[0]
    
    for case in usecases:
        matches = Evaluate( "//file[@path='%s']" % case, doc.documentElement )
        if not matches:
            print "%s not found" % case
            usecaseRoot.appendChild( createNode( case, doc ) )


    out = open( FILES, "w" )
    PrettyPrint( doc, out )
    out.close()

    
    


if __name__=="__main__":
    sync()
