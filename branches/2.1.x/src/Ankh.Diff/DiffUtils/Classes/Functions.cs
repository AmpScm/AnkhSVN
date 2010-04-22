// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

#region Copyright And Revision History

/*---------------------------------------------------------------------------

	Functions.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.IO;
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.Diagnostics;
using Microsoft.Win32;
using System.Windows.Forms;
using Ankh.Diff;
using System.Collections.ObjectModel;

namespace Ankh.Diff.DiffUtils
{
    /// <summary>
    /// Provides helper routines for working with files.
    /// </summary>
    public class Functions
    {
        #region Public Members

        public static bool IsBinaryFile(string strFileName)
        {
            using (FileStream F = File.OpenRead(strFileName))
            {
                return IsBinaryFile(F);
            }
        }

        public static bool IsBinaryFile(FileStream F)
        {
            F.Seek(0, SeekOrigin.Begin);

            //First see if the file begins with any known Unicode byte order marks.
            //If so, then it is a text file.  Use a StreamReader instance to do the
            //auto-detection logic.
            //
            //NOTE: I'm not disposing of the StreamReader because that closes the
            //associated Stream.  The caller opened the file stream, so they should
            //be the one to close it.
            StreamReader Reader = new StreamReader(F, Encoding.Default, true);
            Reader.Read(); //We have to force a Read for it to auto-detect.
            if (Reader.CurrentEncoding != Encoding.Default)
            {
                return false;
            }
            Reader.DiscardBufferedData();
            Reader = null;

            //Since the type was Default encoding, that means there were no byte-order
            //marks to indicate its type.  So we have to scan.  If we find a NULL
            //character in the stream, that means it's a binary file.
            F.Seek(0, SeekOrigin.Begin);
            int i;
            while ((i = F.ReadByte()) > -1)
            {
                if (i == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool AreFilesDifferent(string strFileName1, string strFileName2)
        {
            return AreFilesDifferent(new FileInfo(strFileName1), new FileInfo(strFileName2));
        }

        public static bool AreFilesDifferent(FileInfo Info1, FileInfo Info2)
        {
            //Before we open the files, compare the sizes.
            //If they are different, then the files are
            //certainly different.
            if (Info1.Length != Info2.Length)
            {
                return true;
            }

            using (FileStream FS1 = Info1.OpenRead())
            {
                using (FileStream FS2 = Info2.OpenRead())
                {
                    //The previous length check should ensure these are equal.
                    Debug.Assert(FS1.Length == FS2.Length);

                    //They have the same lengths, so we have
                    //to check byte-by-byte.  As soon as we
                    //find a difference, we can quit.
                    int iByte1, iByte2;
                    do
                    {
                        iByte1 = FS1.ReadByte();
                        iByte2 = FS2.ReadByte();

                        if (iByte1 != iByte2)
                        {
                            return true;
                        }
                    }
                    while (iByte1 >= 0 && iByte2 >= 0);

                    //The files were byte-by-byte equal.
                    return false;
                }
            }
        }

        public static Collection<string> GetFileTextLines(string strFileName)
        {
            using (StreamReader Reader = new StreamReader(strFileName, Encoding.Default, true))
            {
                return GetTextLines(Reader);
            }
        }

        public static Collection<string> GetStringTextLines(string strText)
        {
            using (StringReader Reader = new StringReader(strText))
            {
                return GetTextLines(Reader);
            }
        }

        public static Collection<string> GetTextLines(TextReader Reader)
        {
            Collection<string> Lines = new Collection<string>();

            while (Reader.Peek() > -1)
            {
                string strLine = Reader.ReadLine();
                Lines.Add(strLine);
            }

            return Lines;
        }

        public static StringCollection GetXMLTextLinesFromXML(string strXML, WhitespaceHandling eWS)
        {
            using (StringReader SR = new StringReader(strXML))
            using (XmlTextReader XR = new XmlTextReader(SR))
            {
                StringCollection Coll = Functions.GetXMLTextLines(XR, eWS);
                return Coll;
            }
        }

        public static StringCollection GetXMLTextLines(string strFileName, WhitespaceHandling eWS)
        {
            using (XmlTextReader Reader = new XmlTextReader(strFileName))
            {
                Reader.WhitespaceHandling = eWS;
                StringCollection Coll = GetXMLTextLines(Reader, eWS);
                return Coll;
            }
        }

        public static StringCollection GetXMLTextLines(XmlReader Reader, WhitespaceHandling eWS)
        {
            StringCollection Coll = new StringCollection();
            StringBuilder B = new StringBuilder();
            bool bTrimWSInSplit = eWS != WhitespaceHandling.All;

            //Read each node in the tree.
            string strIndent = "";
            int iCurrentDepth = 0;
            while (Reader.Read())
            {
                int iDepth = Reader.Depth;
                if (iDepth != iCurrentDepth)
                {
                    strIndent = GetIndentString(iDepth);
                    iCurrentDepth = iDepth;
                }

                switch (Reader.NodeType)
                {
                    case XmlNodeType.Attribute: //This should never be returned by XmlReader
                        Coll.Add(String.Format("{2}{0}={3}{1}{3}", Reader.Name, Reader.Value, strIndent, Reader.QuoteChar));
                        break;

                    case XmlNodeType.Comment:
                        SplitAndAddXMLLines(Coll, String.Format("<!-- {0} -->", Reader.Value), strIndent, bTrimWSInSplit);
                        break;

                    case XmlNodeType.Element:
                        B.Length = 0;
                        B.AppendFormat("{1}<{0}", Reader.Name, strIndent);
                        //We have to check for this before we move to the attributes.
                        bool bIsEmptyElement = Reader.IsEmptyElement;
                        while (Reader.MoveToNextAttribute())
                        {
                            B.AppendFormat(" {0}={2}{1}{2}", Reader.Name, Reader.Value, Reader.QuoteChar);
                        }
                        if (bIsEmptyElement)
                            B.Append("/>");
                        else
                            B.Append(">");
                        Coll.Add(B.ToString());
                        break;

                    case XmlNodeType.EndElement:
                        Coll.Add(String.Format("{1}</{0}>", Reader.Name, strIndent));
                        break;

                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.XmlDeclaration:
                        Coll.Add(String.Format("{2}<?{0} {1}?>", Reader.Name, Reader.Value, strIndent));
                        break;

                    case XmlNodeType.SignificantWhitespace:
                        if (eWS != WhitespaceHandling.None)
                        {
                            Coll.Add(String.Format("{1}{0}", Reader.Value, strIndent));
                        }
                        break;

                    case XmlNodeType.Whitespace:
                        if (eWS == WhitespaceHandling.All)
                        {
                            Coll.Add(String.Format("{1}{0}", Reader.Value, strIndent));
                        }
                        break;

                    case XmlNodeType.CDATA:
                        Coll.Add(String.Format("{1}<![CDATA[{0}]]>", Reader.Value, strIndent));
                        break;

                    case XmlNodeType.Document:
                    case XmlNodeType.DocumentFragment:
                        Coll.Add(String.Format("{1}{0}", Reader.Value, strIndent));
                        break;

                    case XmlNodeType.DocumentType:
                        Coll.Add(String.Format("{1}<!DOCTYPE {0} [", Reader.Name, strIndent));
                        SplitAndAddXMLLines(Coll, Reader.Value, GetIndentString(iDepth + 1), bTrimWSInSplit);
                        Coll.Add(String.Format("{0}]>", strIndent));
                        break;

                    case XmlNodeType.Entity:
                        Coll.Add(String.Format("{2}<!ENTITY {0} [{1}]", Reader.Name, Reader.Value, strIndent));
                        SplitAndAddXMLLines(Coll, Reader.Value, GetIndentString(iDepth + 1), bTrimWSInSplit);
                        Coll.Add(String.Format("{0}]>", strIndent));
                        break;

                    case XmlNodeType.EntityReference:
                        Coll.Add(String.Format("{1}&{0}", Reader.Value, strIndent));
                        break;

                    case XmlNodeType.Notation:
                        Coll.Add(String.Format("{2}<!NOTATION {0} [{1}]>", Reader.Name, Reader.Value, strIndent));
                        break;

                    case XmlNodeType.EndEntity:
                    case XmlNodeType.None:
                    case XmlNodeType.Text:
                        SplitAndAddXMLLines(Coll, Reader.Value, strIndent, bTrimWSInSplit);
                        break;
                }
            }

            return Coll;
        }

        #endregion

        #region Private Methods

        private static void SplitAndAddXMLLines(StringCollection Coll, string strText, string strIndent, bool bTrimWSInSplit)
        {
            using (StringReader R = new StringReader(strText))
            {
                while (R.Peek() > -1)
                {
                    string strLine = R.ReadLine();
                    if (bTrimWSInSplit)
                    {
                        strLine = strLine.Trim();
                        if (strLine.Length == 0)
                        {
                            continue;
                        }
                    }

                    string strIndentedLine = String.Format("{0}{1}", strIndent, strLine);
                    Coll.Add(strIndentedLine);
                }
            }
        }

        private static string GetIndentString(int iDepth)
        {
            switch (iDepth)
            {
                case 0: return "";
                case 1: return "\t";
                case 2: return "\t\t";
                case 3: return "\t\t\t";
                case 4: return "\t\t\t\t";
                case 5: return "\t\t\t\t\t";
                case 6: return "\t\t\t\t\t\t";
                case 7: return "\t\t\t\t\t\t\t";
                case 8: return "\t\t\t\t\t\t\t\t";
                case 9: return "\t\t\t\t\t\t\t\t\t";
                default:
                    {
                        StringBuilder B = new StringBuilder();
                        for (int i = 0; i < iDepth; i++)
                        {
                            B.Append("\t");
                        }
                        return B.ToString();
                    }
            }
        }

        private Functions()
        {
            //Private constructor so no one can create an instance.
        }

        #endregion
    }
}
