// $Id$
using System;
using EnvDTE;
using NSvn;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Ankh.Commands
{
    /// <summary>
    /// Allows the user to resolve a conflicted file.
    /// </summary>
    [VSNetCommand( "ResolveConflict", Text="Resolve",  Bitmap = ResourceBitmaps.Default, 
         Tooltip = "Resolve conflicted file"),
     VSNetControl( "Item.Ankh", Position = 1 ),
     VSNetControl( "Project.Ankh", Position = 1 ),
     VSNetControl( "Solution.Ankh", Position = 1)]
    internal class ResolveConflictCommand : CommandBase
    {    
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            ConflictedVisitor v = new ConflictedVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, false );

            if ( v.Conflicted )
                return vsCommandStatus.vsCommandStatusEnabled |
                    vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(AnkhContext context)
        {
            ResolveVisitor v = new ResolveVisitor( context );
            context.SolutionExplorer.VisitSelectedItems( v, false );
            context.SolutionExplorer.RefreshSelection();
        }

        private class ResolveVisitor : LocalResourceVisitorBase
        {
            public ResolveVisitor( AnkhContext context )
            {
                this.context = context;
            }
        
            public override void VisitWorkingCopyFile(WorkingCopyFile file)
            {
                try
                {
                    int oldRev, newRev;
                    this.GetRevisions( file, out oldRev, out newRev );

                    ConflictDialog.Choice selection;

                    using( ConflictDialog dialog = new ConflictDialog(  ) )
                    {
                        dialog.OldRev = oldRev;
                        dialog.NewRev = newRev;
                        dialog.Filename = file.Path;

                        if ( dialog.ShowDialog( this.context.HostWindow ) != DialogResult.OK )
                            return;
                    
                        selection = dialog.Selection;
                    }       

                    // should we copy one of the files over the original?
                    switch( selection )
                    {
                        case ConflictDialog.Choice.OldRev:
                            this.Copy( file.Path, file.Status.Entry.ConflictOld );
                            break;
                        case ConflictDialog.Choice.NewRev:
                            this.Copy( file.Path, file.Status.Entry.ConflictNew  );
                            break;
                        case ConflictDialog.Choice.Mine:
                            this.Copy( file.Path, file.Status.Entry.ConflictWorking );
                            break;
                        default:
                            break;
                    }

                    file.Resolved();
                }
                catch( StatusException )
                {
                    // swallow
                }
            }

            private void GetRevisions( WorkingCopyFile file, out int oldRev, out int newRev )
            {
                oldRev = int.Parse( NUMBER.Match( file.Status.Entry.ConflictOld ).Groups[1].Value );
                newRev = int.Parse( NUMBER.Match( file.Status.Entry.ConflictNew ).Groups[1].Value );
            }  

            private void Copy( string toPath, string fromFile )
            {
                string dir = Path.GetDirectoryName( toPath );
                string fromPath = Path.Combine( dir, fromFile );
                File.Copy( fromPath, toPath,  true );
            }
       
            private readonly Regex NUMBER = new Regex( @".*\.r(\d+)" );
            private AnkhContext context;

        }
    }
}



