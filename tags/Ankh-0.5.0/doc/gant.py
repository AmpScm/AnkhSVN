from Ft.Xml.XPath import Conversions
import ganthelpers
import sys

def transform():

    from Ft.Xml.Xslt import Processor

    p = Processor.Processor()

    p.registerExtensionFunction( "http://arild.no-ip.com/gant", "get-week-numbers", ganthelpers.getWeekNumbers )    
    p.registerExtensionFunction( "http://arild.no-ip.com/gant", "get-week-status", ganthelpers.getWeekStatus )
    
    from Ft.Xml import InputSource
    xform = InputSource.DefaultFactory.fromUri( sys.argv[1] )
    src = InputSource.DefaultFactory.fromUri( sys.argv[2] )

    p.appendStylesheet( xform )

    result = p.run( src )

    f = open( sys.argv[3], "w" )
    print >> f, result
    print result
    f.close()

if __name__=="__main__":
    transform()
