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
    internal class ResolveConflict : CommandBase
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
            ResolveVisitor v = new ResolveVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, false );
            context.SolutionExplorer.RefreshSelection();
        }

        private class ResolveVisitor : LocalResourceVisitorBase
        {

        
            public override void VisitWorkingCopyFile(WorkingCopyFile file)
            {
                int oldRev, newRev;
                this.GetRevisions( file.Path, out oldRev, out newRev );

                ConflictDialog.Choice selection;

                using( ConflictDialog dialog = new ConflictDialog() )
                {
                    dialog.OldRev = oldRev;
                    dialog.NewRev = newRev;
                    dialog.Filename = file.Path;

                    if ( dialog.ShowDialog() != DialogResult.OK )
                        return;
                    
                    selection = dialog.Selection;
                }       

                // should we copy one of the files over the original?
                switch( selection )
                {
                    case ConflictDialog.Choice.OldRev:
                        this.Copy( file.Path, ".r" + oldRev.ToString () );
                        break;
                    case ConflictDialog.Choice.NewRev:
                        this.Copy( file.Path, ".r" + newRev.ToString() );
                        break;
                    case ConflictDialog.Choice.Mine:
                        this.Copy( file.Path, ".mine" );
                        break;
                    default:
                        break;
                }

                file.Resolve();
            }

            private void GetRevisions( string path, out int oldRev, out int newRev )
            {
                string dir = Path.GetDirectoryName( path );
                string file = Path.GetFileName( path );
                string[] files = Directory.GetFiles( dir, file + "*" );
                
                int[] revs = new int[2];
                int counter = 0;
                foreach( string potential in files )
                {
                    if ( NUMBER.IsMatch( potential ) )
                        revs[counter++] = int.Parse( NUMBER.Match( potential ).Groups[1].Value );

                    Debug.Assert( counter <=2, "Should never have more than 2 resolvees with revisions" );
                }

                Debug.Assert( counter == 2, "Should have exactly 2 resolvees with revisions" );

                if ( revs[0] <= revs[1] )
                {
                    oldRev = revs[0];
                    newRev = revs[1];
                }
                else
                {
                    oldRev = revs[1];
                    newRev = revs[0];
                }
            }  

            private void Copy( string path, string extension )
            {
                string dir = Path.GetDirectoryName( path );
                string file = Path.GetFileName( path );

                string[] files = Directory.GetFiles( dir, string.Format( "{0}*{1}", file, extension ) );
                Debug.Assert( files.Length == 1 );

                File.Copy( Path.Combine( dir, files[0] ), path, true );
            }

       
            private readonly Regex NUMBER = new Regex( @".*\.r(\d+)" );

        }
    }
}



