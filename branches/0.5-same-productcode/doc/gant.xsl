<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:py="http://cube.iu.hio.no">
    <xsl:output method="html"/>
    
    <xsl:variable name="legend">20</xsl:variable> 
    <xsl:variable name="cellwidth">20</xsl:variable>
    <xsl:variable name="cellheight">20</xsl:variable>
    
    <xsl:variable name="defaultyear"><xsl:value-of select="/gant/defaultyear"/></xsl:variable>
    <xsl:variable name="startweek"><xsl:value-of select="/gant/start/@week"/></xsl:variable>
    <xsl:variable name="startyear"><xsl:value-of select="/gant/start/@year"/></xsl:variable>
    <xsl:variable name="endweek"><xsl:value-of select="/gant/end/@week"/></xsl:variable>
    <xsl:variable name="endyear"><xsl:value-of select="/gant/end/@year"/></xsl:variable>
    
    
    <xsl:template match="/gant">
        <xsl:param name="level"/>
        <style>
            
        </style>
        <table>
            <tr>
                <td colspan="2" align="center"><xsl:value-of select="title"/></td>
            </tr>
            <tr>
                <td style="background-color: fuchsia;">Item</td>
                <td>
                    <table>
                        <tr>
                            <xsl:for-each 
                                select="py:get-week-numbers( number($startweek), number($startyear), number($endweek), number($endyear), number($defaultyear))">
                                <td align="center">
                                    <xsl:attribute name="width"><xsl:value-of select="$cellwidth"/></xsl:attribute>
                                    
                                    <xsl:value-of select="."/>
                                </td>
                            </xsl:for-each>
                        </tr>
                    </table>
                </td>
            </tr>            
            <xsl:apply-templates select="/gant/item">
                <xsl:with-param name="level" select="0"/>
            </xsl:apply-templates>
            
        </table>
    </xsl:template>
    
    <xsl:template match="//item">
        <xsl:param name="level"/>
        <tr>
            <td>
                <table>
                    <tr>
                        <td>
                            <xsl:attribute name="width"><xsl:value-of select="$level * $legend"/></xsl:attribute>  
                        </td>
                        <td>
                            <xsl:attribute name="width"><xsl:value-of select="(8-$level) * $legend"/></xsl:attribute>
                            <xsl:value-of select="text"/>
                        </td>
                    </tr>
                </table>
            </td>
            
            <td>
                <table>
                    <tr><td>
                        <xsl:for-each select="py:get-week-status(number($startweek), 
                        number($startyear), number($endweek), number($endyear), 
                        number($defaultyear), 
                        number(start/@week), number(start/@year),
                        number(end/@week), number(end/@year) )">
                            <td>   
                                <xsl:attribute name="width"><xsl:value-of select="$cellwidth"/></xsl:attribute>
                                <xsl:attribute name="height"><xsl:value-of select="$cellheight"/></xsl:attribute>
                                <xsl:if test="starts-with( ., 'busy')">
                                        <xsl:attribute name="style">background-color: fuchsia;</xsl:attribute>
                                </xsl:if>
                                                                
                            </td>
                                                    
                        </xsl:for-each>
                    </td></tr>
                </table>
            </td>
            
        </tr>
        
        <xsl:apply-templates select="./item">
            <xsl:with-param name="level" select="$level + 1"/>
        </xsl:apply-templates>
        
    </xsl:template>
    

</xsl:stylesheet>

  