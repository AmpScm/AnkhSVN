<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html"/>
    
    <xsl:template match="HelpTOC">
    <html>
    <head>
    <style type="text/css">
        BODY
        {
            font-size: 10pt;
            font-family: 'Arial Narrow';
        }
        ul 
        {
            list-style-type: none;
            list-style-image: url("img/arrow.gif")
        }
    </style>
    </head>
        <body>
            <img src="img/logo(250x68).png"/>
            <xsl:call-template name="toc" />
        </body>
    </html>
    </xsl:template>
    <xsl:template name="toc">
        <ul>
            <xsl:for-each select ="HelpTOCNode">
                <li>
                    <a target='file'>
                        <xsl:attribute name="href">
                            <xsl:value-of select="substring( @Url, 2, string-length(@Url)-1 )"/>
                        </xsl:attribute>
                        <xsl:value-of select="@Title"/>
                    </a>
                    <xsl:call-template name="toc"/>
                 </li>
            </xsl:for-each>
        </ul>
    </xsl:template>
</xsl:stylesheet>
