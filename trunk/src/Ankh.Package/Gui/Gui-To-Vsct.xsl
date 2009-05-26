<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable"
                xmlns:gui="http://schemas.studioturtle.net/2007/01/gui/"
                xmlns:task="http://schemas.studioturtle.net/2006/12/layout-task"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:me="local-script">

  <xsl:include href="./Gui-Common.xsl"/>
  <xsl:output method="xml" indent="yes"/>
  <!-- Simple XML schema to generate the ctc file from our own format. -->
  <xsl:template match="/gui:VsGui">
    <CommandTable>
      <xsl:comment>Generated file; please edit the .gui file instead of this file</xsl:comment>
      <xsl:comment>Includes</xsl:comment>
      <xsl:apply-templates select="gui:Imports/gui:Import[@include]" mode="include" />

      <xsl:variable name="symbols">
        <xsl:apply-templates select="gui:Imports/gui:Import[not (@include)]" mode="include" />
      </xsl:variable>

      <Commands package="{gui:UI/@packageId}">
        <Menus>
          <xsl:apply-templates select="gui:UI//gui:Menu" mode="menus" />
        </Menus>
        <Groups>
          <xsl:apply-templates select="gui:UI//gui:Group" mode="groups" />
        </Groups>
        <Buttons>
          <xsl:apply-templates select="gui:UI//gui:Button" mode="buttons" />
        </Buttons>
        <Bitmaps>
          <xsl:if test="gui:UI/@autoBmpId and //gui:UI//gui:Button[@iconFile]">
            <Bitmap guid="{gui:UI/@autoBmpId}">
              <xsl:choose>
                <xsl:when test="1=1">
                  <!-- This caches the bitmap file inside the CTC -->
                  <xsl:attribute name="href">
                    <xsl:value-of select="$BitmapFile"/>
                  </xsl:attribute>
                </xsl:when>
                <xsl:otherwise>
                  <!-- This replicates the old behavior -->
                  <xsl:attribute name="resID">
                    <xsl:value-of select="gui:UI/@autoBmpResId"/>
                  </xsl:attribute>
                </xsl:otherwise>
              </xsl:choose>
              <xsl:attribute name="usedList">
                <xsl:value-of select="normalize-space(substring-after(me:generateBitmap(//gui:UI//gui:Button[@iconFile]/@iconFile, $BitmapFile, $Src),','))"/>
              </xsl:attribute>
            </Bitmap>
          </xsl:if>
        </Bitmaps>

      </Commands>

      <xsl:comment>
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
        <xsl:text>&#9;// Out of context commands&#10;</xsl:text>
        <xsl:apply-templates select="gui:KeyBindings/gui:Editor/gui:BindKey" />
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
      </xsl:comment>
      <xsl:for-each select="gui:BitmapStrips/gui:Strip">
        <xsl:comment>
          <xsl:value-of select="concat('Creating strip: ', @name)" />
        </xsl:comment>
        <xsl:variable name="q">
          <xsl:if test="@bitmap">
            <xsl:value-of select="me:generateStrip(@bitmap, @type, @from, gui:StripIcon/@id, gui:StripIcon/@iconFile, 1, $Src, $Configuration)"/>
          </xsl:if>
          <xsl:if test="@bitmap24">
            <xsl:value-of select="me:generateStrip(@bitmap24, @type, @from, gui:StripIcon/@id, gui:StripIcon/@iconFile, 0, $Src, $Configuration)"/>
          </xsl:if>
        </xsl:variable>
      </xsl:for-each>
      <Symbols>
        <xsl:copy-of select="$symbols" />
      </Symbols>
    </CommandTable>
  </xsl:template>
  <xsl:template match="gui:Import[@include]" mode="include">
    <Extern href="{@include}" />
  </xsl:template>
  <xsl:template match="gui:Import[@type]" mode="include">
    <xsl:copy-of select="me:InputSymbol(@type, @from, @prefix, $Src, $Configuration)"/>
  </xsl:template>
  <xsl:template name="parentRef">
    <Parent
        guid="{substring-before(me:MakeId(../@id),':')}"
        id="{substring-after(me:MakeId(../@id),':')}" />
  </xsl:template>
  <xsl:template name="strings">
    <Strings>
      <ButtonText>
        <xsl:value-of select="@text"/>
      </ButtonText>
      <xsl:if test="@menuText">
        <MenuText>
          <xsl:value-of select="@menuText"/>
        </MenuText>
      </xsl:if>
      <xsl:if test="@toolTip">
        <ToolTipText>
          <xsl:value-of select="@toolTip"/>
        </ToolTipText>
      </xsl:if>
      <xsl:if test="@commandWellText">
        <CommandName>
          <xsl:value-of select="@commandWellText"/>
        </CommandName>
      </xsl:if>
      <xsl:if test="@name">
        <CanonicalName>
          <xsl:value-of select="@name"/>
        </CanonicalName>
      </xsl:if>
      <xsl:choose>
        <xsl:when test="@localizedName">
          <LocCanonicalName>
            <xsl:value-of select="@localizedName"/>
          </LocCanonicalName>
        </xsl:when>
        <xsl:when test="@name">
          <LocCanonicalName>
            <xsl:value-of select="@name"/>
          </LocCanonicalName>
        </xsl:when>
      </xsl:choose>
    </Strings>
  </xsl:template>
  <xsl:template name="iconRef">
    <xsl:choose>
      <xsl:when test="@iconFile">
        <Icon
          guid="{//gui:UI/@autoBmpId}"
          id="{me:nodePosition(., //gui:UI//gui:Button[@iconFile])+1}" />
      </xsl:when>
      <xsl:when test="@iconId and contains(@iconId, ':')">
        <Icon
          guid="{substring-before(@iconId,':')}"
          id="{substring-after(@iconId,':')}" />
      </xsl:when>
      <xsl:when test="@iconId">
        <Icon
          guid="{ancestor-or-self::gui:*/@default-prefix}"
          id="{@iconId}" />
      </xsl:when>
    </xsl:choose>
  </xsl:template>
  <!-- ************************************************* -->
  <!-- **                 Menus                       ** -->
  <!-- ************************************************* -->
  <xsl:template match="gui:Menu" mode="menus">
    <Menu
      guid="{substring-before(me:MakeId(@id),':')}"
      id="{substring-after(me:MakeId(@id), ':')}"
      priority="{@priority}" >
      <xsl:attribute name="type">
        <xsl:choose>
          <xsl:when test="@type">
            <xsl:value-of select="@type"/>
          </xsl:when>
          <xsl:otherwise>Menu</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:call-template name="parentRef" />
      <xsl:if test="@alwaysCreate='true'">
        <CommandFlag>AlwaysCreate</CommandFlag>
      </xsl:if>
      <xsl:if test="@defaultDocked='true'">
        <CommandFlag>DefaultDocked</CommandFlag>
      </xsl:if>
      <xsl:if test="@defaultInvisible='true' or gui:Visibility[@context]">
        <CommandFlag>DefaultInvisible</CommandFlag>
      </xsl:if>
      <xsl:if test="@dontCache='true'">
        <CommandFlag>DontCache</CommandFlag>
      </xsl:if>
      <xsl:if test="@dynamicVisibility='true' or @defaultInvisible='true' or gui:Visibility[@context]">
        <CommandFlag>DynamicVisibility</CommandFlag>
      </xsl:if>
      <xsl:if test="@iconAndText='true'">
        <CommandFlag>IconAndText</CommandFlag>
      </xsl:if>
      <xsl:if test="@noCustomize='true'">
        <CommandFlag>NoCustomize</CommandFlag>
      </xsl:if>
      <xsl:if test="@notInTBList='true'">
        <CommandFlag>NotInTBList</CommandFlag>
      </xsl:if>
      <xsl:if test="@noToolbarClose='true'">
        <CommandFlag>NoToolbarClose</CommandFlag>
      </xsl:if>
      <xsl:if test="@textChanges='true' or @textIsAnchorCommand='true'">
        <CommandFlag>TextChanges</CommandFlag>
      </xsl:if>
      <xsl:if test="@textIsAnchorCommand='true'">
        <CommandFlag>TextIsAnchorCommand</CommandFlag>
      </xsl:if>
      <xsl:call-template name="strings" />
    </Menu>
  </xsl:template>
  <!-- ************************************************* -->
  <!-- **                 Groups                      ** -->
  <!-- ************************************************* -->
  <xsl:template match="gui:Group" mode="groups">
    <Group
      guid="{substring-before(me:MakeId(@id),':')}"
      id="{substring-after(me:MakeId(@id), ':')}"
      priority="{@priority}" >
      <xsl:call-template name="parentRef" />
    </Group>
  </xsl:template>
  <!-- ************************************************* -->
  <!-- **                 Buttons                     ** -->
  <!-- ************************************************* -->
  <xsl:template match="gui:Button" mode="buttons">
    <Button
      guid="{substring-before(me:MakeId(@id),':')}"
      id="{substring-after(me:MakeId(@id), ':')}"
      priority="{@priority}" >
      <xsl:attribute name="type">
        <xsl:choose>
          <xsl:when test="@type">
            <xsl:value-of select="@type"/>
          </xsl:when>
          <xsl:otherwise>Button</xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:call-template name="parentRef" />
      <xsl:call-template name="iconRef" />
      <!-- Flags -->
      <xsl:if test="@allowParams='true'">
        <CommandFlag>AllowParams</CommandFlag>
      </xsl:if>
      <xsl:if test="@commandWellOnly='true'">
        <CommandFlag>CommandWellOnly</CommandFlag>
      </xsl:if>
      <xsl:if test="@defaultDisabled='true' or @defaultInvisible='true'">
        <CommandFlag>DefaultDisabled</CommandFlag>
      </xsl:if>
      <xsl:if test="@defaultInvisible='true' or gui:Visibility[@context]">
        <CommandFlag>DefaultInvisible</CommandFlag>
      </xsl:if>
      <xsl:if test="@dontCache='true'">
        <CommandFlag>DontCache</CommandFlag>
      </xsl:if>
      <xsl:if test="@dynamicItemStart='true'">
        <CommandFlag>DynamicItemStart</CommandFlag>
      </xsl:if>
      <xsl:if test="@dynamicVisibility='true' or @defaultInvisible='true' or gui:Visibility[@context]">
        <CommandFlag>DynamicVisibility</CommandFlag>
      </xsl:if>
      <xsl:if test="@fixMenuController='true'">
        <CommandFlag>FixMenuController</CommandFlag>
      </xsl:if>
      <xsl:if test="@iconAndText='true'">
        <CommandFlag>IconAndText</CommandFlag>
      </xsl:if>
      <xsl:if test="@noButtonCustomize='true'">
        <CommandFlag>NoButtonCustomize</CommandFlag>
      </xsl:if>
      <xsl:if test="@noCustomize='true'">
        <CommandFlag>NoCustomize</CommandFlag>
      </xsl:if>
      <xsl:if test="@noKeyCustomize='true'">
        <CommandFlag>NoKeyCustomize</CommandFlag>
      </xsl:if>
      <xsl:if test="@noShowOnMenuController='true'">
        <CommandFlag>NoShowOnMenuController</CommandFlag>
      </xsl:if>
      <xsl:if test="@pict='true'">
        <CommandFlag>Pict</CommandFlag>
      </xsl:if>
      <xsl:if test="@postExec='true'">
        <CommandFlag>PostExec</CommandFlag>
      </xsl:if>
      <!-- ProfferedCmd -->
      <xsl:if test="@routeToDocs='true'">
        <CommandFlag>RouteToDocs</CommandFlag>
      </xsl:if>
      <xsl:if test="@textCascadeUseButton='true'">
        <CommandFlag>TextCascadeUseBtn</CommandFlag>
      </xsl:if>
      <xsl:if test="@textMenuUseButton='true'">
        <CommandFlag>|TEXTMENUUSEBUTTON</CommandFlag>
      </xsl:if>
      <xsl:if test="@textChanges='true'">
        <CommandFlag>TextChanges</CommandFlag>
      </xsl:if>
      <!-- TextChangesButton?? -->
      <xsl:if test="@textContextUseButton='true'">
        <CommandFlag>TextContextUseButton</CommandFlag>
      </xsl:if>
      <xsl:if test="@textMenuCtrlUseMenu='true'">
        <CommandFlag>TextMenuCtrlUseMenu</CommandFlag>
      </xsl:if>
      <xsl:if test="@textMenuUseButtton='true'">
        <CommandFlag>TextMenuUseButton</CommandFlag>
      </xsl:if>
      <xsl:if test="@textOnly='true'">
        <CommandFlag>TextOnly</CommandFlag>
      </xsl:if>
      <xsl:call-template name="strings" />
    </Button>
  </xsl:template>
  <xsl:template match="gui:ComboBox">
    <xsl:text>&#9;&#9;</xsl:text>
    <!-- Combo Box ID -->
    <xsl:value-of select="me:MakeId(@id)"/>
    <xsl:text>, </xsl:text>
    <!-- Group-Id -->
    <xsl:value-of select="me:MakeId(../@id)"/>
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
    <xsl:value-of select="me:MakeId(@id)"/>
    <xsl:text>, </xsl:text>
    <!-- Parent-Id -->
    <xsl:value-of select="me:MakeId(../@id)"/>
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
      <xsl:value-of select="me:MakeId(@id)"/>
      <xsl:text>, </xsl:text>
      <!-- Parent-Id -->
      <xsl:value-of select="me:MakeId(../@id)"/>
      <xsl:text>, </xsl:text>
      <!-- Priority -->
      <xsl:value-of select="@priority"/>
      <xsl:text>;&#10;</xsl:text>
    </xsl:if>
  </xsl:template>
  <xsl:template match="gui:KeyBinding">
    <xsl:text>&#9;</xsl:text>
    <!-- Command Id -->
    <xsl:value-of select="me:MakeId(../@id)"/>
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
  <xsl:template match="gui:BindKey">
    <xsl:text>&#9;</xsl:text>
    <!-- Command Id -->
    <xsl:value-of select="me:MakeId(@id)"/>
    <xsl:text>, </xsl:text>
    <!-- Editor Id -->
    <xsl:value-of select="me:MakeId(../@id)"/>
    <xsl:text>, </xsl:text>
    <!-- Emulation (Defined as for future use; repeat editor) -->
    <xsl:value-of select="me:MakeId(../@id)"/>
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
    <xsl:value-of select="me:MakeId(../@id)"/>
    <xsl:text>, </xsl:text>
    <xsl:value-of select="@context"/>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
  <xsl:template match="gui:Command">
    <xsl:text>&#9;</xsl:text>
    <xsl:value-of select="me:MakeId(@id)"/>
    <xsl:text>;&#10;</xsl:text>
  </xsl:template>
</xsl:stylesheet>
