// $Id: ResolveConflictCommand.cs 1719 2004-10-04 09:01:35Z chris $
using System;
using EnvDTE;

using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;
using NSvn.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// Allows the user retrieve a file list of the selected item(s).
    /// </summary>
    [VSNetCommand( "FileListCommand", Text="Create a list of files...",  
         Bitmap = ResourceBitmaps.ResolveConflict, 
         Tooltip = "Resolve conflicted file"),
     VSNetControl( "Item.Ankh.Tools", Position = 1 ),
     VSNetProjectNodeControl( "Ankh.Tools", Position = 1 ),
     VSNetControl( "Solution.Ankh.Tools", Position = 1)]
    public class FileListCommand : CommandBase
    {    
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
          return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "Dateiliste speichern unter ...";
            fd.Filter = "Text-Datei (*.txt)|*.txt";
            fd.FilterIndex = 1;
            if (fd.ShowDialog() == DialogResult.OK) 
            {
                StreamWriter output = new StreamWriter(fd.FileName, false, System.Text.Encoding.UTF8);
                IList files = context.SolutionExplorer.GetSelectionFileNames();

                foreach ( string path in files )
                {
                    output.WriteLine(path);
                }

                output.Close();
            }
        }
    }
}




