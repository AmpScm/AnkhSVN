<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:fo="http://www.w3.org/1999/XSL/Format"
    xmlns:py="http://arild.no-ip.com/gant">
    
    <xsl:param name="legend">2</xsl:param> 
    <xsl:variable name="defaultyear"><xsl:value-of select="/gant/defaultyear"/></xsl:variable>
    <xsl:variable name="startweek"><xsl:value-of select="/gant/start/@week"/></xsl:variable>
    <xsl:variable name="startyear"><xsl:value-of select="/gant/start/@year"/></xsl:variable>
    <xsl:variable name="endweek"><xsl:value-of select="/gant/end/@week"/></xsl:variable>
    <xsl:variable name="endyear"><xsl:value-of select="/gant/end/@year"/></xsl:variable>

    <xsl:template match="/">
        <fo:root xmlns:fo="http://www.w3.org/1999/XSL/Format">
            <fo:layout-master-set>
                <fo:simple-page-master master-name="gant"
                  page-height="21cm" 
                  page-width="29.7cm"
                  margin-top="1cm" 
                  margin-bottom="1cm" 
                  margin-left="1.5cm" 
                  margin-right="1.5cm">
                    <fo:region-body margin-top="1cm" margin-bottom="1.5cm"/>
                </fo:simple-page-master>
            </fo:layout-master-set>
            
            <fo:page-sequence master-reference="gant">
                <fo:flow flow-name="xsl-region-body">
                    <xsl:apply-templates/>                
                </fo:flow>
            
            </fo:page-sequence>
        
        </fo:root>
    </xsl:template>
    
    <xsl:template match="/gant">
        <fo:table border="solid 1pt black"
                  table-layout="fixed">
            <xsl:call-template name="column-widths"/>
            <fo:table-body>
                
                <xsl:call-template name="print-week-numbers"/>
                
                <xsl:apply-templates select="/gant/item">
                    <xsl:with-param name="level" select="0"/>
                </xsl:apply-templates>       
                
                <!--<xsl:call-template name="print-week-numbers"/>    -->
            </fo:table-body>        
        </fo:table>        
    </xsl:template>
    
    <xsl:template match="//item">
        <xsl:param name="level"/>
        <fo:table-row>
            
            <fo:table-cell padding="1.5pt" 
                vertical-align="middle"
                border-right="0.8pt solid black"
                border="solid 0.6pt black">
                <fo:block font-size="8pt">
                    <xsl:call-template name="print-spaces">
                        <xsl:with-param name="spaces">
                            <xsl:value-of select="$level * $legend"/>
                        </xsl:with-param>
                    </xsl:call-template>
                    
                    <xsl:value-of select="text"/>
                </fo:block>
            </fo:table-cell>
            
            <xsl:for-each select="py:get-week-status(number($startweek), 
                number($startyear), number($endweek), number($endyear), 
                number($defaultyear), 
                number(start/@week), number(start/@year),
                number(end/@week), number(end/@year) )">
                <fo:table-cell border="solid 0.2pt black">  
                    <xsl:choose>
                        <xsl:when test=".='busy'">
                            <xsl:attribute name="background-color">red</xsl:attribute>
                        </xsl:when>
                    </xsl:choose>  
                    &#160;                                                     
                </fo:table-cell>
                                        
            </xsl:for-each>
            
            
        </fo:table-row>
        
        <xsl:apply-templates select="./item">
            <xsl:with-param name="level" select="$level + 1"/>
        </xsl:apply-templates>
        
    </xsl:template>
    
    <xsl:template name="column-widths">
        <fo:table-column column-width="8cm"/>
        <fo:table-column column-width="0.52cm">
            <xsl:attribute name="number-columns-repeated">
                <xsl:value-of select="count(py:get-week-numbers( number($startweek), 
                                    number($startyear), number($endweek), number($endyear), 
                                    number($defaultyear)))"/>
            </xsl:attribute>
        </fo:table-column>     
            
    </xsl:template>
    
    <xsl:template name="print-spaces">
        <xsl:param name="spaces"/>
        <xsl:if test="$spaces > 0">
            &#160;
            <xsl:call-template name="print-spaces">
                <xsl:with-param name="spaces" select="$spaces - 1"/>
            </xsl:call-template>            
        </xsl:if>  
    </xsl:template>
    
    <xsl:template name="print-week-numbers">
        <fo:table-row>
            <fo:table-cell padding="2pt">
                <fo:block   font-size="7pt"
                            font-weight="bold">Item</fo:block>
            </fo:table-cell>
            <xsl:for-each 
                select="py:get-week-numbers( number($startweek), number($startyear), number($endweek), number($endyear), number($defaultyear))">
                <fo:table-cell  border="solid 0.2pt black"
                                border-bottom="solid 1pt black" 
                                text-align="center"
                                vertical-align="middle"
                                padding="2pt">                    
                    <fo:block font-size="7pt"><xsl:value-of select="."/></fo:block>
                </fo:table-cell>
            </xsl:for-each>
        </fo:table-row>
        
    </xsl:template>
    
</xsl:stylesheet>