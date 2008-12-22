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
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using Ankh.Scc;
using System.Windows.Forms.Design;
using System.IO;
using SharpSvn;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to save currnet file to disk from Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.SaveToFile, AlwaysAvailable=true)]
    class SaveToFileCommand : ViewRepositoryFileCommand
    {
        public override void OnExecute(CommandEventArgs e)
        {
            ISvnRepositoryItem ri = null;

            foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                ri = i;
                break;
            }
            if (ri == null)
                return;

            IUIService ui = e.GetService<IUIService>();

            string toFile;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                string name = ri.Origin.Target.FileName;

                sfd.Filter = "All Files (*.*)|*";
                string ext = Path.GetExtension(name).Trim('.');                

                if(!string.IsNullOrEmpty(ext))
                    sfd.Filter = string.Format("{0} Files|*.{0}|{1}", ext, sfd.Filter);

                sfd.FileName = name;

                if (sfd.ShowDialog(ui.GetDialogOwnerWindow()) != DialogResult.OK)
                    return;

                toFile = sfd.FileName;
            }

            SaveFile(e, ri, toFile);
        }
    }
}
