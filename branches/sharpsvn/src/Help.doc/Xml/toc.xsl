<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:import href="contents.xsl"/>
<xsl:output method="html"/>
   
   <xsl:template name="toc">        
        <ul>
            <xsl:for-each select="section">
                <li><a href="#{generate-id()}"><xsl:value-of select="@header"/></a>
                    <xsl:call-template name="toc"/>
                </li>
            </xsl:for-each>
        </ul>
       <xsl:apply-imports/>              
   </xsl:template>
  </xsl:stylesheet>

  
