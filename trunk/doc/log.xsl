<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html" />
    <xsl:template match="/">
        <html>
            <style type="text/css">
                body 
                {
                    font: normal 8pt verdana,arial,helvetica;
                    color: #000000;
                }
                table tr th, table tr td {
                    font-size: 8pt;
                    text-align: left;
                    background:#a6caf0;
                }
                .details
                {
                    font-family: Lucida Console, monospace;
                    font-size: 8pt;
                }
                h1 
                {
                    margin: 0px 0px 5px;
                    font: 14pt verdana,arial,helvetica;
                }
            </style>
            <head>
                <title>Log</title>
            </head>
            <body>
                <h1>Logg messages:</h1>
                <hr />
                <xsl:apply-templates />
            </body>
        </html>
    </xsl:template>
    <xsl:template match="logentry">
        <table border="0" cellpadding="3" cellspacing="2" width="100%">
            <col />
            <col width="90%" />
            <th>Revision:</th>
            <td>
                <xsl:value-of select="@revision" />
            </td>
            <tr />
            <th>Author:</th>
            <td>
                <xsl:value-of select="author" />
            </td>
            <tr />
            <th>Date:</th>
            <td>
                <xsl:call-template name="format-date">
                    <xsl:with-param name="date" select="date" />
                </xsl:call-template>
            </td>
        </table>
        <br/>
        <span class="details">
            <xsl:call-template name="string-replace">
                <xsl:with-param name="string" select="msg" />
                <xsl:with-param name="from" select="'&#13;'" />
                <xsl:with-param name="to">
                <br />
                </xsl:with-param>
            </xsl:call-template>
        </span>
        <hr />
    </xsl:template>
    <xsl:template name="string-replace">
        <xsl:param name="string" />
        <xsl:param name="from" />
        <xsl:param name="to" />
        <xsl:choose>
            <xsl:when test="contains($string,$from)">
                <xsl:value-of select="substring-before($string,$from)" />
                <xsl:copy-of select="$to" />
                <xsl:call-template name="string-replace">
                    <xsl:with-param name="string" select="substring-after($string,$from)" />
                    <xsl:with-param name="from" select="$from" />
                    <xsl:with-param name="to" select="$to" />
                </xsl:call-template>
            </xsl:when>
            <xsl:otherwise>
                <xsl:value-of select="$string" />
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    <xsl:template name="format-date">
        <xsl:param name="date" />
        <!-- Day --><xsl:value-of select="'&#32;'" />
        <xsl:value-of select="number(substring($date, 9, 2))" />
        <xsl:text></xsl:text>
        <!-- Month --><xsl:value-of select="'&#32;'" />
        <xsl:variable name="month" select="number(substring($date, 6, 2))" />
        <xsl:choose>
            <xsl:when test="$month=1">January</xsl:when>
            <xsl:when test="$month=2">February</xsl:when>
            <xsl:when test="$month=3">March</xsl:when>
            <xsl:when test="$month=4">April</xsl:when>
            <xsl:when test="$month=5">May</xsl:when>
            <xsl:when test="$month=6">June</xsl:when>
            <xsl:when test="$month=7">July</xsl:when>
            <xsl:when test="$month=8">August</xsl:when>
            <xsl:when test="$month=9">September</xsl:when>
            <xsl:when test="$month=10">October</xsl:when>
            <xsl:when test="$month=11">November</xsl:when>
            <xsl:when test="$month=12">December</xsl:when>
        </xsl:choose>
        <xsl:text></xsl:text>
        <!-- Year --><xsl:value-of select="'&#32;'" />
        <xsl:value-of select="substring($date, 1, 4)" />
        <xsl:text></xsl:text>
        <!-- Time --><xsl:value-of select="'&#32;'" />
        <xsl:value-of select="substring($date, 12, 8)" />
    </xsl:template>
</xsl:stylesheet>
