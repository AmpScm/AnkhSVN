<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://schemas.studioturtle.net/2007/01/gui/"
                xmlns:gui="http://schemas.studioturtle.net/2007/01/gui/"
                xmlns:task="http://schemas.studioturtle.net/2006/12/layout-task"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:me="local-script">
  <xsl:param name="Src" select="'AnkhSvn.xml'" />
  <xsl:param name="Configuration" select="'Debug'" />
  <xsl:param name="BitmapFile" select="'obj/CtCBitmap.bmp'" />
  <xsl:param name="BitmapId" select="555" />
  <msxsl:script implements-prefix="me" language="C#">
    <msxsl:assembly name="System.Drawing" />
    <msxsl:using namespace="System.Drawing" />
    <msxsl:using namespace="System.Drawing.Imaging" />
    <msxsl:using namespace="System.Reflection" />
    <msxsl:using namespace="System.Runtime.InteropServices" />
    <msxsl:using namespace="System.IO" />
    <![CDATA[	
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
      
	public string InputType(string type, string from, string prefix, string sourceFile, string configuration)
	{
		if(string.IsNullOrEmpty(sourceFile))
		{
			return " // Ignored loading inputtype; sourcefile is not set\n\n";
		}
		StringBuilder sb = new StringBuilder();

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

		AssemblyName name = AssemblyName.GetAssemblyName(from);
		Assembly a = Assembly.Load(System.IO.File.ReadAllBytes(from));
		
		Type tp = a.GetType(type);
		
		GuidAttribute typeAttr = null;
		object[] attrs = tp.GetCustomAttributes(typeof(GuidAttribute), true);
		
		if(attrs.Length > 0)
			typeAttr = (GuidAttribute)attrs[0];

		sb.AppendFormat("\n // Imported from: {0}", from);
		sb.AppendFormat("\n // Type: {0}\n", tp.AssemblyQualifiedName);
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
					val = formatGuid(itemAttr.Value.ToString()) + ":";

				sb.AppendFormat("#define {0}{1} {3}0x{2,-8:X}\n", prefix, fif.Name, v, val);
				sb.AppendFormat("#define {0}{1}_IdOnly 0x{2,-8:X}\n", prefix, fif.Name, v);
			}
			else if(v is Guid)
			{
				Guid g = (Guid)v;

				sb.AppendFormat("#define {0}{1} {2}\n", prefix, fif.Name, formatGuid(g.ToString()));
			}
			else
			{
				sb.AppendFormat("#define {0}{1} {2}\n", prefix, fif.Name, CQuote(v.ToString()));
			}
		}

		sb.AppendFormat("// /Loaded\n", tp.AssemblyQualifiedName);

		return sb.ToString();
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
    int imgCount = nodes.Count;
    Bitmap bmp = new Bitmap(16 * imgCount, 16, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
    
    using(Graphics g = Graphics.FromImage(bmp))
    {
      //SolidBrush transparent = new SolidBrush(Color.FromArgb(0xff,0,0xff));
      //g.FillRectangle(transparent, 0, 0, 16*imgCount, 16);
      
      int i = 0;
      foreach(XPathNavigator n in nodes)
      {
        string fp = Path.GetFullPath(Path.Combine(baseDir, n.Value));
        using(Image img = Image.FromFile(fp))
        {
          g.DrawImage(img, 16 * i++, 0);
        }
      }
    }
    System.Console.WriteLine("Should generate bitmap for {0} images", nodes.Count);
    
    bmp.Save(bitmapFile, System.Drawing.Imaging.ImageFormat.Bmp);
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
   ]]>
  </msxsl:script>
  <xsl:output method="text" />
  <!-- Simple XML schema to generate the ctc file from our own format. -->
  <xsl:template match="/gui:VsGui">
    <xsl:text>/* Generated file; please edit the .gui file instead of this file */&#10;&#10;&#10;</xsl:text>
    <xsl:text>/* Includes */&#10;</xsl:text>
    <xsl:apply-templates select="gui:Imports/gui:Import" mode="include" />
    <xsl:text>/* /Includes */&#10;&#10;</xsl:text>

    <xsl:text>/* Inline values */&#10;</xsl:text>
    <xsl:apply-templates select="gui:Defines/gui:Define" mode="defines" />
    <xsl:text>/* /Defines values */&#10;&#10;</xsl:text>

    <xsl:text>&#10;&#10;CMDPLACEMENT_SECTION&#10;</xsl:text>
    <xsl:text>&#9;&#9;// Item ID, Parent ID, Priority&#10;</xsl:text>
    <xsl:text>&#9;&#9;// Buttons&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:ButtonRef" mode="placement" />
    <xsl:text>&#9;&#9;// Menus&#10;</xsl:text>    
    <xsl:apply-templates select="gui:UI//gui:MenuRef" mode="placement" />
    <xsl:text>&#9;&#9;// Groups&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:GroupRef" mode="placement" />
    <xsl:text>&#10;CMDPLACEMENT_END&#10;&#10;</xsl:text>

    <xsl:value-of select="concat('CMDS_SECTION ', gui:UI/@packageId)"/>

    <xsl:text>&#10;&#10;&#9;MENUS_BEGIN&#10;</xsl:text>
    <xsl:text>&#9;&#9;// New Menu ID, Parent Group ID, Priority, Type, Menu Name, Menu Text&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:Menu" mode="menus" />
    <xsl:text>&#9;MENUS_END&#10;&#10;</xsl:text>


    <xsl:text>&#9;NEWGROUPS_BEGIN&#10;</xsl:text>
    <xsl:text>&#9;&#9;// New Group ID, Parent Menu ID, Priority&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:Group" mode="groups" />
    <xsl:text>&#9;NEWGROUPS_END&#10;&#10;</xsl:text>

    <xsl:text>&#9;BUTTONS_BEGIN&#10;</xsl:text>
    <xsl:text>&#9;&#9;// Command ID, Group ID, Priority, Icon ID, Button Type, Flags, Button Text, Menu Text, ToolTip Text, Command Well Text, English Name, Localized Name&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:Button" mode="buttons" />
    <xsl:text>&#9;BUTTONS_END&#10;&#10;</xsl:text>

    <xsl:text>&#9;BITMAPS_BEGIN&#10;</xsl:text>
    <xsl:text>&#9;&#9;// Bitmap ID, Icon Index...&#10;</xsl:text>
    <xsl:if test="gui:UI/@autoBmpId and //gui:UI//gui:Button[@iconFile]">
      <xsl:text>&#9;&#9;</xsl:text>
      <xsl:value-of select="gui:UI/@autoBmpId"/>:<xsl:value-of select="$BitmapId"/>
      <xsl:for-each select="//gui:UI//gui:Button[@iconFile]">
        <xsl:value-of select="', '"/>
        <xsl:value-of select="position()"/>
      </xsl:for-each>
      <xsl:text>;&#10;</xsl:text>
      <xsl:value-of select="me:generateBitmap(//gui:UI//gui:Button[@iconFile]/@iconFile, $BitmapFile, $Src)"/>
    </xsl:if>
    <xsl:text>&#9;BITMAPS_END&#10;&#10;</xsl:text>

    <xsl:text>&#9;COMBOS_BEGIN&#10;</xsl:text>
    <xsl:text>&#9;&#9;// Combo Box ID, Group ID, Priority, Fill Command ID, Width, Type, Flags, Button Text, Menu Text, ToolTip Text, CommandWellName, CannonicalName, LocalizedCanonicalName;&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:ComboBox" />
    <xsl:text>&#9;COMBOS_END&#10;&#10;</xsl:text>

    <xsl:text>CMDS_END&#10;&#10;</xsl:text>

    <xsl:text>CMDUSED_SECTION&#10;</xsl:text>
    <xsl:text>&#9;// Command ID&#10;</xsl:text>
    <xsl:apply-templates select="gui:UsedCommands/gui:Command" />
    <xsl:text>CMDUSED_END&#10;&#10;</xsl:text>

    <xsl:text>KEYBINDINGS_SECTION&#10;</xsl:text>
    <xsl:text>&#9;// Command ID, Editor ID, Emulation ID, Key state&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:KeyBinding" />
    <xsl:text>KEYBINDINGS_END&#10;&#10;</xsl:text>

    <xsl:text>VISIBILITY_SECTION&#10;</xsl:text>
    <xsl:text>&#9;// Item ID, Context ID&#10;</xsl:text>
    <xsl:text>&#9;// Buttons&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:Button/gui:Visibility | gui:UI//gui:ButtonRef/gui:Visibility" mode="visibility" />
    <xsl:text>&#9;// Menus&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:Menu/gui:Visibility | gui:UI//gui:MenuRef/gui:Visibility" mode="visibility" />
    <xsl:text>&#9;// Groups&#10;</xsl:text>
    <xsl:apply-templates select="gui:UI//gui:Group/gui:Visibility | gui:UI//gui:GroupRef/gui:Visibility" mode="visibility" />
    <xsl:text>VISIBILITY_END&#10;&#10;</xsl:text>
    <xsl:text>/* END */&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:Import[@include]" mode="include">
    <xsl:text>#include "</xsl:text>
    <xsl:value-of select="@include" />
    <xsl:text>"&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:Import[@type]" mode="include">
    <xsl:value-of select="me:InputType(@type, @from, @prefix, $Src, $Configuration)"/>
  </xsl:template>
  <xsl:template match="gui:*[@id and @value]" mode="defines">
    <xsl:text>#define </xsl:text>
    <xsl:value-of select="concat(@id, ' ', @value, '&#9;&#9; // from ', local-name())" />
    <xsl:text>&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:Define[@id and @guid]" mode="defines">
    <xsl:text>#define </xsl:text>
    <xsl:value-of select="concat(@id, ' ', me:formatGuid(@guid))" />
    <xsl:text>&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:Define[@id and @string]" mode="defines">
    <xsl:text>#define </xsl:text>
    <xsl:value-of select="concat(@id, ' ', me:CQuote(@string))" />
    <xsl:text>&#10;</xsl:text>
  </xsl:template>

  <!-- ************************************************* -->
  <!-- **                 Menus                       ** -->
  <!-- ************************************************* -->
  <xsl:template match="gui:Menu" mode="menus">
    <xsl:text>&#9;&#9;</xsl:text>
    <!-- Menu-Id -->
    <xsl:value-of select="@id"/>
    <xsl:text>, </xsl:text>
    <!-- Group-Id -->
    <xsl:value-of select="../@id"/>
    <xsl:text>, </xsl:text>
    <!-- Priority -->
    <xsl:value-of select="@priority"/>
    <xsl:text>, </xsl:text>
    <!-- Type -->
    <xsl:variable name="type">
      <xsl:value-of select="@type"/>
      <xsl:if test="@alwaysCreate='true'">
        <xsl:text>|ALWAYSCREATE</xsl:text>
      </xsl:if>
      <xsl:if test="@defaultDocked='true'">
        <xsl:text>|DEFAULTDOCKED</xsl:text>
      </xsl:if>
      <xsl:if test="@defaultInvisible='true' or gui:Visibility[@context]">
        <xsl:text>|DEFAULTINVISIBLE</xsl:text>
      </xsl:if>
      <xsl:if test="@dontCache='true'">
        <xsl:text>|DONTCACHE</xsl:text>
      </xsl:if>
      <xsl:if test="@dynamicVisibility='true' or @defaultInvisible='true' or gui:Visibility[@context]">
        <!-- Should set if default invisible-->
        <xsl:text>|DYNAMICVISIBILITY</xsl:text>
      </xsl:if>
      <xsl:if test="@noCustomize='true'">
        <xsl:text>|NOCUSTOMIZE</xsl:text>
      </xsl:if>
      <xsl:if test="@notInTBList='true'">
        <xsl:text>|NOTINTBLIST</xsl:text>
      </xsl:if>
      <xsl:if test="@noToolbarClose='true'">
        <xsl:text>|NOTOOLBARCLOSE</xsl:text>
      </xsl:if>
      <xsl:if test="@textChanges='true'">
        <xsl:text>|TEXTCHANGES</xsl:text>
      </xsl:if>
      <xsl:if test="@textIsAnchorCommand='true'">
        <xsl:text>|TEXTISANCHORCOMMAND</xsl:text>
      </xsl:if>
      <xsl:if test="@iconAndText='true'">
        <xsl:text>|ICONANDTEXT</xsl:text>
      </xsl:if>
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="starts-with($type,'|')">
        <xsl:value-of select="substring($type, 2)"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$type"/>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text>, </xsl:text>
    <!-- Name -->
    <xsl:value-of select="me:CQuote(@name)"/>
    <xsl:text>, </xsl:text>
    <xsl:if test="@text">
      <xsl:value-of select="me:CQuote(@text)"/>
    </xsl:if>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
  <!-- ************************************************* -->
  <!-- **                 Groups                      ** -->
  <!-- ************************************************* -->
  <xsl:template match="gui:Group" mode="groups">
    <xsl:text>&#9;&#9;</xsl:text>
    <!-- Group-Id -->
    <xsl:value-of select="@id"/>
    <xsl:text>, </xsl:text>
    <!-- Menu-Id -->
    <xsl:value-of select="../@id"/>
    <xsl:text>, </xsl:text>
    <!-- Priority -->
    <xsl:value-of select="@priority"/>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
  <!-- ************************************************* -->
  <!-- **                 Buttons                     ** -->
  <!-- ************************************************* -->
  <xsl:template match="gui:Button" mode="buttons">
    <xsl:text>&#9;&#9;</xsl:text>
    <!-- Button-Id -->
    <xsl:value-of select="@id"/>
    <xsl:text>, </xsl:text>
    <!-- Group-Id -->
    <xsl:value-of select="../@id"/>
    <xsl:text>, </xsl:text>
    <!-- Priority -->
    <xsl:value-of select="@priority"/>
    <xsl:text>, </xsl:text>
    <!-- Icon ID -->
    <xsl:choose>
      <xsl:when test="@iconFile">
        <xsl:value-of select="concat(//gui:UI/@autoBmpId,':', me:nodePosition(., //gui:UI//gui:Button[@iconFile])+1)" />
      </xsl:when>
      <xsl:when test="@iconId and contains(@iconId, ':')">
        <xsl:value-of select="@iconId"/>
      </xsl:when>
      <xsl:when test="@iconId">
        <!-- TODO: Using the default is probably not what we want here! -->
        <xsl:value-of select="concat(ancestor-or-self::gui:*/@default-prefix, ':', @iconId)" />
      </xsl:when>
      <!--xsl:when test="Icon">
        
      </xsl:when-->
      <xsl:otherwise>
        <xsl:text>guidOfficeIcon:msotcidNoIcon</xsl:text>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text>, </xsl:text>
    <!-- Type -->
    <xsl:choose>
      <xsl:when test="@type">
        <xsl:value-of select="@type"/>
      </xsl:when>
      <xsl:otherwise>BUTTON</xsl:otherwise>
    </xsl:choose>
    <xsl:text>, </xsl:text>
    <!-- Flags -->
    <xsl:variable name="flags">
      <xsl:if test="@noKeyCustomize='true'">
        <xsl:text>NOKEYCUSTOMIZE</xsl:text>
      </xsl:if>
      <xsl:if test="@noButtonCustomize='true'">
        <xsl:text>|NOBUTTONCUSTOMIZE</xsl:text>
      </xsl:if>
      <xsl:if test="@noCustomize='true'">
        <xsl:text>|NOCUSTOMIZE</xsl:text>
      </xsl:if>
      <xsl:if test="@pict='true'">
        <xsl:text>|PICT</xsl:text>
      </xsl:if>
      <xsl:if test="@textOnly='true'">
        <xsl:text>|TEXTONLY</xsl:text>
      </xsl:if>
      <xsl:if test="@iconAndText='true'">
        <xsl:text>|ICONANDTEXT</xsl:text>
      </xsl:if>
      <xsl:if test="@textContextUseButton='true'">
        <xsl:text>|TEXTCONTEXTUSEBUTTON</xsl:text>
      </xsl:if>
      <xsl:if test="@textMenuUseButton='true'">
        <xsl:text>|TEXTMENUUSEBUTTON</xsl:text>
      </xsl:if>
      <xsl:if test="@textMenuCtrlUseMenu='true'">
        <xsl:text>|TEXTMENUCTRLUSEMENU</xsl:text>
      </xsl:if>
      <xsl:if test="@textCascadeUseButton='true'">
        <xsl:text>|TEXTCASCADEUSEBUTTON</xsl:text>
      </xsl:if>
      <xsl:if test="@textChanges='true'">
        <xsl:text>|TEXTCHANGES</xsl:text>
      </xsl:if>
      <xsl:if test="@defaultDisabled='true' or @defaultInvisible='true'">
        <xsl:text>|DEFAULTDISABLED</xsl:text>
      </xsl:if>
      <xsl:if test="@defaultInvisible='true' or gui:Visibility[@context]">
        <xsl:text>|DEFAULTINVISIBLE</xsl:text>
      </xsl:if>
      <xsl:if test="@dynamicVisibility='true' or @defaultInvisible='true' or gui:Visibility[@context]">
        <xsl:text>|DYNAMICVISIBILITY</xsl:text>
      </xsl:if>
      <xsl:if test="@dynamicItemStart='true'">
        <xsl:text>|DYNAMICITEMSTART</xsl:text>
      </xsl:if>
      <xsl:if test="@commandWellOnly='true'">
        <xsl:text>|COMMANDWELLONLY</xsl:text>
      </xsl:if>
      <xsl:if test="@allowParams='true'">
        <xsl:text>|ALLOWPARAMS</xsl:text>
      </xsl:if>
      <xsl:if test="@postExec='true'">
        <xsl:text>|POSTEXEC</xsl:text>
      </xsl:if>
      <xsl:if test="@dontCache='true'">
        <xsl:text>|DONTCACHE</xsl:text>
      </xsl:if>
      <xsl:if test="@fixMenuController='true'">
        <xsl:text>|FIXMENUCONTROLLER</xsl:text>
      </xsl:if>
      <xsl:if test="@noShowOnMenuController='true'">
        <xsl:text>|NOSHOWONMENUCONTROLLER</xsl:text>
      </xsl:if>
      <xsl:if test="@routeToDocs='true'">
        <xsl:text>|ROUTETODOCS</xsl:text>
      </xsl:if>
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="string-length($flags) = 0">
        <xsl:value-of select="'0'"/>
      </xsl:when>
      <xsl:when test="starts-with($flags,'|')">
        <xsl:value-of select="substring($flags, 2)"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$flags"/>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text>, </xsl:text>
    <!-- Button Text -->
    <xsl:value-of select="me:CQuote(@text)"/>
    <xsl:text>, </xsl:text>
    <!-- Menu Text -->
    <xsl:if test="@menuText">
      <xsl:value-of select="me:CQuote(@menuText)"/>
    </xsl:if>
    <xsl:text>, </xsl:text>
    <!-- ToolTip -->
    <xsl:if test="@toolTip">
      <xsl:value-of select="me:CQuote(@toolTip)"/>
    </xsl:if>
    <xsl:text>, </xsl:text>
    <!-- Command Well Name-->
    <xsl:if test="@commandWellText">
      <xsl:value-of select="me:CQuote(@commandWellText)"/>
    </xsl:if>
    <xsl:text>, </xsl:text>
    <!-- Command Name-->
    <xsl:if test="@name">
      <xsl:value-of select="me:CQuote(@name)"/>
    </xsl:if>
    <xsl:text>, </xsl:text>
    <!-- Localized Command Name-->
    <xsl:choose>
      <xsl:when test="@localizedName">
        <xsl:value-of select="me:CQuote(@localizedName)"/>
      </xsl:when>
      <xsl:when test="@name">
        <xsl:value-of select="me:CQuote(@name)"/>
      </xsl:when>
    </xsl:choose>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:ComboBox">
    <xsl:text>&#9;&#9;</xsl:text>
    <!-- Combo Box ID -->
    <xsl:value-of select="@id"/>
    <xsl:text>, </xsl:text>
    <!-- Group-Id -->
    <xsl:value-of select="../@id"/>
    <xsl:text>, </xsl:text>
    <!-- Priority -->
    <xsl:value-of select="@priority"/>
    <xsl:text>, </xsl:text>
    <!-- Fill Command ID -->
    <xsl:value-of select="@fillCommand"/>
    <xsl:text>, </xsl:text>
    <!-- Width -->
    <xsl:value-of select="@width"/>
    <xsl:text>, </xsl:text>
    <!-- Type -->
    <xsl:value-of select="@type"/>
    <xsl:text>, </xsl:text>
    <!-- Flags -->
    <xsl:variable name="flags">
      <xsl:if test="@noKeyCustomize='true'">
        <xsl:text>NOKEYCUSTOMIZE</xsl:text>
      </xsl:if>
      <xsl:if test="@noButtonCustomize='true'">
        <xsl:text>|NOBUTTONCUSTOMIZE</xsl:text>
      </xsl:if>
      <xsl:if test="@noCustomize='true'">
        <xsl:text>|NOCUSTOMIZE</xsl:text>
      </xsl:if>
      <xsl:if test="@defaultInvisible='true' or gui:Visibility[@context]">
        <xsl:text>|DEFAULTINVISIBLE</xsl:text>
      </xsl:if>
      <xsl:if test="@dynamicVisibility='true' or @defaultInvisible='true' or gui:Visibility[@context]">
        <xsl:text>|DYNAMICVISIBILITY</xsl:text>
      </xsl:if>
      <xsl:if test="@commandWellOnly='true'">
        <xsl:text>|COMMANDWELLONLY</xsl:text>
      </xsl:if>
      <xsl:if test="@iconAndText='true'">
        <xsl:text>|ICONANDTEXT</xsl:text>
      </xsl:if>
      <xsl:if test="@filterKeys='true'">
        <xsl:text>|FILTERKEYS</xsl:text>
      </xsl:if>
      <xsl:if test="@noAutoComplete='true'">
        <xsl:text>|NOAUTOCOMPLETE</xsl:text>
      </xsl:if>
      <xsl:if test="@caseSensitive='true'">
        <xsl:text>|CASESENSITIVE</xsl:text>
      </xsl:if>
      <xsl:if test="@stretchHorizontally='true'">
        <xsl:text>|STRETCHHORIZONTALLY</xsl:text>
      </xsl:if>
    </xsl:variable>
    <xsl:choose>
      <xsl:when test="string-length($flags) = 0">
        <xsl:value-of select="'0'"/>
      </xsl:when>
      <xsl:when test="starts-with($flags,'|')">
        <xsl:value-of select="substring($flags, 2)"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$flags"/>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:text>, </xsl:text>
    <!-- Button Text -->
    <xsl:value-of select="me:CQuote(@text)"/>
    <xsl:text>, </xsl:text>
    <!-- Menu Text -->
    <xsl:if test="@menuText">
      <xsl:value-of select="me:CQuote(@menuText)"/>
    </xsl:if>
    <xsl:text>, </xsl:text>
    <!-- ToolTip -->
    <xsl:if test="@toolTip">
      <xsl:value-of select="me:CQuote(@toolTip)"/>
    </xsl:if>
    <xsl:text>, </xsl:text>
    <!-- Command Well Name-->
    <xsl:if test="@commandWellText">
      <xsl:value-of select="me:CQuote(@commandWellText)"/>
    </xsl:if>
    <xsl:text>, </xsl:text>
    <!-- Command Name-->
    <xsl:if test="@name">
      <xsl:value-of select="me:CQuote(@name)"/>
    </xsl:if>
    <xsl:text>, </xsl:text>
    <!-- Localized Command Name-->
    <xsl:if test="@localizedName">
      <xsl:value-of select="me:CQuote(@localizedName)"/>
    </xsl:if>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:ButtonRef" mode="placement">
    <xsl:text>&#9;&#9;</xsl:text>
    <!-- Item ID -->
    <xsl:value-of select="@id"/>
    <xsl:text>, </xsl:text>
    <!-- Parent-Id -->
    <xsl:value-of select="../@id"/>
    <xsl:text>, </xsl:text>
    <!-- Priority -->
    <xsl:value-of select="@priority"/>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:MenuRef | gui:GroupRef" mode="placement">
    <xsl:if test="
            (self::gui:MenuRef and (parent::gui:Group or parent::gui:GroupRef)) or
            (self::gui:GroupRef and (parent::gui:Menu or parent::gui:MenuRef))">
      <xsl:text>&#9;&#9;</xsl:text>
      <!-- Item ID -->
      <xsl:value-of select="@id"/>
      <xsl:text>, </xsl:text>
      <!-- Parent-Id -->
      <xsl:value-of select="../@id"/>
      <xsl:text>, </xsl:text>
      <!-- Priority -->
      <xsl:value-of select="@priority"/>
      <xsl:text>;&#10;</xsl:text>
    </xsl:if>
  </xsl:template>
  <xsl:template match="gui:KeyBinding">
    <xsl:text>&#9;</xsl:text>
    <!-- Command Id -->
    <xsl:value-of select="../@id"/>
    <xsl:text>, </xsl:text>
    <!-- Editor Id -->
    <xsl:value-of select="@editor"/>
    <xsl:text>, </xsl:text>
    <!-- Emulation (Defined as for future use; repeat editor) -->
    <xsl:value-of select="@editor"/>
    <xsl:text>, </xsl:text>
    <xsl:choose>
      <xsl:when test="string-length(@key1) = 1">
        <xsl:text>'</xsl:text>
        <xsl:value-of select="me:safeToUpper(@key1)"/>
        <xsl:text>'</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="@key1"/>
      </xsl:otherwise>
    </xsl:choose>
    <xsl:value-of select="':'"/>
    <xsl:if test="contains(@mod1, 'ctrl')">
      <xsl:value-of select="'C'"/>
    </xsl:if>
    <xsl:if test="contains(@mod1, 'alt')">
      <xsl:value-of select="'A'"/>
    </xsl:if>
    <xsl:if test="contains(@mod1, 'shift')">
      <xsl:value-of select="'S'"/>
    </xsl:if>
    <xsl:if test="@key2">
      <xsl:text> : </xsl:text>
      <xsl:choose>
        <xsl:when test="string-length(@key2) = 1">
          <xsl:text>'</xsl:text>
          <xsl:value-of select="me:safeToUpper(@key2)"/>
          <xsl:text>'</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@key2"/>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:value-of select="':'"/>
      <xsl:if test="contains(@mod2, 'ctrl')">
        <xsl:value-of select="'C'"/>
      </xsl:if>
      <xsl:if test="contains(@mod2, 'alt')">
        <xsl:value-of select="'A'"/>
      </xsl:if>
      <xsl:if test="contains(@mod2, 'shift')">
        <xsl:value-of select="'S'"/>
      </xsl:if>      
    </xsl:if>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:Visibility[parent::gui:*/@id and @context]" mode="visibility">
    <xsl:text>&#9;</xsl:text>
    <xsl:value-of select="../@id"/>
    <xsl:text>, </xsl:text>
    <xsl:value-of select="@context"/>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:Command">
    <xsl:text>&#9;</xsl:text>
    <xsl:value-of select="@id"/>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
</xsl:stylesheet>
