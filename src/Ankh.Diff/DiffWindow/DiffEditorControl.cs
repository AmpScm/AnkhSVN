// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using Ankh.Scc.UI;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.ObjectModel;
using System.IO;
using Ankh.Diff.DiffUtils;
using Ankh.Diff;

namespace Ankh.UI.DiffWindow
{
    public partial class DiffEditorControl : VSEditorControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiffControl"/> class.
        /// </summary>
        public DiffEditorControl()
        {
            InitializeComponent();
        }

        IAnkhPackage Package
        {
            get { return Context.GetService<IAnkhPackage>(); }
        }

        /// <summary>
        /// Called when the frame is created
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            CommandContext = AnkhId.DiffMergeContextGuid;
            KeyboardContext = AnkhId.DiffMergeContextGuid;            
        }

        protected override void OnLoad(EventArgs e)
        {
            ToolStripRenderer renderer = null;
            System.Windows.Forms.Design.IUIService ds = Context.GetService<System.Windows.Forms.Design.IUIService>();
            if (ds != null)
            {
                renderer = ds.Styles["VsToolWindowRenderer"] as ToolStripRenderer;
            }

            if (renderer != null)
                diffControl1.ToolStripRenderer = renderer;

            base.OnLoad(e);
        }

        private void GetFileLines(string strA, string strB, out Collection<string> A, out Collection<string> B)
        {
            A = File.Exists(strA) ? Functions.GetFileTextLines(strA) : Functions.GetStringTextLines(string.Empty);
            B = File.Exists(strB) ? Functions.GetFileTextLines(strB) : Functions.GetStringTextLines(string.Empty);
        }

        internal void CreateDiffEditor(IAnkhServiceProvider context, AnkhDiffArgs args)
        {
            Context = context;

            DynamicFactory.CreateEditor(args.BaseFile, this);
            OnFrameCreated(EventArgs.Empty);

            Collection<string> A, B;
            GetFileLines(args.BaseFile, args.MineFile, out A, out B);
            TextDiff Diff = new TextDiff(HashType.HashCode, false, false);
            EditScript Script = Diff.Execute(A, B);

            string strCaptionA = args.BaseTitle ?? Path.GetFileName(args.BaseFile);
            string strCaptionB = args.MineTitle ?? Path.GetFileName(args.MineFile);
            diffControl1.SetData(A, B, Script, strCaptionA, strCaptionB);
        }
    }
}
