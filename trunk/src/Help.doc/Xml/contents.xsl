<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output method="html"/>

    <xsl:template match="style">
        <xsl:attribute name='href'>
            <xsl:value-of select="@url"/>
        </xsl:attribute>
    </xsl:template>

    <xsl:template match="/doc">
        <h1><xsl:value-of select="@title"/></h1>
       <!-- <xsl:call-template name="toc"/> -->
        <xsl:apply-templates select="section">
            <xsl:with-param name="level" select="1"/>
        </xsl:apply-templates>
    </xsl:template>
        
    <xsl:template match="section">
        <xsl:param name="level"/>
        
        <a name="{generate-id()}">
            <xsl:element name="{concat( 'h', $level+1 )}">
                <xsl:value-of select="@header"/>
            </xsl:element>
        </a>
        
        <xsl:apply-templates>
            <xsl:with-param name="level" select="$level+1"/>
        </xsl:apply-templates>
        
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
    
    <xsl:template match="img">
        <img>
            <xsl:attribute name='src'>
                <xsl:value-of select="@src"/>
            </xsl:attribute>
            
            <xsl:if test="@align">
                <xsl:attribute name='align'>
                    <xsl:value-of select="@align"/>
                </xsl:attribute>
            </xsl:if>
            
            <xsl:if test="@hspace">
                <xsl:attribute name='hspace'>
                    <xsl:value-of select="@hspace"/>
                </xsl:attribute>
            </xsl:if>
            
            <xsl:if test="@vspace">
                <xsl:attribute name='vspace'>
                    <xsl:value-of select="@vspace"/>
                </xsl:attribute>
            </xsl:if>

        </img>
    </xsl:template>
    
    <xsl:template match="p">
        <p><xsl:apply-templates/></p>
    </xsl:template>
    
    <xsl:template match="br">
        <br>
            <xsl:if test="@clear">
                <xsl:attribute name='clear'>
                    <xsl:value-of select="@clear"/>
                </xsl:attribute>
            </xsl:if>
            <xsl:apply-templates/>
        </br>
    </xsl:template>
    
    <xsl:template match="table">
        <table><xsl:apply-templates select="tr"/></table>
    </xsl:template>
    
    <xsl:template match="tr">
        <tr><xsl:apply-templates select="td"/></tr>
    </xsl:template>
    
    <xsl:template match="td">
        <td><xsl:apply-templates/></td>
    </xsl:template>
    
    <xsl:template match="b">
        <b><xsl:apply-templates/></b>
    </xsl:template>
    
    <xsl:template match="i">
        <i><xsl:apply-templates/></i>
    </xsl:template> 
    
    <xsl:template match="hr">
        <hr><xsl:apply-templates/></hr>
    </xsl:template> 
</xsl:stylesheet>

  
