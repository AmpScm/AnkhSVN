<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html"/>
    
    <xsl:template match="HelpTOC">
        <xsl:call-template name="toc" />
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
