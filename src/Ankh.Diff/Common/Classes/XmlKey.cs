#region Copyright And Revision History

/*---------------------------------------------------------------------------

	XmlKey.cs
	Copyright © 2003 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	4.26.2003	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.Xml;
using System.Collections;
using Microsoft.JScript;
using System.Drawing;

namespace Ankh.Diff
{
    public sealed class XmlKey
    {
        public XmlKey(XmlDocument Doc)
        {
            m_Node = Doc.DocumentElement;
        }

        public XmlKey(XmlNode Node)
        {
            m_Node = Node;
        }

        public XmlNode Node
        {
            get
            {
                return m_Node;
            }
        }

        public string KeyType
        {
            get
            {
                return m_Node.Name;
            }
        }

        public string XmlKeyName
        {
            get
            {
                return GetString("XmlKeyName", "");
            }
        }

        public string GetString(string strName, string strDefault)
        {
            XmlAttribute Attr = m_Node.Attributes[strName];
            if (Attr != null)
            {
                return Unescape(Attr.Value);
            }
            else
            {
                return strDefault;
            }
        }

        public void SetString(string strName, string strValue)
        {
            SetNodeAttribute(m_Node, strName, strValue);
        }

        public int GetInt(string strName, int iDefault)
        {
            XmlAttribute Attr = m_Node.Attributes[strName];
            if (Attr != null)
            {
                return Int32.Parse(Attr.Value);
            }
            else
            {
                return iDefault;
            }
        }

        public void SetInt(string strName, int iValue)
        {
            SetString(strName, iValue.ToString());
        }

        public bool GetBool(string strName, bool bDefault)
        {
            return GetInt(strName, bDefault ? 1 : 0) != 0;
        }

        public void SetBool(string strName, bool bValue)
        {
            SetInt(strName, bValue ? 1 : 0);
        }

        public Color GetColor(string strName, Color clrDefault)
        {
            return Color.FromArgb(GetInt(strName, clrDefault.ToArgb()));
        }

        public void SetColor(string strName, Color clrValue)
        {
            SetInt(strName, clrValue.ToArgb());
        }

        public bool ValueExists(string strName)
        {
            return m_Node.Attributes[strName] != null;
        }

        public XmlKey[] GetSubKeys()
        {
            ArrayList lstKeys = new ArrayList();
            if (m_Node.HasChildNodes)
            {
                foreach (XmlNode Node in m_Node.ChildNodes)
                {
                    if (Node.NodeType == XmlNodeType.Element)
                    {
                        lstKeys.Add(new XmlKey(Node));
                    }
                }
            }

            XmlKey[] arKeys = new XmlKey[lstKeys.Count];
            lstKeys.CopyTo(arKeys, 0);
            return arKeys;
        }

        public string[] GetValueNames()
        {
            XmlAttributeCollection Attributes = m_Node.Attributes;
            int iNumAttributes = Attributes.Count;
            string[] arNames = new string[iNumAttributes];
            for (int i = 0; i < iNumAttributes; i++)
            {
                arNames[i] = Attributes[i].Name;
            }

            return arNames;
        }

        public XmlKey GetSubKey(string strSubKeyType, string strXmlKeyName)
        {
            return new XmlKey(GetSubKeyNode(strSubKeyType, strXmlKeyName, false));
        }

        public XmlKey AddSubKey(string strSubKeyType, string strXmlKeyName)
        {
            return new XmlKey(AddSubKeyNode(strSubKeyType, strXmlKeyName));
        }

        public void DeleteSubKey(string strSubKeyType, string strXmlKeyName)
        {
            XmlNode Node = GetSubKeyNode(strSubKeyType, strXmlKeyName, true);
            if (Node != null)
            {
                m_Node.RemoveChild(Node);
            }
        }

        public void DeleteValue(string strName)
        {
            m_Node.Attributes.RemoveNamedItem(strName);
        }

        private XmlNode GetSubKeyNode(string strSubKeyType, string strXmlKeyName, bool bAllowNull)
        {
            string strSelect = String.Format("{0}[@XmlKeyName=\"{1}\"]", strSubKeyType, strXmlKeyName);
            XmlNode SubKeyNode = m_Node.SelectSingleNode(strSelect);
            if (SubKeyNode == null && !bAllowNull)
            {
                SubKeyNode = AddSubKeyNode(strSubKeyType, strXmlKeyName);
            }

            return SubKeyNode;
        }

        private XmlNode AddSubKeyNode(string strSubKeyType, string strXmlKeyName)
        {
            XmlNode SubKeyNode = m_Node.OwnerDocument.CreateElement(strSubKeyType);
            SetNodeAttribute(SubKeyNode, "XmlKeyName", strXmlKeyName);
            m_Node.AppendChild(SubKeyNode);
            return SubKeyNode;
        }

        private void SetNodeAttribute(XmlNode Node, string strName, string strValue)
        {
            XmlAttribute Attr = Node.OwnerDocument.CreateAttribute(strName);
            Attr.Value = Escape(strValue);
            Node.Attributes.SetNamedItem(Attr);
        }

        private string Escape(string strValue)
        {
            return GlobalObject.escape(strValue);
        }

        private string Unescape(string strValue)
        {
            return GlobalObject.unescape(strValue);
        }

        private XmlNode m_Node;
    }
}
