<?xml version="1.0" encoding="UTF-8"?>
<!-- 
Simple XSL program for displaying use cases.
Copyright (c) 2002 Clear View Training Limited

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

Clear View Training email address: jim@clearviewtraining.com
Clear View Training web address: www.clearviewtraining.com
-->
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
	<xsl:template name="NumberFlowElement">
		<xsl:number format="1." level="multiple" count="FlowElement"/>
	</xsl:template>
	<!-- -->
	<xsl:template name="Indent">
	<!-- For debugging purposes. Prints out the amount of indent.
		<xsl:value-of select="count(ancestor::While | WhileElement | If | IfElement | For | ForElement)"/>
		-->
		<xsl:for-each select="ancestor::While | WhileElement | If | IfElement | For | ForElement">
			&#xa0;&#xa0;&#xa0;
		</xsl:for-each>
	</xsl:template>
	<!-- -->
	<xsl:template name="NumberWhileElement">
		<xsl:call-template name="Indent"/>
		<xsl:call-template name="NumberFlowElement"/>
		<xsl:number format="1" level="multiple" count="WhileElement"/>.
	</xsl:template>
	<!-- -->
	<xsl:template match="UseCase">
		<html>
			<head>
				<title>
					<xsl:value-of select="UseCase/Name"/>
				</title>
				<link rel="stylesheet" href="UseCaseStyle.css"/>
			</head>
			<body>
				<table border="1">
					<xsl:apply-templates/>
				</table>
			</body>
		</html>
	</xsl:template>
	<!-- -->
	<xsl:template match="Name">
		<tr>
			<td>Name:</td>
			<td>
				<xsl:value-of select="."/>
			</td>
		</tr>
	</xsl:template>
	<!-- -->
	<xsl:template match="ID">
		<tr>
			<td>ID</td>
			<td>
				<xsl:value-of select="."/>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="Actors">
		<tr>
			<td>Actors</td>
			<td>
				<ul>
					<xsl:apply-templates/>
				</ul>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="ActorID">
		<li>
			<xsl:value-of select="."/>
		</li>
	</xsl:template>
	<xsl:template match="Summary">
		<tr>
			<td>Summary</td>
			<td>
				<xsl:value-of select="."/>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="Relationships">
		<tr>
			<td>Relationships</td>
			<td>
				<ul>
					<xsl:apply-templates/>
				</ul>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="Relationship">
		<li>
			<xsl:value-of select="."/>
		</li>
	</xsl:template>
	<xsl:template match="MainFlow">
		<xsl:apply-templates/>
	</xsl:template>
	<xsl:template match="Preconditions">
		<tr>
			<td>Preconditions</td>
			<td>
				<ul>
					<xsl:apply-templates/>
				</ul>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="Precondition">
		<li>
			<xsl:value-of select="."/>
		</li>
	</xsl:template>
	<xsl:template match="Postconditions">
		<tr>
			<td>Postconditions</td>
			<td>
				<ul>
					<xsl:apply-templates/>
				</ul>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="Postcondition">
		<li>
			<xsl:value-of select="."/>
		</li>
	</xsl:template>
	<xsl:template match="FlowElements">
		<tr>
			<td colspan="2">Main flow</td>
		</tr>
		<tr>
			<td colspan="2">
				<xsl:apply-templates/>
			</td>
		</tr>
	</xsl:template>
	<xsl:template match="FlowElement">
		<p>
			<xsl:call-template name="NumberFlowElement"/>
			<xsl:apply-templates/>
		</p>
	</xsl:template>
	<xsl:template match="Step">
		<xsl:apply-templates/>
	</xsl:template>
	<xsl:template match="While">
			While <xsl:value-of select="@condition"/>
		<xsl:apply-templates/>
	</xsl:template>
	<xsl:template match="WhileElement">
		<p>
			<xsl:call-template name="NumberWhileElement"/>
			<xsl:apply-templates/>
		</p>
	</xsl:template>
</xsl:stylesheet>
