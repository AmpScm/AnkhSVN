import os, os.path
from Ft.Xml.Domlette import NonvalidatingReader
from Ft.Xml.XPath import Evaluate
from Ft.Xml.Domlette import PrettyPrint
import filebrowserhelpers
import sys

PATH="uml\\usecases"
FILES="files.xml"
HANDLER = "showusecase.html"

def createNode( case, doc ):

    realpath = os.path.join( PATH, case ).replace("\\", "/" )

    usecasedoc = NonvalidatingReader.parseUri( realpath )
    name = Evaluate( "/UseCase/Preface/Name/text()", usecasedoc.documentElement )[0].data

    nodeObj = filebrowserhelpers.FileNode(  path = case, 
                                            name=name, 
                                            realpath=realpath,
                                            description = "",
                                            handler = HANDLER )
    
    

    return filebrowserhelpers.createXmlFileNode( doc, nodeObj )


def sync():
    isuc = lambda path: os.path.isfile( os.path.join( PATH, path ) ) and \
        os.path.splitext( path )[1]==".xml"
    usecases = [ file for file in os.listdir( PATH ) if isuc( file ) ]

    
    doc, useCaseRoot = filebrowserhelpers.createDocAndNode( 
"""<directories>
    <directory path="usecases">
        <name>Use cases</name>
    </directory>
</directories>""")
    
    for case in usecases:
        print case
        node = createNode( case, doc )
        useCaseRoot.appendChild( createNode( case, doc ) )


    out = open( sys.argv[1], "w" )
    PrettyPrint( doc, out )
    out.close()

    
    


if __name__=="__main__":
    sync()
