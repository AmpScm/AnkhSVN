import os
from Ft.Xml.Domlette import NonvalidatingReader
from Ft.Xml.XPath import Evaluate

class FileNode:
    def __init__( self, path, name, description, handler = None, realpath = None, ):
        self.path = path
        self.handler = handler
        self.realpath = realpath
        self.name = name
        self.description = description


def createXmlFileNode( doc, node ):
    fileNode = doc.createElementNS( None, "file" )
    fileNode.setAttributeNS( None, 'path', node.path )
    
    if node.handler:
        handlerNode = doc.createElementNS( None, "handler" )
        handlerNode.appendChild( doc.createTextNode( node.handler ) )
        fileNode.appendChild( handlerNode )

    if node.realpath:
        realpathNode = doc.createElementNS( None, "realpath" )
        realpathNode.appendChild( doc.createTextNode( node.realpath ) )
        fileNode.appendChild( realpathNode )

   
    nameNode = doc.createElementNS( None, "name" )
    nameNode.appendChild( doc.createTextNode( node.name ) )
    fileNode.appendChild( nameNode )
    
    descriptionNode = doc.createElementNS( None, "description" )
    fileNode.appendChild( descriptionNode )
    descriptionNode.appendChild( doc.createTextNode( node.description ) )
    

    return fileNode

def createDocAndNode( docTemplate ):
    doc = NonvalidatingReader.parseString(
       docTemplate,
        "http://spam.com/base" )

    directory = Evaluate( "/directories/directory", doc )[0]

    return doc, directory
