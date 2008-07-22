<?xml version="1.0"?>
<xsl:stylesheet version="1.0"
      xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:x="inline-doc"
                exclude-result-prefixes="x msxsl">
  <msxsl:script implements-prefix="x" language="c#">
    public string DateTitle()
    {
      return (DateTime.Now - new TimeSpan(9,0,0)).ToString("yyyy-MM-dd");
    }
    
    public string MakeMessage(string msg)
    {
      if (string.IsNullOrEmpty(msg))
        return null;
        
      System.Text.RegularExpressions.Match m
        = System.Text.RegularExpressions.Regex.Match(msg, "^\\s*\\*\\s+", RegexOptions.Multiline);
      
      if(m.Success)
        msg = msg.Substring(0, m.Index);
        
      msg = msg.Trim();
      
      return msg;
    }
  </msxsl:script>
  <xsl:param name="latestUrl">http://ankhsvn.net/daily/</xsl:param>
  <xsl:param name="latestVersion">2.0.0000.0</xsl:param>
  <xsl:param name="latestName">AnkhSVN-Daily-2.0.0000.0.msi</xsl:param>
  <xsl:param name="buildRev">4567</xsl:param>

  <xsl:output media-type="text/html" method="html" indent="no"/>
  <xsl:template match="/">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <xsl:text>&#13;&#10;</xsl:text>
        <title>AnkhSVN-Daily Update - <xsl:value-of select="$latestVersion"/></title>
        <xsl:text>&#13;&#10;</xsl:text>
        <link rel="Stylesheet" type="text/css" href="daily.css" />
      </head>
      <body>
        <xsl:text>&#13;&#10;</xsl:text>
        <h1>AnkhSVN-Daily Update - <xsl:value-of select="$latestVersion"/></h1>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <p>
          The AnkhSVN project provides Daily builds of the current development tree. These versions
          are not tested, but may contain bugfixes (and new bugs) that are not in released versions. If
          you are testing these daily builds (Thanks!) and find issues in it, please report those issues
          with the exact version.
        </p>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <h2>The latest daily build is <a href="{$latestUrl}"><xsl:value-of select="$latestName"/></a></h2>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <p>
          You can find older daily releases in the <a href="http://ankhsvn.net/daily/?all=1">daily folder</a> of documents &amp; files.
          The previous version of this report is still available in our <a href="http://ankhsvn.net/daily/?id={$buildRev}">Subversion Repository</a>.
        </p>
        <p>Published daily builds will be available for at least a week after they are build.</p>        
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:text>&#13;&#10;</xsl:text>
        <h2>Recent changes</h2>
        <xsl:text>&#13;&#10;</xsl:text>
        <table>
          <xsl:text>&#13;&#10;</xsl:text>
          <xsl:apply-templates select="/log/logentry" mode="days" />
          <xsl:text>&#13;&#10;</xsl:text>
        </table>
        <xsl:text>&#13;&#10;</xsl:text>
        <xsl:comment>/Recent changes</xsl:comment>
        <xsl:text>&#13;&#10;</xsl:text>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="logentry" mode="days">
    <xsl:variable name="me" select="." />
    <xsl:variable name="myDate" select="substring-before(date,'T')"/>
    <xsl:if test="@revision = ../logentry[substring-before(date,'T') = $myDate][1]/@revision">      
      <tr xmlns="http://www.w3.org/1999/xhtml" class="d">
        <td colspan="4">
          <xsl:value-of select="$myDate"/>
        </td>        
      </tr>
      <xsl:text>&#13;&#10;</xsl:text>
      <xsl:apply-templates select="../logentry[substring-before(date,'T') = $myDate]"/>
    </xsl:if>
  </xsl:template>
  <xsl:template match="logentry">
    <xsl:if test="x:MakeMessage(msg)">
    <tr xmlns="http://www.w3.org/1999/xhtml">
      <td>
        <xsl:value-of select="' '"/>
      </td>
      <td class="r">
        <a id="{@revision}" href="http://ankhsvn.net/rev/?r={@revision}">[r<xsl:value-of select="@revision"/>]</a>
      </td>
      <td class="a">
        <xsl:value-of select="author"/>
      </td>
      <td class="f">
        <xsl:for-each select="paths/path">
          <xsl:if test=".!=../path[1]">
            <xsl:value-of select="', '"/>
          </xsl:if>
          <xsl:value-of select="substring-after(., '/trunk/')"/>
        </xsl:for-each>
      </td>
    </tr>
    <tr xmlns="http://www.w3.org/1999/xhtml">
      <td colspan="2">
        <xsl:value-of select="' '"/>
      </td>
      <td class="l" colspan="2">
        <xsl:value-of select="x:MakeMessage(msg)"/>
      </td>
    </tr>
    <xsl:text>&#13;&#10;</xsl:text>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>