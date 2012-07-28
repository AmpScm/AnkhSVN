﻿// Original: Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Apache License, Version 2.0. Please see http://www.apache.org/licenses/LICENSE-2.0.html

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Ankh.Scc
{
        /// <summary>
        /// A class containing the data for each of the drag-dropped Solution Explorer node
        /// </summary>
        sealed class SolutionExplorerClipboardItem
        {
            Guid _projectGuid;
            string _projectFileName;
            string _fileName;
            bool _nodeIsProject;

            /// <summary>
            /// IDataObject format string for project items from solution explorer.
            /// </summary>
            internal const string ClipFormatProjectItem = "CF_VSSTGPROJECTITEMS";

            /// <summary>
            /// Very arcane method to decode an IDataObject from Solution Explorer.
            /// </summary>
            /// <param name="data">The data object.</param>
            /// <param name="streamKey">The key to get the memory stream  from data</param>
            /// <param name="nodeIsProject">True if the given node is a project node.</param>
            /// <returns>A list of SolutionExplorerNodeData objects.</returns>
            public static IList<SolutionExplorerClipboardItem> DecodeProjectItemData(IDataObject data, bool nodeIsProject)
            {
                /*
                 This function reads the memory stream in the data object and parses the data.
                 The structure of the data is as follows:
                DROPFILES structure (20 bytes)
                String\0
                String\0
                ..
                String\0\0
       
                One String for each drag-dropped Soln Explorer node. 
                The fWide member in the DROPFILES structure tells us if the string is encoded in Unicode or ASCII.
                And each string contains the following:
                {project guid} +"|"+ project file name +"|"+ drag-dropped file name

                The exact format is documented as part of the documentation of IVsSolution.GetItemOfProjref()
                which is the API to parse these strings.
                */

                MemoryStream stream = data.GetData(ClipFormatProjectItem) as MemoryStream;
                List<SolutionExplorerClipboardItem> nodeData = new List<SolutionExplorerClipboardItem>();
                if (stream != null && stream.Length > 0 && stream.CanRead)
                {
                    BinaryReader reader = null;
                    try
                    {
                        Encoding encoding = (System.BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode);
                        reader = new BinaryReader(stream, encoding);

                        // Read the initial DROPFILES struct (20 bytes)
                        Int32 files = reader.ReadInt32();
                        Int32 x = reader.ReadInt32();
                        Int32 y = reader.ReadInt32();
                        Int32 unused = reader.ReadInt32(); // This is not used anywhere, but is read out of the way.
                        Int32 unicode = reader.ReadInt32();

                        // If the string is not unicode, use ASCII encoding
                        if (unicode == 0)
                        {
                            reader = new BinaryReader(stream, Encoding.ASCII);
                        }

                        char lastChar = '\0';
                        List<string> eachNodeStrings = new List<string>();
                        StringBuilder fileNameString = new StringBuilder();
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            char c = reader.ReadChar();
                            if (c == '\0' && lastChar == '\0')
                            {
                                break;
                            }

                            if (c == '\0')
                            {
                                eachNodeStrings.Add(fileNameString.ToString());
                                fileNameString = new StringBuilder();
                            }
                            else
                            {
                                fileNameString.Append(c);
                            }

                            lastChar = c;
                        }

                        foreach (string eachNode in eachNodeStrings)
                        {
                            string[] items = eachNode.Split('|');

                            if (items.Length != 3)
                                continue;

                            SolutionExplorerClipboardItem node = new SolutionExplorerClipboardItem();
                            node.ProjectGuid = new Guid(items[0]);
                            node.ProjectFileName = items[1];

                            // For PROJECTITEM data structures, the file name portion has been run through ToLower (or equivalent)
                            // but the IDataObject will also have a "System.String" data type which is still in the correct case
                            // so we will use the System.String data for PROJECTITEM objects. If there's more than one item in the
                            // data string then just use the path embedded in the data since the System.String value depends
                            // on what was selected first in the multiple selection. No, I didn't write this.
                            if (nodeIsProject || !data.GetDataPresent("System.String") || (1 != eachNodeStrings.Count))
                            {
                                node.FileName = items[2];
                            }
                            else
                            {
                                node.FileName = data.GetData("System.String") as String;
                            }

                            node.IsProjectNode = nodeIsProject;
                            nodeData.Add(node);
                        }
                    }
                    finally
                    {
                        if (reader != null)
                        {
                            reader.Close();
                        }
                    }
                }

                return nodeData;
            }

            /// <summary>
            /// The guid of the project 
            /// </summary>
            public Guid ProjectGuid
            {
                get { return _projectGuid; }
                private set { _projectGuid = value; }
            }

            /// <summary>
            /// Path of the project file
            /// </summary>
            /// <value></value>
            public string ProjectFileName
            {
                get { return _projectFileName; }
                private set { _projectFileName = value; }
            }

            /// <summary>
            /// Path of the file that is drag-dropped
            /// </summary>
            /// <value></value>
            public string FileName
            {
                get { return _fileName; }
                private set { _fileName = value; }
            }

            /// <summary>
            /// Whether the node is a project node or not
            /// </summary>
            public bool IsProjectNode
            {
                get { return _nodeIsProject; }
                private set { _nodeIsProject = value; }
            }

            #region Equality Methods
            /// <summary>
            /// Object.Equals().
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                SolutionExplorerClipboardItem other = obj as SolutionExplorerClipboardItem;

                return (this == other);
            }

            /// <summary>
            /// Calculate GetHashCode of this struct
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return StringComparer.Ordinal.GetHashCode(FileName);
            }

            /// <summary>
            /// Operator !=.
            /// </summary>
            /// <param name="data1"></param>
            /// <param name="data2"></param>
            /// <returns></returns>
            public static bool operator !=(SolutionExplorerClipboardItem data1, SolutionExplorerClipboardItem data2)
            {
                return !(data1 == data2);
            }

            /// <summary>
            /// Operator ==.
            /// </summary>
            /// <param name="data1"></param>
            /// <param name="data2"></param>
            /// <returns></returns>
            public static bool operator ==(SolutionExplorerClipboardItem data1, SolutionExplorerClipboardItem data2)
            {
                bool n1 = ((object)data1 == null);
                bool n2 = ((object)data2 == null);

                if (n1 || n2)
                    return (n1 && n2);

                return (data1.IsProjectNode == data2.IsProjectNode &&
                        data1.ProjectGuid == data2.ProjectGuid &&
                        data1.FileName == data2.FileName &&
                        data1.ProjectFileName == data2.ProjectFileName);
            }
            #endregion
        }

}
