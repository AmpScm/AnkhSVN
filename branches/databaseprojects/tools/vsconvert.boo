namespace Ankh.Tools

import System
import System.IO
import System.Xml from System.Xml


def ConvertSolution( infile as string, outfile as string, frompattern as string,
        topattern as string ) as void:
    using reader = File.OpenText(infile):
        contents = reader.ReadToEnd()
        contents = contents.Replace( "Version 8.00", "Version 7.10" )
        contents = contents.Replace( frompattern, topattern )
        using writer = File.CreateText(outfile):
            writer.Write( contents )
        
        
        
def ConvertProject( infile as string, outfile as string ) as void:
    doc = XmlDocument()
    doc.Load( infile )
    
    # replace schema and product version on VB and C# projects
    if infile =~ ".*(cs)|(vb)proj":
        node = doc.SelectSingleNode( 
            "VisualStudioProject/*/@ProductVersion" )
        print ( "Setting version attribute" )
        node.Value = "7.0.9466"
        print( node.OuterXml )
        
        # now the schema version
        print( "Setting schema version" )
        node = doc.SelectSingleNode(
            "VisualStudioProject/*/@SchemaVersion" )
        node.Value = "1.0"
        print( node.OuterXml )
        
    # replace version in vcproj files
    if infile =~ ".*vcproj":
        print( "Setting VC++ file version" )
        node = doc.SelectSingleNode(
            "VisualStudioProject/@Version" )
        node.Value = "7.0"
        print( node.OuterXml )
        
        # remove the References node
        node = doc.SelectSingleNode(
            "VisualStudioProject/References" )
        if node != null:
            print( "Warning, removing References node" )
            node.ParentNode.RemoveChild( node )
        
    doc.Save( outfile )
    
_, _, infile, outfile = Environment.GetCommandLineArgs()
if infile =~ ".*sln":
    frompattern, topattern = Environment.GetCommandLineArgs()[4:6]
    ConvertSolution( infile, outfile, frompattern, topattern )
else:
    if infile =~ ".*proj":
        ConvertProject( infile, outfile )
    else:
        print( "Usage: vsconvert [FROMPROJECT TOPROJECT] [FROMSOLUTION TOSOLUTION FROMPATTERN TOPATTERN]" )
    
