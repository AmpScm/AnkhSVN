//using System;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Windows.Forms;
//
//namespace Ankh.Commands
//{
//	/// <summary>
//	/// Lets the user cat a file from a repos and have Windows open it.
//	/// </summary>
//    [VSNetCommand("ViewInWindows", Tooltip="Have Windows launch the associated application.", Text = "In Windows" ),
//    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
//    internal class ViewInWindowsCommand : ViewRepositoryFileCommand
//    {
//        public override void Execute(AnkhContext context, string parameters)
//        {
//            try
//            {
//                context.StartOperation( "Opening" );
//
//                CatVisitor v = new CatVisitor( context );
//                context.RepositoryController.VisitSelectedNodes( v );
//
//                // shell execute each file.
//                foreach( string filename in v.FileNames )
//                {
//                    Process process = new Process();
//                    process.StartInfo.FileName = filename;
//                    process.StartInfo.UseShellExecute = true;
//
//                    try
//                    {
//                        process.Start();
//                    }
//                    catch( Win32Exception ex )
//                    {
//                        // no application is associated with the file type
//                        if ( ex.NativeErrorCode == NOASSOCIATEDAPP )
//                            MessageBox.Show( "Windows could not find an application associated with the file type", 
//                                "No associated application", MessageBoxButtons.OK );
//                        else
//                            throw;
//                    }
//                }
//            }
//            finally
//            {
//                context.EndOperation();
//            }
//        }        
//
//        private const int NOASSOCIATEDAPP = 1155;
//    }
//}
