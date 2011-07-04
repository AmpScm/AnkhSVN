// $Id$
//
// Copyright 2004-2008 The AnkhSVN Project
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

using System.IO;
using System.Threading;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System;

namespace Utils
{
    public sealed class ProcessReader
    {
        public ProcessReader( StreamReader reader )
        {
            this.reader = reader;
            this.queueNonEmpty = new ManualResetEvent( false );
            this.output = new StringBuilder();
            this.queue = new Queue();
        }
            
        public void Start()
        {
            this.thread = new Thread( new ThreadStart( this.Read ) );
            this.thread.Start();
        }

        public void Wait()
        {
            this.thread.Join();
        }

        public string Output
        {
            get{ return this.output.ToString(); }
        }

        /// <summary>
        /// Whether the reader thread has exited.
        /// </summary>
        public bool HasExited
        {
            get
            { 
                return !this.thread.IsAlive; 
            }
        }

        /// <summary>
        /// Whether the queue is empty.
        /// </summary>
        public bool Empty
        {
            get
            {
                lock(this.queue)
                {
                    return this.queue.Count == 0;
                }
            }
        }

        /// <summary>
        /// Retrieves the 'next' line, blocking if necessary. Use in conjunction with the
        /// WaitHandle, which will be signaled when the queue is non-empty.
        /// </summary>
        /// <returns></returns>
        public string ReadLine()
        {
            string retval;
            lock(this.queue)
            {
                Debug.WriteLine( "ReadLine() waiting on event", "Ankh" );
                this.queueNonEmpty.WaitOne();
                Debug.WriteLine( "ReadLine() event signaled", "Ankh" );

                retval = (string)this.queue.Dequeue();
                if ( this.queue.Count == 0 )
                {
                    Debug.WriteLine( "ReadLine() resetting event: queue is empty" );
                    this.queueNonEmpty.Reset();
                }
            }
            return retval;
        }

        /// <summary>
        /// Wait handle to wait on the queue being filled.
        /// </summary>
        public WaitHandle WaitHandle
        {
            get { return this.queueNonEmpty; }
        }

        private void Read()
        {
            string line = null;
            while( (line = reader.ReadLine()) != null )
            {
                this.output.Append( line + Environment.NewLine );
                lock(this.queue)
                {
                    this.queue.Enqueue( line );

                    // the queue should now be non-empty.
                    this.queueNonEmpty.Set();

                    // yield control.
                    Thread.Sleep(0);
                }
            }
        }
            

        private Queue queue;
        private StreamReader reader;
        private Thread thread;
        private StringBuilder output;
        private ManualResetEvent queueNonEmpty;
    }
}
