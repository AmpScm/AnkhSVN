<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:template match="/">
        <html>
            <head>
                <title><xsl:value-of select="doc/@title"/></title>
                <link rel="stylesheet" type="text/css" href="docstyle.css"/>
            </head>
            <body>       
                
                <xsl:apply-templates/>              
            </body>
        </html>
    </xsl:template>
    
    <xsl:template match="/doc">
        <h1><xsl:value-of select="@title"/></h1>
        <xsl:call-template name="toc"/>
        <xsl:apply-templates/>
    </xsl:template>
    
    
    <xsl:template name="toc">        
        <ul>
            <xsl:for-each select="section">
                <li><a href="#{generate-id()}"><xsl:value-of select="@header"/></a>
                <xsl:call-template name="toc"/></li>
            </xsl:for-each>
        </ul>
    </xsl:template>
        
    
    <xsl:template match="/doc/section">
        <a name="{generate-id()}"><h2><xsl:value-of select="@header"/></h2></a>
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="/doc/section/section">
        <a name="{generate-id()}"><h3><xsl:value-of select="@header"/></h3></a>
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="/doc/section/section/section">
        <a name="{generate-id()}"><h4><xsl:value-of select="@header"/></h4></a>
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="/doc/section/section/section/section">
        <a name="{generate-id()}"><h5><xsl:value-of select="@header"/></h5></a>
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="/doc/section/section/section/section/section">
        <a name="{generate-id()}"><h6><xsl:value-of select="@header"/></h6></a>
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="list">
        <ul>
            <xsl:apply-templates/>
        </ul>
    </xsl:template>
    
    <xsl:template match="list/item">
        <li><xsl:apply-templates/></li>
    </xsl:template>
    
    <xsl:template match="definitionlist">
        <ul>
            <xsl:apply-templates/>
        </ul>
    </xsl:template>
    
    <xsl:template match="definitionlist/definition">
        <li><b><xsl:value-of select="@word"/></b> - <xsl:apply-templates/></li>
    </xsl:template>
    
    <xsl:template match="code"><br/>
<pre><xsl:apply-templates/></pre>
    </xsl:template>
    
    <xsl:template match="inlinecode">
        <code><xsl:apply-templates/></code>
    </xsl:template>
    
    <xsl:template match="link">
        <a>
            <xsl:attribute name='href'>
                <xsl:value-of select="@url"/>
            </xsl:attribute>
            <xsl:value-of select="."/>
        </a>
    </xsl:template>
    
    <xsl:template match="p">
        <p><xsl:apply-templates/></p>
    </xsl:template>
    
    <xsl:template match="b">
        <b><xsl:apply-templates/></b>
    </xsl:template>
    
    <xsl:template match="i">
        <i><xsl:apply-templates/></i>
    </xsl:template> 
    
    
    
</xsl:stylesheet>

  