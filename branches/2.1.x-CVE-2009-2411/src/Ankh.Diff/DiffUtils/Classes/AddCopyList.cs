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

	AddCopyList.cs
	Copyright (c) 2002 Bill Menees.  All rights reserved.
	Bill@Menees.com

	Who		When		What
	-------	----------	-----------------------------------------------------
	BMenees	10.20.2002	Created.

-----------------------------------------------------------------------------*/

#endregion

using System;
using System.IO;
using System.Collections;

namespace Ankh.Diff.DiffUtils
{
    public class Addition
    {
        public byte[] arBytes;
    }

    public class Copy
    {
        public int iBaseOffset;
        public int iLength;
    }

    //This inherits from ArrayList instead of CollectionBase because
    //its not strongly typed.  It can contain Additions and Copies.
    public class AddCopyList : ArrayList
    {
        #region Public Members

        //Gets the total byte length of all of the adds and copies.
        //This should equal the file size of the output file.
        public int TotalByteLength
        {
            get
            {
                int iTotal = 0;
                foreach (object o in this)
                {
                    Addition A = o as Addition;
                    if (A != null)
                    {
                        iTotal += A.arBytes.Length;
                    }
                    else
                    {
                        Copy C = (Copy)o;
                        iTotal += C.iLength;
                    }
                }
                return iTotal;
            }
        }

        /// <summary>
        /// This outputs the Add/Copy info to a stream in GDIFF format.
        /// </summary>
        /// <param name="Diff">The stream to dump the diff info to.  It must support at least forward-only writing.</param>
        public void GDIFF(Stream Diff)
        {
            //http://www.w3.org/TR/NOTE-gdiff-19970825.html
            //
            //The GDIFF format is a binary format. The mime type of a GDIFF file is "application/gdiff". 
            //All binary numbers in a GDIFF file are stored in big endian format (most significant byte first). 
            //Each diff stream starts with the 4-byte magic number (value 0xd1ffd1ff), followed by a 1-byte 
            //version number (value 4). The version number is followed by a sequence of 1 byte commands which 
            //are interpreted in order. The last command in the stream is the end-of-file command (value 0). 
            //
            //byte - 8 bit signed 
            //ubyte - 8 bit unsigned 
            //ushort - 16 bit unsigned, most significant byte first 
            //int - 32 bit signed, most significant byte first 
            //long - 64 bit signed, most significant byte first 

            //Write the magic number 0xd1ffd1ff ("diff diff")
            for (int i = 0; i < 2; i++)
            {
                Diff.WriteByte(0xd1);
                Diff.WriteByte(0xff);
            }

            //Write the version
            Diff.WriteByte(0x04);

            //Write the data
            foreach (object o in this)
            {
                if (o is Addition)
                    GDIFFAdd(Diff, (Addition)o);
                else
                    GDIFFCopy(Diff, (Copy)o);
            }

            //Write the end-of-file command
            Diff.WriteByte(0x00);
        }

        #endregion

        #region Private Members

        private void GDIFFAdd(Stream Diff, Addition Add)
        {
            //Name	Cmd		Followed By			Action
            //-----------------------------------------------------------
            //DATA	1		1 byte				append 1 data byte
            //DATA	2		2 bytes				append 2 data bytes
            //DATA	<n>		<n> bytes			append <n> data bytes
            //DATA	246		246 bytes			append 246 data bytes
            //DATA	247		ushort, <n> bytes	append <n> data bytes
            //DATA	248		int, <n> bytes		append <n> data bytes

            int iLength = Add.arBytes.Length;
            if (iLength <= 246)
            {
                Diff.WriteByte((byte)iLength);
                Diff.Write(Add.arBytes, 0, iLength);
            }
            else if (iLength <= ushort.MaxValue)
            {
                Diff.WriteByte(247);
                WriteBigEndian(Diff, (ushort)iLength);
                Diff.Write(Add.arBytes, 0, iLength);
            }
            else
            {
                Diff.WriteByte(248);
                WriteBigEndian(Diff, iLength);
                Diff.Write(Add.arBytes, 0, iLength);
            }
        }

        private void GDIFFCopy(Stream Diff, Copy C)
        {
            //Name	Cmd		Followed By			Action
            //-----------------------------------------------------------
            //COPY	249		ushort, ubyte		copy <position>, <length>
            //COPY	250		ushort, ushort		copy <position>, <length>
            //COPY	251		ushort, int			copy <position>, <length>
            //COPY	252		int, ubyte			copy <position>, <length>
            //COPY	253		int, ushort			copy <position>, <length>
            //COPY	254		int, int			copy <position>, <length>
            //COPY	255		long, int			copy <position>, <length>

            if (C.iBaseOffset <= ushort.MaxValue)
            {
                if (C.iLength <= byte.MaxValue)
                {
                    Diff.WriteByte(249);
                    WriteBigEndian(Diff, (ushort)C.iBaseOffset);
                    Diff.WriteByte((byte)C.iLength);
                }
                else if (C.iLength <= ushort.MaxValue)
                {
                    Diff.WriteByte(250);
                    WriteBigEndian(Diff, (ushort)C.iBaseOffset);
                    WriteBigEndian(Diff, (ushort)C.iLength);
                }
                else
                {
                    Diff.WriteByte(251);
                    WriteBigEndian(Diff, (ushort)C.iBaseOffset);
                    WriteBigEndian(Diff, C.iLength);
                }
            }
            else
            {
                if (C.iLength <= byte.MaxValue)
                {
                    Diff.WriteByte(252);
                    WriteBigEndian(Diff, C.iBaseOffset);
                    Diff.WriteByte((byte)C.iLength);
                }
                else if (C.iLength <= ushort.MaxValue)
                {
                    Diff.WriteByte(253);
                    WriteBigEndian(Diff, C.iBaseOffset);
                    WriteBigEndian(Diff, (ushort)C.iLength);
                }
                else
                {
                    Diff.WriteByte(254);
                    WriteBigEndian(Diff, C.iBaseOffset);
                    WriteBigEndian(Diff, C.iLength);
                }
            }
        }

        private void WriteBigEndian(Stream Diff, ushort us)
        {
            WriteBigEndian(Diff, BitConverter.GetBytes(us));
        }

        private void WriteBigEndian(Stream Diff, int i)
        {
            WriteBigEndian(Diff, BitConverter.GetBytes(i));
        }

        private void WriteBigEndian(Stream Diff, byte[] arBytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                for (int i = arBytes.Length - 1; i >= 0; i--)
                {
                    Diff.WriteByte(arBytes[i]);
                }
            }
            else
            {
                Diff.Write(arBytes, 0, arBytes.Length);
            }
        }

        #endregion
    }
}
