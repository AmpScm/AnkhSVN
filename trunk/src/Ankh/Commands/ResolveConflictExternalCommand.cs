// $Id: ResolveConflictCommand.cs 1580 2004-07-24 01:44:31Z Arild $
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
    /// Allows the user to resolve a conflicted file.
    /// </summary>
    [VSNetCommand( "ResolveConflictExternalCommand", Text="Resolve conflicted file using external editor...",  
         Bitmap = ResourceBitmaps.ResolveConflict, 
         Tooltip = "Resolve conflicted file using external editor"),
     VSNetControl( "Item.Ankh", Position = 1 ),
     VSNetProjectNodeControl( "Ankh", Position = 1 ),
     VSNetControl( "Solution.Ankh", Position = 1)]
    public class ResolveConflictExternalCommand : ResolveConflictCommand
    {    
        /// <summary>
        /// Gets path to the diff executable while taking care of config file settings.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The exe path.</returns>
        private string GetExe( Ankh.IContext context )
        {
            return context.Config.MergeExePath;
        }
    }
}




