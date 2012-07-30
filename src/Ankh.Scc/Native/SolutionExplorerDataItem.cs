// Original: Copyright © Microsoft Corporation.  All Rights Reserved.
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
            /// <summary>
            /// IDataObject format string for project items from solution explorer.
            /// </summary>
            static readonly string[] SupportedFormats = new string[] { "CF_VSSTGPROJECTITEMS", "CF_VSREFPROJECTITEMS", "CF_VSSTGPROJECTS", "CF_VSREFPROJECTS" };

            /// <summary>
            /// Returns a boolean to indicate if a compatible format is available
            /// </summary>
            /// <param name="dataObject"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public static bool CanRead(IDataObject dataObject, out string type)
            {
                foreach (string format in SupportedFormats)
                {
                    if (dataObject.GetDataPresent(format))
                    {
                        type = format;
                        return true;
                    }
                }
                type = null;
                return false;
            }
            /// <summary>
            /// Decode an IDataObject from Solution Explorer.
            /// </summary>
            /// <param name="data">The data object.</param>
            /// <param name="format">The key to get the memory stream  from data</param>
            /// <returns>A list of ProjRef strings.</returns>
            public static IList<string> DecodeProjectItemData(IDataObject data, string format)
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

                using (MemoryStream stream = data.GetData(format) as MemoryStream)
                {
                    if (stream == null || stream.Length <= 22 || !stream.CanRead)
                        return new string[0];

                    Encoding encoding = (System.BitConverter.IsLittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode);
                    BinaryReader reader = new BinaryReader(stream, encoding);

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
                    List<string> items = new List<string>();
                    StringBuilder sb = new StringBuilder();

                    while (stream.Position < stream.Length)
                    {
                        char c = reader.ReadChar();
                        if (c == '\0' && lastChar == '\0')
                        {
                            break;
                        }

                        if (c == '\0')
                        {
                            items.Add(sb.ToString());
                            sb.Clear();
                        }
                        else
                        {
                            sb.Append(c);
                        }

                        lastChar = c;
                    }

                    return items.AsReadOnly();
                }
            }
        }
}
