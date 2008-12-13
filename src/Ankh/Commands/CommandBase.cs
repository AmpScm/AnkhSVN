// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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

using Ankh.UI;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Ankh.Selection;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Base class for ICommand instances
    /// </summary>
    public abstract class CommandBase : Ankh.Commands.ICommandHandler
    {
        public virtual void OnUpdate(CommandUpdateEventArgs e)
        {
            // Just leave the defaults: Enabled          
        }

        public abstract void OnExecute(CommandEventArgs e);

        /// <summary>
        /// Gets whether the Shift key was down when the current window message was send
        /// </summary>
        public static bool Shift
        {
            get
            {
                return (0 != (System.Windows.Forms.Control.ModifierKeys & System.Windows.Forms.Keys.Shift));
            }
        }

        protected static void SaveAllDirtyDocuments(ISelectionContext selection, IAnkhServiceProvider context)
        {
            if (selection == null)
                throw new ArgumentNullException("selection");
            if (context == null)
                throw new ArgumentNullException("context");

            IAnkhOpenDocumentTracker tracker = context.GetService<IAnkhOpenDocumentTracker>();
            if (selection != null && tracker != null)
                tracker.SaveDocuments(selection.GetSelectedFiles(true));
        }

        internal static XslCompiledTransform GetTransform(IAnkhServiceProvider context, string name)
        {

            // get the embedded resource and copy it to path
            string resourceName = "Ankh.Commands." + name;
            using (Stream s =
                typeof(CommandBase).Assembly.GetManifestResourceStream(resourceName))
            {
                XPathDocument doc = new XPathDocument(s);

                // TODO: Transforms should be cached as a dynamic assembly is created
                // which stays in memory until the appdomain closes
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(doc);

                return transform;
            }
        }

        protected static void CreateTransformFile(string path, string name)
        {
            // get the embedded resource and copy it to path
            string resourceName = "Ankh.Commands." + name;
            Stream ins =
                typeof(CommandBase).Assembly.GetManifestResourceStream(resourceName);
            int len;
            byte[] buffer = new byte[4096];
            using (FileStream outs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                while ((len = ins.Read(buffer, 0, 4096)) > 0)
                {
                    outs.Write(buffer, 0, len);
                }
            }
        }
    }
}
