<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://schemas.microsoft.com/VisualStudio/ImageManifestSchema/2014"
                xmlns:img="http://schemas.ankhsvn.net/2016/05/images/"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:me="local-script" exclude-result-prefixes="msxsl img me">

  <xsl:param name="Src" select="'F:\ankhsvn\trunk-2\src\ankh.package\gui\AnkhSvn.xml'" />
  <xsl:param name="Configuration" select="'Debug'" />

  <msxsl:script implements-prefix="me" language="C#">
    <msxsl:assembly name="System.Drawing" />
    <msxsl:using namespace="System.Collections.Generic" />
    <msxsl:using namespace="System.Drawing" />
    <msxsl:using namespace="System.Drawing.Imaging" />
    <msxsl:using namespace="System.Reflection" />
    <msxsl:using namespace="System.Runtime.InteropServices" />
    <msxsl:using namespace="System.IO" />
    <![CDATA[	
      const string ImgMnfNs = "http://schemas.microsoft.com/VisualStudio/ImageManifestSchema/2014";

  // http://msdn2.microsoft.com/en-us/library/system.enum.getunderlyingtype.aspx
  static object GetAsUnderlyingType(Enum enval)
  {
    Type entype = enval.GetType();

    Type undertype = Enum.GetUnderlyingType(entype);

    return Convert.ChangeType(enval, undertype);
  }
  
  Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>(StringComparer.OrdinalIgnoreCase);
  Type LoadType(string from, string type, string sourceFile, string configuration)
  {
    if(string.IsNullOrEmpty(sourceFile))
      throw new ArgumentNullException("sourceFile");
    
    string srcPath = Path.GetDirectoryName(Path.GetFullPath(sourceFile));

    from = System.IO.Path.Combine(srcPath, from);
    
    if(from.Contains("$(Configuration)"))
    {
      if(string.IsNullOrEmpty(configuration))
        throw new InvalidOperationException("Configuration not set");
        
      string altFrom = from.Replace("$(Configuration)", configuration);
      
      from = altFrom;
    }
    
    from = Path.GetFullPath(from);
    
    if(!File.Exists(from))
      throw new FileNotFoundException(string.Format("Import {0} missing", from));
    
    Assembly a;
    
    if (!_assemblies.TryGetValue(from, out a))
    {
      AssemblyName name = AssemblyName.GetAssemblyName(from);
      a = Assembly.Load(System.IO.File.ReadAllBytes(from));
      _assemblies[from] = a;
    }
    
    Type tp = a.GetType(type);
    
    if(tp == null)
      throw new InvalidOperationException(string.Format("Class {0} not found in assembly: {1}", type, from));

    return tp;
  }
  
  Dictionary<string,string> _idMap = new Dictionary<string,string>();
  
  public XPathNodeIterator InputSymbol(string type, string from, string prefix, string sourceFile, string configuration)
  {
    XmlDocument doc = new XmlDocument();
    XmlElement de = doc.CreateElement("doc", ImgMnfNs);
    doc.AppendChild(de);
    
    Type tp = LoadType(from, type, sourceFile, configuration);
    
    GuidAttribute typeAttr = null;
    object[] attrs = tp.GetCustomAttributes(typeof(GuidAttribute), true);
    
    if(attrs.Length > 0)
      typeAttr = (GuidAttribute)attrs[0];

    de.AppendChild(doc.CreateComment(string.Format("Type: {0}", tp.AssemblyQualifiedName)));
    
    string typeName = tp.FullName.Replace('.','_');
    if (typeAttr != null)
    {
      XmlElement parent = de;
      parent = doc.CreateElement("Guid", ImgMnfNs);
      de.AppendChild(parent);
      parent.SetAttribute("Name", typeName);
      parent.SetAttribute("Value", new Guid(typeAttr.Value).ToString("B"));
    }
    
    foreach(System.Reflection.FieldInfo fif in tp.GetFields(BindingFlags.Public | System.Reflection.BindingFlags.Static))
    {
      object v = fif.GetValue(null);
      
      if(v == null)
        continue;
        
      if(fif.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length > 0)
        continue;
      
      GuidAttribute itemAttr = typeAttr;
      attrs = fif.GetCustomAttributes(typeof(GuidAttribute), true);
      
      if(attrs.Length > 0)
        typeAttr = (GuidAttribute)attrs[0];
      
      Type vType = v.GetType();
      if(vType.IsEnum)
      {
        v = GetAsUnderlyingType((Enum)v);
        
        if(itemAttr != null)
        {
          _idMap[prefix+fif.Name] = typeName + ':' + prefix + fif.Name;
        }
        
        XmlElement idSymbol = doc.CreateElement("ID", ImgMnfNs);
        de.AppendChild(idSymbol);
        
        idSymbol.SetAttribute("Name", prefix + fif.Name);
        idSymbol.SetAttribute("Value", string.Format("{0}", v));
      }
      else if(v is Guid)
      {
        XmlElement guidSymbol = doc.CreateElement("Guid", ImgMnfNs);
        de.AppendChild(guidSymbol);
        guidSymbol.SetAttribute("Name", prefix + fif.Name);
        guidSymbol.SetAttribute("Value", ((Guid)v).ToString("B"));
      }
    }

    return doc.CreateNavigator().Select("/*/node()");
  }
]]>
  </msxsl:script>

  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="/img:VsImages">
    <ImageManifest>
      <xsl:comment>Generated file; please edit the original file instead of this generated file</xsl:comment>
      <Symbols>
        <String Name="Resource" Value="/Ankh.ImageCatalog;Component/"/>
        <xsl:apply-templates select="img:Imports/img:Import[not (@include)]" mode="include" />
      </Symbols>
      <Images>
        <xsl:apply-templates select="img:Images//img:Image"/>
      </Images>
    </ImageManifest>
  </xsl:template>

  <!--xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template-->

  <xsl:template match="img:Image">
    <Image
      Guid="$({ancestor-or-self::img:*/@guid})"
      ID="$({@id})"
      >
      <xsl:if test="@res">
        <Source Uri="$(Resource){@res}">
          <Size Value="16" />
        </Source>
      </xsl:if>
      <xsl:apply-templates select="img:Source" />
    </Image>
  </xsl:template>

  <xsl:template match="img:Import[@type]" mode="include">
    <xsl:copy-of select="me:InputSymbol(@type, @from, @prefix, $Src, $Configuration)"/>
  </xsl:template>
</xsl:stylesheet>
