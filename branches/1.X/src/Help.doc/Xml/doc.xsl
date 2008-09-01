<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:import href="contents.xsl"/>
<xsl:output method="html"/>
    <xsl:template match="/">
        <html>
            <head>
                <LINK href="../helpstyle.css" rel="stylesheet" type="text/css" />
                <title><xsl:value-of select="doc/@title"/></title>
            </head>
            <body>       
                <xsl:apply-imports/>              
            </body>
        </html>
    </xsl:template>   
</xsl:stylesheet>

  
