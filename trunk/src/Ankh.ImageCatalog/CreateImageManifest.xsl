<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://schemas.microsoft.com/VisualStudio/ImageManifestSchema/2014"
                xmlns:img="http://schemas.ankhsvn.net/2016/05/images/"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl img">
    <xsl:output method="xml" indent="yes"/>

    <xsl:template match="/img:VsImages">
      <ImageManifest>
        <xsl:comment>Generated file; please edit the original file instead of this generated file</xsl:comment>
        <Symbols>
          <String Name="Resource" Value="/Ankh.ImageCatalog"/>
        </Symbols>
        <Images>
          
        </Images>
      </ImageManifest>
    </xsl:template>
  
    <xsl:template match="@* | node()">
        <xsl:copy>
            <xsl:apply-templates select="@* | node()"/>
        </xsl:copy>
    </xsl:template>
</xsl:stylesheet>
