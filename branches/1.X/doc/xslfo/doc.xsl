<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:fo="http://www.w3.org/1999/XSL/Format">

    <xsl:template match="/">
        <fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">
            <fo:layout-master-set>
                <fo:simple-page-master master-name="doc"
                  page-height="29.7cm" 
                  page-width="21cm"
                  margin-top="1cm" 
                  margin-bottom="1cm" 
                  margin-left="2.2cm" 
                  margin-right="2.2cm">
                    <fo:region-body margin-top="1cm" margin-bottom="2.5cm"/>
                    <fo:region-before extent="3cm"/>
                    <fo:region-after extent="1cm"/>
                </fo:simple-page-master>
            </fo:layout-master-set>
            
            <fo:page-sequence master-reference="doc">
            
                <fo:static-content flow-name="xsl-region-after">                    
                    <fo:block font-size="7pt" text-align="end">
                            <fo:page-number/>
                     </fo:block>
                </fo:static-content>
                
                <fo:flow flow-name="xsl-region-body">
                    <xsl:apply-templates/>                
                </fo:flow>
            
            </fo:page-sequence>
            
            
        </fo:root>
    </xsl:template>
    
    <xsl:template match="/doc">
        <fo:block   font-size="20pt"
                    font-weight="bold"
                    font-family="sans-serif"
                    space-after.optimum="18pt"
                    keep-with-next="always"
                    text-align="center">
            <xsl:value-of select="@title"/>
        </fo:block> 
        
        <xsl:apply-templates>
            <xsl:with-param name="level" select="1"/>
        </xsl:apply-templates>                
    </xsl:template>
    
    <xsl:template match="section">
        
        <fo:block   font-family="sans-serif"
                    font-size="15pt"
                    font-style="italic"
                    font-weight="bold"
                    space-before.optimum="8pt"
                    space-after.optimum="5pt"
                    keep-with-next="always"
                    text-align="left">
            <xsl:value-of select="@header"/>
        </fo:block>
        
        <xsl:apply-templates/>
        
    </xsl:template>
    
    <xsl:template match="section/section">
        <fo:block   font-family="sans-serif"
                    font-size="13pt"
                    font-weight="bold"
                    space-before.optimum="5pt"
                    space-after.optimum="5pt"
                    keep-with-next="always"
                    text-align="left">
            <xsl:value-of select="@header"/>
        </fo:block>
        
        <xsl:apply-templates/>
    </xsl:template>
    
    <xsl:template match="section/section/section">
        <fo:block   font-family="sans-serif"
                    font-size="12pt"
                    font-weight="bold"
                    space-before.optimum="8pt"
                    keep-with-next="always"
                    break-before="page"
                    space-after.optimum="5pt"
                    text-align="left">
            <xsl:value-of select="@header"/>
        </fo:block>
        
        <xsl:apply-templates/>
    </xsl:template>
    
    
    
    <xsl:template match="p">
        <fo:block   font-family="Times"
                    text-align="justify"
                    space-after.optimum="7pt"
                    font-size="12pt">
            <xsl:apply-templates/>        
        </fo:block>
    </xsl:template>
    
    <xsl:template match="signature">
        <fo:block   font-family="Times"
                    text-align="center"
                    space-after.optimum="7pt"
                    space-before.optimum="18pt"
                    font-size="9pt">
            <xsl:apply-templates/>        
        </fo:block>
    </xsl:template>
    
    <xsl:template match="b">
        <fo:inline font-weight="bold"><xsl:apply-templates/></fo:inline>
    </xsl:template>
    
    <xsl:template match="i">
        <fo:inline font-style="italic"><xsl:apply-templates/></fo:inline>
    </xsl:template>
    
    <xsl:template match="list">
        <fo:list-block space-after.optimum="7pt">
            <xsl:apply-templates/>
        </fo:list-block>
    </xsl:template>
    
    <xsl:template match="list/item">
        <fo:list-item>
            <fo:list-item-label end-indent="label-end()">
                <fo:block>&#x2022;</fo:block>
            </fo:list-item-label>
            <fo:list-item-body start-indent="body-start()">
                <fo:block><xsl:apply-templates/></fo:block>
            </fo:list-item-body>
        </fo:list-item>
        
    </xsl:template>
    
    <xsl:template match="code">
        <fo:block font-family="Courier"
                  white-space-collapse="false"
                  space-before.optimum="8pt"
                  space-after.optimum="8pt">
            <xsl:apply-templates/>            
        </fo:block>
    </xsl:template>
    

</xsl:stylesheet>
