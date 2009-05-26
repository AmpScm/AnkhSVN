<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://schemas.studioturtle.net/2007/01/gui/"
                xmlns:gui="http://schemas.studioturtle.net/2007/01/gui/"
                xmlns:task="http://schemas.studioturtle.net/2006/12/layout-task"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:me="local-script">
  <xsl:param name="Src" select="'F:\ankhsvn\trunk-2\src\ankh.package\gui\AnkhSvn.xml'" />
  <xsl:param name="Configuration" select="'Debug'" />
  <xsl:param name="BitmapFile" select="'../obj/CtCBitmap.bmp'" />
  <msxsl:script implements-prefix="me" language="C#">
    <msxsl:assembly name="System.Drawing" />
    <msxsl:using namespace="System.Collections.Generic" />
    <msxsl:using namespace="System.Drawing" />
    <msxsl:using namespace="System.Drawing.Imaging" />
    <msxsl:using namespace="System.Reflection" />
    <msxsl:using namespace="System.Runtime.InteropServices" />
    <msxsl:using namespace="System.IO" />
    <![CDATA[	
    
  const string VsctNs = "http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable";
  public string UpperFirst(string value)
  {
    if(string.IsNullOrEmpty(value))
      return "";
    else
      return char.ToUpper(value[0]) + value.Substring(1);
  }   

  public string CQuote(string value)
  {
    value = value ?? "";
    System.Text.StringBuilder sb = new System.Text.StringBuilder();

    sb.Append('\"');

    for(int i = 0; i < value.Length; i++)
    {
      if("\"\\\'".IndexOf(value[i]) >= 0)
      sb.Append('\\');

      sb.Append(value[i]);
    }

    sb.Append('\"');

    return sb.ToString();
  }

  // http://msdn2.microsoft.com/en-us/library/system.enum.getunderlyingtype.aspx
  static object GetAsUnderlyingType(Enum enval)
  {
    Type entype = enval.GetType();

    Type undertype = Enum.GetUnderlyingType(entype);

    return Convert.ChangeType( enval, undertype );
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
      
      //if(System.IO.File.Exists(altFrom))
        from = altFrom;
    }
    
    from = Path.GetFullPath(from);
    
    if(!File.Exists(from))
    {
      throw new FileNotFoundException(string.Format("Import {0} missing", from));
      //return string.Format(" // Ignored loading inputtype; from={0}\n\n", from);
    }
    
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
  
  public string InputType(string type, string from, string prefix, string sourceFile, string configuration)
  {
    StringBuilder sb = new StringBuilder();
  
    Type tp = LoadType(from, type, sourceFile, configuration);
    
    GuidAttribute typeAttr = null;
    object[] attrs = tp.GetCustomAttributes(typeof(GuidAttribute), true);
    
    if(attrs.Length > 0)
      typeAttr = (GuidAttribute)attrs[0];

    sb.AppendFormat("\n // Imported from: {0}", from);
    sb.AppendFormat("\n // Type: {0}\n", tp.AssemblyQualifiedName);
    
    string typeName = tp.FullName.Replace('.','_');
    if (typeAttr != null)
      sb.AppendFormat("#define {0} {1}\n", typeName, formatGuid(typeAttr.Value.ToString()));
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
        
        string val = "";
        if(itemAttr != null)
        {
          val = formatGuid(itemAttr.Value.ToString()) + ":";
          _idMap[prefix+fif.Name] = typeName + ':' + prefix + fif.Name;
        }

        sb.AppendFormat("#define {0}{1} 0x{2:X}\n", prefix, fif.Name, v);
      }
      else if(v is Guid)
      {
        Guid g = (Guid)v;

        sb.AppendFormat("#define {0}{1} {2}\n", prefix, fif.Name, formatGuid(g.ToString()));
      }
    }

    sb.AppendFormat("// /Loaded\n", tp.AssemblyQualifiedName);

    return sb.ToString();
  }
  
  public XPathNodeIterator InputSymbol(string type, string from, string prefix, string sourceFile, string configuration)
  {
    XmlDocument doc = new XmlDocument();
    XmlElement de = doc.CreateElement("doc", VsctNs);
    doc.AppendChild(de);
    XmlElement parent = de;
    
    Type tp = LoadType(from, type, sourceFile, configuration);
    
    GuidAttribute typeAttr = null;
    object[] attrs = tp.GetCustomAttributes(typeof(GuidAttribute), true);
    
    if(attrs.Length > 0)
      typeAttr = (GuidAttribute)attrs[0];

    de.AppendChild(doc.CreateComment(string.Format("Type: {0}", tp.AssemblyQualifiedName)));
    
    string typeName = tp.FullName.Replace('.','_');
    if (typeAttr != null)
    {
      parent = doc.CreateElement("GuidSymbol", VsctNs);
      de.AppendChild(parent);
      parent.SetAttribute("name", typeName);
      parent.SetAttribute("value", new Guid(typeAttr.Value).ToString("B"));
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
        
        string val = "";
        if(itemAttr != null)
        {
          val = formatGuid(itemAttr.Value.ToString()) + ":";
          _idMap[prefix+fif.Name] = typeName + ':' + prefix + fif.Name;
        }
        
        XmlElement idSymbol = doc.CreateElement("IDSymbol", VsctNs);
        parent.AppendChild(idSymbol);
        
        idSymbol.SetAttribute("name", prefix + fif.Name);
        idSymbol.SetAttribute("value", string.Format("0x{0:X}", v));
      }
      else if(v is Guid)
      {
        XmlElement guidSymbol = doc.CreateElement("GuidSymbol", VsctNs);
        parent.AppendChild(guidSymbol);
        guidSymbol.SetAttribute("name", prefix + fif.Name);
        guidSymbol.SetAttribute("value", ((Guid)v).ToString("B"));
      }
    }

    return doc.CreateNavigator().Select("/*/node()");
  }
  
  public string MakeId(string value)
  {
    string v;
    
    if (_idMap.TryGetValue(value, out v))
      return v;

    return value;
  }
  
  public string formatGuid(string v)
  {
    v = v.ToUpperInvariant();
    return "{ 0x" + v.Substring(0,8) + ", 0x" + v.Substring(-1+10,4) + 
                  ", 0x" + v.Substring(-1+15,4) + ", { 0x" + v.Substring(-1+ 20,2) + 
                  ", 0x" + v.Substring(-1+22,2) + ", 0x" + v.Substring(-1+ 25,2) +
                  ", 0x" + v.Substring(-1+27,2) + ", 0x" + v.Substring(-1+ 29,2) +
                  ", 0x" + v.Substring(-1+31,2) + ", 0x" + v.Substring(-1+ 33,2) +
                  ", 0x" + v.Substring(-1+35,2) + " } }";
  }
  
  public string generateBitmap(XPathNodeIterator nodes, string bitmapFile, string sourceFile)
  {
    string baseDir = Path.GetDirectoryName(sourceFile);
    StringBuilder sb = new StringBuilder();
    int imgCount = nodes.Count;
    Bitmap bmp = new Bitmap(16 * imgCount, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
    
    using(Graphics g = Graphics.FromImage(bmp))
    {
      int i = 0;
      foreach(XPathNavigator n in nodes)
      {
        using(Image img = Image.FromFile(Path.GetFullPath(Path.Combine(baseDir, n.Value))))
        {
          g.DrawImage(img, 16 * i++, 0);
        }
        sb.Append(", ");
        sb.Append(i);
      }
    }
    bmp.Save(Path.Combine(baseDir, bitmapFile), ImageFormat.Bmp);
    return sb.ToString();
  }
  
  // @bitmap, @type, @from, gui:StripIcon/@id, gui:StripIcon/@iconFile, true
  public string generateStrip(string bitmap, string type, string assembly, XPathNodeIterator keys, XPathNodeIterator images, bool transparent, string sourceFile, string configuration)
  {
    string baseDir = Path.GetDirectoryName(sourceFile);
    Type tp = LoadType(assembly, type, sourceFile, configuration);
    
    int imgCount = 0;
    foreach(int i in Enum.GetValues(tp))
    {
      if(i+1 > imgCount)
        imgCount = i+1;
    }
        
    Bitmap bmp = new Bitmap(16 * imgCount, 16, transparent ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb);
    
    using(Graphics g = Graphics.FromImage(bmp))
    {
      if (!transparent)
      {
        SolidBrush vsTransparent = new SolidBrush(Color.FromArgb(0xFF,0x00,0xFF));
        g.FillRectangle(vsTransparent, 0, 0, 16*imgCount, 16);
      }
      
      while (keys.MoveNext() && images.MoveNext())
      {
        string fp = Path.GetFullPath(Path.Combine(baseDir, images.Current.Value));
        int pos = (int)tp.InvokeMember(keys.Current.Value, BindingFlags.GetField, null, null, null);
        
        using(Image img = Image.FromFile(fp))
        {
          g.DrawImage(img, 16*pos, 0);
        }
      }    
    }
  
    bmp.Save(Path.Combine(baseDir, bitmap), ImageFormat.Bmp);
    return "";
  }
  
  public string safeToUpper(string value)
  {
    if(string.IsNullOrEmpty(value) || value.Length > 1)
      return value;
      
    char c = value[0];
    
    if((c >= 'a') && (c <= 'z'))
      return value.ToUpperInvariant();
    else
      return value;
  }
  
  public int nodePosition(XPathNavigator needle, XPathNodeIterator haystack)
  {
    int i = 0;
    string needleXml = needle.OuterXml;
    foreach(XPathNavigator n in haystack)
    {    
      if(needleXml == n.OuterXml)
        return i;
        
      i++;
    }
    
    return -1;
  }
  
  public string FullPath(string value)
  {
    return System.IO.Path.GetFullPath(value);
  }
   ]]>
  </msxsl:script>
</xsl:stylesheet>
