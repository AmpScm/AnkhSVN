<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html"/>
    <xsl:import href="contents.xsl"/>
    <xsl:template match="/">
        <html>
            <head>
                <title><xsl:value-of select="doc/@title"/></title>
                <link rel="stylesheet" type="text/css" href="docstyle.css"/>
            </head>
            <body>       
                
                <xsl:apply-imports/>              
            </body>
        </html>
    </xsl:template>   
</xsl:stylesheet>

  
