from Ft.Xml.XPath import Conversions

MONDAY = 1
TUESDAY = 2
WEDNESDAY = 3
THURSDAY = 4
FRIDAY = 5
SATURDAY = 6
SUNDAY = 7

def weeksSince1970( year, week ):
    weeks = 0
    for year in range( 1970, year ):
        weeks += weeksInYear( year )

    return weeks + week

class Interval:
    def __init__( self, startWeek, startYear, endWeek, endYear, defaultYear ):
        default = lambda val: int(val) == 0 and int(defaultYear) or int(val) 
        self.startWeek = int( startWeek )
        self.endWeek = int( endWeek )
        self.startYear = default( startYear )
        self.endYear = default( endYear )

    def contains( self, year, week ):
        start = weeksSince1970( self.startYear, self.startWeek )
        end = weeksSince1970( self.endYear, self.endWeek )
        current = weeksSince1970( year, week )
        return current >= start and current <= end
            
            

    

def iterateOverWeeks( ctx, interval, callback ):
    
    #print defaultyear
    #print "%s %s %s %s" %(startweek, startyear, endweek, endyear)
    doc = getDoc( ctx.node )
    weeks = []
    if interval.startYear == interval.endYear:
        weeks.extend( weekRange(doc, interval.startYear, interval.startWeek, 
            interval.endWeek, callback) )
    elif interval.startYear < interval.endYear:
        weeks.extend( weekRange(doc, interval.startYear, interval.startWeek, 
            weeksInYear(interval.startYear), callback ) )
        year = interval.startYear + 1
        while year < interval.endYear:
            weeks.extend( weekRange(doc, year, 1, weeksInYear(year), callback) )
            year += 1

        weeks.extend( weekRange(doc, interval.endYear, 1, interval.endWeek, callback) )

    return weeks

def getWeekNumbers( ctx, startweek, startyear, endweek, endyear, defaultyear ):
    formatWeek = lambda year, weeknr: str(weeknr)
    interval = Interval( startweek, startyear, endweek, endyear, defaultyear )
    return iterateOverWeeks( ctx, interval, formatWeek )

def getWeekStatus( ctx, startweek, startyear, endweek, endyear, defaultyear,
        intervalstartweek, intervalstartyear, intervalendweek, intervalendyear ):
    all = Interval( startweek, startyear, endweek, endyear, defaultyear ) 
    section = Interval( intervalstartweek, intervalstartyear, intervalendweek, intervalendyear, defaultyear ) 
    callback = lambda year, week: section.contains( year, week ) and "busy" or " "

    return iterateOverWeeks( ctx, all, callback )
        
def weekRange( doc, year, startweek, endweek, callback ):
    weeks = []
    for week in range(startweek, endweek + 1):
        element = doc.createElementNS( None, "week" )
        element.appendChild( doc.createTextNode( callback( year, week) ) )
        weeks.append( element )
    return weeks
    

def getDoc( node ):
    """returns a document from an xml node"""
    doc = node
    while doc.parentNode:
        doc = doc.parentNode

    return doc


def weeksInYear( year ):
    """Most years have 52 weeks, but years that start on a Thursday and 
    leap years that start on a Wednesday have 53 weeks. - taken from 
    http://www.tondering.dk/claus/cal/node6.html#SECTION00670000000000000000"""
    if (isLeapYear( year ) and dayOfWeek( 1, 1, year ) == WEDNESDAY):
        return 53
    elif dayOfWeek( 1, 1, year ) == THURSDAY:
        return 53
    else:
        return 52


        
    

def isLeapYear( year ):
    return (year % 4 == 0 and year % 100 != 0) or year % 400 == 0

def dayOfWeek( day, month, year ):
    #algo taken from http://www.tondering.dk/claus/cal/node3.html#SECTION00350000000000000000
    a = (14-month)/12
    y = year - a
    m = month + 12*a - 2
    return (day + y + y/4 - y/100 + y/400 + (31*m)/12) % 7


def transform():

    from Ft.Xml.Xslt import Processor

    p = Processor.Processor()

    p.registerExtensionFunction( "http://cube.iu.hio.no", "get-week-numbers", getWeekNumbers )    
    p.registerExtensionFunction( "http://cube.iu.hio.no", "get-week-status", getWeekStatus )
    
    from Ft.Xml import InputSource
    xform = InputSource.DefaultFactory.fromUri( "gant.xsl" )
    src = InputSource.DefaultFactory.fromUri( "work_schedule.xml" )

    p.appendStylesheet( xform )

    result = p.run( src )

    f = open( "out.html", "w" )
    print >> f, result
    print result
    f.close()

if __name__=="__main__":
    transform()
