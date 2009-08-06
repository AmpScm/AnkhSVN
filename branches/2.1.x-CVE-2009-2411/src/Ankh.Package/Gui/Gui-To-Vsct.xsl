<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns="http://schemas.microsoft.com/VisualStudio/2005-10-18/CommandTable"
                xmlns:gui="http://schemas.studioturtle.net/2007/01/gui/"
                xmlns:me="local-script"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:xs="http://www.w3.org/2001/XMLSchema" 
                exclude-result-prefixes="gui me msxsl">

  <xsl:include href="./Gui-Common.xsl"/>
  <xsl:output method="xml" indent="yes" />
  <!-- Simple XML schema to generate the ctc file from our own format. -->
  <xsl:template match="/gui:VsGui">
    <CommandTable language="en-us">
      <xsl:comment>Generated file; please edit the original file instead of this generated file</xsl:comment>
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
                    <xsl:value-of select="me:FullPath($BitmapFile, $Src)"/>
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
        <Combos>
          <xsl:apply-templates select="gui:UI//gui:ComboBox" mode="combo"/>
        </Combos>

      </Commands>
      <CommandPlacements>
        <xsl:comment>Menus</xsl:comment>
        <xsl:apply-templates select="gui:UI//gui:MenuRef" mode="placement" />
        <xsl:comment>Groups</xsl:comment>
        <xsl:apply-templates select="gui:UI//gui:GroupRef" mode="placement" />
        <xsl:comment>Buttons</xsl:comment>
        <xsl:apply-templates select="gui:UI//gui:ButtonRef" mode="placement" />
      </CommandPlacements>

      <VisibilityConstraints>
        <xsl:comment>&#9;// Menus</xsl:comment>
        <xsl:apply-templates select="gui:UI//gui:Menu/gui:Visibility | gui:UI//gui:MenuRef/gui:Visibility" mode="visibility" />
        <xsl:comment>&#9;// Groups</xsl:comment>
        <xsl:apply-templates select="gui:UI//gui:Group/gui:Visibility | gui:UI//gui:GroupRef/gui:Visibility" mode="visibility" />
        <xsl:comment>&#9;// Buttons</xsl:comment>
        <xsl:apply-templates select="gui:UI//gui:Button/gui:Visibility | gui:UI//gui:ButtonRef/gui:Visibility" mode="visibility" />
      </VisibilityConstraints>

      <KeyBindings>
        <xsl:comment>Button bindings</xsl:comment>
        <xsl:apply-templates select="gui:UI//gui:KeyBinding" />
        <xsl:comment>Out of context commands</xsl:comment>
        <xsl:apply-templates select="gui:KeyBindings/gui:Editor/gui:BindKey" />
      </KeyBindings>

      <xsl:if test="gui:UsedCommands/gui:Command">
        <UsedCommands>
          <xsl:apply-templates select="gui:UsedCommands/gui:Command" mode="usedCommand" />
        </UsedCommands>
      </xsl:if>

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
    <Extern href="{translate(@include,'/','\')}" />
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
      <xsl:if test="@localizedName">
        <LocCanonicalName>
          <xsl:value-of select="@localizedName"/>
        </LocCanonicalName>
      </xsl:if>
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
  <xsl:template match="gui:ComboBox" mode="combo">
    <Combo
      guid="{substring-before(me:MakeId(@id),':')}"
      id="{substring-after(me:MakeId(@id), ':')}"
      idCommandList="{@fillCommand}"
      defaultWidth="{@width}"
      priority="{@priority}"
      type="{@type}">
      <xsl:call-template name="parentRef" />
      <xsl:if test="@caseSensitive='true'">
        <CommandFlag>CaseSensitive</CommandFlag>
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
      <xsl:if test="@dynamicVisibility='true' or @defaultInvisible='true' or gui:Visibility[@context]">
        <CommandFlag>DynamicVisibility</CommandFlag>
      </xsl:if>
      <xsl:if test="@filterKeys='true'">
        <CommandFlag>FilterKeys</CommandFlag>
      </xsl:if>
      <xsl:if test="@iconAndText='true'">
        <CommandFlag>IconAndText</CommandFlag>
      </xsl:if>
      <xsl:if test="@noAutoComplete='true'">
        <CommandFlag>NoAutoComplete</CommandFlag>
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
      <xsl:if test="@stretchHorizontally='true'">
        <CommandFlag>StretchHorizontally</CommandFlag>
      </xsl:if>
      <xsl:call-template name="strings" />
    </Combo>
  </xsl:template>
  <xsl:template match="gui:ButtonRef" mode="placement">
    <CommandPlacement
      guid="{substring-before(me:MakeId(@id),':')}"
      id="{substring-after(me:MakeId(@id), ':')}"
      priority="{@priority}">
      <xsl:call-template name="parentRef" />
    </CommandPlacement>
  </xsl:template>
  <xsl:template match="gui:MenuRef | gui:GroupRef" mode="placement">
    <xsl:if test="
            (self::gui:MenuRef and (parent::gui:Group or parent::gui:GroupRef)) or
            (self::gui:GroupRef and (parent::gui:Menu or parent::gui:MenuRef))">
      <CommandPlacement
        guid="{substring-before(me:MakeId(@id),':')}"
        id="{substring-after(me:MakeId(@id), ':')}"
        priority="{@priority}">
        <xsl:call-template name="parentRef" />
      </CommandPlacement>

    </xsl:if>
  </xsl:template>
  <xsl:template name="keys">
    <xsl:variable name="key1">
      <xsl:choose>
        <xsl:when test="string-length(@key1) = 1">
          <xsl:value-of select="me:safeToUpper(@key1)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@key1"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="mod1">
      <xsl:if test="contains(@mod1, 'ctrl')">Control </xsl:if>
      <xsl:if test="contains(@mod1, 'alt')">Alt </xsl:if>
      <xsl:if test="contains(@mod1, 'shift')">Shift </xsl:if>
    </xsl:variable>
    <xsl:variable name="key2">
      <xsl:choose>
        <xsl:when test="string-length(@key2) = 1">
          <xsl:value-of select="me:safeToUpper(@key2)"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@key2"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
    <xsl:variable name="mod2">
      <xsl:if test="contains(@mod2, 'ctrl')">Control </xsl:if>
      <xsl:if test="contains(@mod2, 'alt')">Alt </xsl:if>
      <xsl:if test="contains(@mod2, 'shift')">Shift </xsl:if>
    </xsl:variable>
    <xsl:if test="@key1">
      <xsl:attribute name="key1">
        <xsl:value-of select="$key1"/>
      </xsl:attribute>

      <xsl:if test="string-length(normalize-space($mod1))">
        <xsl:attribute name="mod1">
          <xsl:value-of select="normalize-space($mod1)"/>
        </xsl:attribute>
      </xsl:if>
    </xsl:if>
    <xsl:if test="@key2">
      <xsl:attribute name="key2">
        <xsl:value-of select="$key2"/>
      </xsl:attribute>
      <xsl:if test="string-length(normalize-space($mod2))">
        <xsl:attribute name="mod2">
          <xsl:value-of select="normalize-space($mod2)"/>
        </xsl:attribute>
      </xsl:if>
    </xsl:if>
  </xsl:template>
  <xsl:template match="gui:KeyBinding">
    <KeyBinding
      guid="{substring-before(me:MakeId(../@id),':')}"
        id="{substring-after(me:MakeId(../@id), ':')}"
      editor="{@editor}"
      emulator="{@editor}">
      <xsl:call-template name="keys" />
    </KeyBinding>
  </xsl:template>
  <xsl:template match="gui:BindKey">
    <KeyBinding
      guid="{substring-before(me:MakeId(@id),':')}"
        id="{substring-after(me:MakeId(@id), ':')}"
      editor="{../@id}"
      emulator="{../@id}">
      <xsl:call-template name="keys" />
    </KeyBinding>
  </xsl:template>
  <xsl:template match="gui:Visibility[parent::gui:*/@id and @context]" mode="visibility">
    <VisibilityItem
       guid="{substring-before(me:MakeId(../@id),':')}"
       id="{substring-after(me:MakeId(../@id), ':')}"
       context="{@context}" />
  </xsl:template>
  <xsl:template match="gui:Command" mode="usedCommand">
    <UsedCommand
       guid="{substring-before(me:MakeId(@id),':')}"
      id="{substring-after(me:MakeId(@id), ':')}" />
  </xsl:template>
</xsl:stylesheet>
