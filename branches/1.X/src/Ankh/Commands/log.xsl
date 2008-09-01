<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="yes" encoding="utf-8"/>
    <xsl:template match="/">
        <html>
            <head>
                <title>Log</title>
                <style type="text/css">
                body 
                {
                    font: normal 8pt verdana,arial,helvetica;
                    color: #000000;
                }
                .headers
                {
                    font-size: 8pt;
                    font-weight: bold;
                    text-align: left;
                    vertical-align: top;
                    position: absolute;
                    left: 10px;
                    width: 10%;
                }                
                .items
                {
                    font: normal 8pt verdana,arial,helvetica;
                    color: #000000;
                    text-align: left;
                    position: relative;
                    left: 10%;
                }
                .logmessage
                {
                    border: 1px solid black;
                    background-color: #eeeee0;
                    overflow: scroll;
                    font: normal 8pt Lucida Console,courier,monospace;
                    position: relative;
                    left: 10%;
                    padding: 3px;
                }
                .changedpaths
                {
                    position: relative;
                    left: 10%;
                    padding: 3px;   
                    display: none;
                }
                
                td 
                {
                    font: normal 8pt verdana,arial,helvetica;
                    color: #000000;
                    text-align: left;
                }
                
                th
                {
                    font-size: 8pt;
                    font-weight: bold;
                    text-align: left;
                    vertical-align: top;
                    width: 200px;
                }
                
                td.message
                {
                    border: 1px solid black;
                    background-color: #eeeee0;
                    overflow: scroll;
                    font: normal 8pt courier,monospace;
                }
                
                table.changedpathstable
                {
                    border: 1px solid black;
                    background-color: #cacad9;
                }
                td.path
                {
                    padding: 3px;
                    font: normal 8pt courier,monospace;
                }
                td.action
                {
                    padding: 3px;
                    width: 200px;                
                }
                
                pre
                {
                    margin: 0px;
                }               
                
                </style>
                <script type="text/javascript">
                    function toggleDiv( div )
                    {
                        var elt = document.getElementById( div );
                        
                        if ( elt.style.display=="block" ) {
                            elt.style.display="none"; 
                        }
                        else {
                            elt.style.display="block";
                        }
                    }
                </script>
                </head>
            <body>
                
                <xsl:apply-templates/>
            </body>
        </html>
    </xsl:template>
    
    <xsl:template match="/LogResult/LogItem">
        <div class="headers">Revision:</div>
        <div class="items">
            <xsl:value-of select="Revision"/>
        </div>
        
        <div class="headers">Author:</div>
        <div class="items">
            <xsl:value-of select="Author"/>
        </div>
        
        <div class="headers">Date:</div>
        <div class="items">
            <xsl:call-template name="format-date">
                <xsl:with-param name="date">
                    <xsl:value-of select="Date"/>
                </xsl:with-param>
            </xsl:call-template>
        </div>
        
                <div class="headers">Message</div>
                <div class="logmessage">
                    <xsl:call-template name="string-replace">                    
                        <xsl:with-param name="to"><br/></xsl:with-param>
                        <xsl:with-param name="from" select="'&#10;'"/>
                        <xsl:with-param name="string">
                            <xsl:call-template name="string-replace">   
                                <xsl:with-param name="string">
                                    <xsl:call-template name="string-replace">
                                        <xsl:with-param name="to" select="'&#160;&#160;&#160;&#160;'"/>
                                        <xsl:with-param name="from" select="'&#9;'"/>
                                        <xsl:with-param name="string" select="Message"/>
                                    </xsl:call-template>                                
                                </xsl:with-param>     
                                <xsl:with-param name="from" select="'&#32;'"/>
                                <xsl:with-param name="to" select="'&#160;'"/> 
                            </xsl:call-template>
                        </xsl:with-param>
                    </xsl:call-template>
                </div>
        
        
        
        <xsl:apply-templates select="ChangedPaths"/>
        
        <hr/>
        
    </xsl:template>
    
    <xsl:template match="ChangedPaths">
        <div class="headers" style="position: relative;">
            <a>                
                <xsl:attribute name="href">
                javascript:toggleDiv('<xsl:value-of select="generate-id()"/>')</xsl:attribute>
                Changed paths
            </a>
        </div>
        <div class="changedpaths">
            <xsl:attribute name="id"><xsl:value-of select="generate-id()"/>
            </xsl:attribute>
            
            <table class="changedpathstable">        
                <xsl:apply-templates select="ChangedPath"/>
            </table>
        </div>        
    </xsl:template>
    
    <xsl:template match="ChangedPath">
        <tr>            
            <td class="path">
                <xsl:value-of select="Path"/>
            </td>
            
            <td class="action">
                <xsl:value-of select="Action"/>
            </td>
        </tr>
    </xsl:template>
    
    <!-- Following two functions taken from /service/modifications.xsl in the
    Draco.NET source -->
    
    <!-- Replace all occurences of the character(s) 'from' by `to' in the string 'string'.-->
  <xsl:template name="string-replace" xml:space="default">
    <xsl:param name="string" />
    <xsl:param name="from" />
    <xsl:param name="to" />
    <xsl:choose>
      <xsl:when test="contains($string,$from)">
        <xsl:value-of select="substring-before($string,$from)" />
        <xsl:copy-of select="$to" />
        <xsl:call-template name="string-replace">
          <xsl:with-param name="string" select="substring-after($string,$from)" />
          <xsl:with-param name="from" select="$from" />
          <xsl:with-param name="to" select="$to" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$string" />
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
 
  <!-- Parses a date in UTC format, e.g. 2003-03-07T20:34:04.0000000-00:00 -->
  <xsl:template name="format-date" xml:space="default">
    <xsl:param name="date" />
    
    
   
    <!-- Month -->
    <xsl:variable name="month" select="number(substring($date, 6, 2))"/>
    <xsl:choose>
      <xsl:when test="$month=1">January</xsl:when>
      <xsl:when test="$month=2">February</xsl:when>
      <xsl:when test="$month=3">March</xsl:when>
      <xsl:when test="$month=4">April</xsl:when>
      <xsl:when test="$month=5">May</xsl:when>
      <xsl:when test="$month=6">June</xsl:when>
      <xsl:when test="$month=7">July</xsl:when>
      <xsl:when test="$month=8">August</xsl:when>
      <xsl:when test="$month=9">September</xsl:when>
      <xsl:when test="$month=10">October</xsl:when>
      <xsl:when test="$month=11">November</xsl:when>
      <xsl:when test="$month=12">December</xsl:when>
    </xsl:choose>
    <xsl:text>&#032;</xsl:text>
    
    <!-- Day -->
    <xsl:value-of select="number(substring($date, 9, 2))" />
    <xsl:text>,&#032;</xsl:text>
     
    <!-- Year -->
    <xsl:value-of select="substring($date, 1, 4)" />
    <xsl:text>&#032;-&#032;</xsl:text>
    <!-- Time -->
    <xsl:value-of select="substring($date, 12, 8)" />
  </xsl:template>
  
  
</xsl:stylesheet>

  