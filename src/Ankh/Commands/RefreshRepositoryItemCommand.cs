//// $Id$
//using System;
//using Ankh.RepositoryExplorer;
//
//namespace Ankh.Commands
//{
//    /// <summary>
//    /// Summary description for RefreshRepositoryItemCommand.
//    /// </summary>
//    [VSNetCommand("ViewInVsNet", Tooltip="View this file in VS.NET", Text = "In VS.NET" ),
//    VSNetControl( "ReposExplorer", Position = 1 ) ]
//    internal class RefreshRepositoryItemCommand : CommandBase
//    {
//        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
//        {
//            // we only want directories
//            if ( context.RepositoryExplorer.SelectedNode != null &&
//                context.RepositoryExplorer.SelectedNode.IsDirectory )
//            {
//                return Enabled;
//            }
//            else
//                return Disabled;
//        }
//
//        public override void Execute(AnkhContext context, string parameters)
//        {
//            //context.Con
//        }
//
//
//        
//    }
//}
