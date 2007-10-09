<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" indent="yes"/>

    <xsl:template match ="/">
        <html>
            <head>            
                <title>Blame</title>
                <style type="text/css">
                td
                {
                    height: 5px;
                    font-size: 10pt;                    
                }
                pre
                {
                    margin: 0px;
                }
                td.author
                {
                    background-color: AliceBlue;
                    text-align: center;
                }
                td.line
                {
                    background-color: GhostWhite;
                    font-family: 'lucida console', monospace;
                }
                td.revision
                {
                    background-color: PapayaWhip;
                    padding: 2px;
                    text-align: center;
                }
                table#main
                {
                    align: center;
                    border: 1px solid black;
                }
                </style>
            </head>
            <body>
                <table border="0" id="main">
                    <xsl:apply-templates/>
                </table>
            </body>
        </html>
    </xsl:template>
    
    <xsl:template match="/BlameResult/Blame">
        <tr>
            <td class="author"><xsl:value-of select="Author"/></td>
            <td class="revision"><xsl:value-of select="Revision"/></td>
            <td class="line"><pre><xsl:value-of select="Line"/></pre></td>
        </tr>
    </xsl:template>
</xsl:stylesheet>

  