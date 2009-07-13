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

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.VS
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ComStreamWrapper : Stream
    {
        readonly IStream _comStream;
        bool _disposed;
        bool _readOnly;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComStreamWrapper"/> class.
        /// </summary>
        /// <param name="comStream">The COM stream.</param>
        [CLSCompliant(false)]
        public ComStreamWrapper(IStream comStream)
            : this(comStream, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComStreamWrapper"/> class.
        /// </summary>
        /// <param name="comStream">The COM stream.</param>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        [CLSCompliant(false)]
        public ComStreamWrapper(IStream comStream, bool readOnly)
        {
            if (comStream == null)
                throw new ArgumentNullException("comStream");

            _comStream = comStream;
            _readOnly = readOnly;
        }


        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <value></value>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Position
        {
            get
            {
                return Seek(0, SeekOrigin.Current);
            }

            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite
        {
            get
            {
                return !_readOnly;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <value></value>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }


        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <value></value>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="T:System.NotSupportedException">A class derived from Stream does not support seeking. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Length
        {
            get
            {
                STATSTG[] sg = new STATSTG[1];
                _comStream.Stat(sg, (uint)(STATFLAG.STATFLAG_DEFAULT));

                return (long)sg[0].cbSize.QuadPart;                
            }
        }


        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        public override void Flush()
        {
            if (_disposed)
                throw new ObjectDisposedException("ComStreamWrapper");

            _comStream.Commit(0);
        }


        /// <summary>
        /// Reads the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            else if (_disposed)
                throw new ObjectDisposedException("ComStreamWrapper");

            uint bytesRead;
            byte[] b = buffer;

            if (index != 0)
            {
                b = new byte[buffer.Length - index];
                buffer.CopyTo(b, 0);
            }

            _comStream.Read(b, (uint)count, out bytesRead);

            if (index != 0)
            {
                b.CopyTo(buffer, index);
            }

            return (int)bytesRead;
        }


        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override void SetLength(long value)
        {
            if (_disposed)
                throw new ObjectDisposedException("ComStreamWrapper");
            else if (!CanWrite)
                throw new InvalidOperationException();

            ULARGE_INTEGER ul = new ULARGE_INTEGER();
            ul.QuadPart = (ulong)value;
            _comStream.SetSize(ul);
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
        /// <exception cref="T:System.NotSupportedException">The stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
        /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_disposed)
                throw new ObjectDisposedException("ComStreamWrapper");

            LARGE_INTEGER l = new LARGE_INTEGER();
            ULARGE_INTEGER[] ul = new ULARGE_INTEGER[1];
            ul[0] = new ULARGE_INTEGER();
            l.QuadPart = offset;
            _comStream.Seek(l, (uint)origin, ul);
            return (long)ul[0].QuadPart;
        }


        /// <summary>
        /// Writes the specified buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        public override void Write(byte[] buffer, int index, int count)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            else if (_disposed)
                throw new ObjectDisposedException("ComStreamWrapper");
            else if (!CanWrite)
                throw new InvalidOperationException();

            uint bytesWritten;

            if (count > 0)
            {

                byte[] b = buffer;

                if (index != 0)
                {
                    b = new byte[buffer.Length - index];
                    buffer.CopyTo(b, 0);
                }

                _comStream.Write(b, (uint)count, out bytesWritten);
                if (bytesWritten != count)
                    throw new IOException("Didn't write enough bytes to IStream!");  // @TODO: Localize this.

                if (index != 0)
                {
                    b.CopyTo(buffer, index);
                }
            }
        }


        /// <summary>
        /// Closes the current stream and releases any resources (such as sockets and file handles) associated with the current stream.
        /// </summary>
        public override void Close()
        {
            if (!_disposed)
                Flush();

            _disposed = true;
        }
    }

}
