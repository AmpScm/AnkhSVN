// $Id: ViewRepositoryFileCommand.cs 738 2003-06-22 23:25:33Z Arild $
using EnvDTE;
using NSvn;
using System.IO;
using System.Collections;

namespace Ankh.Commands
{
    /// <summary>
    /// A command that lets you view a repository file.
    /// </summary>
    [VSNetCommand("Checkout", Tooltip="Checkout this file or foder", Text = "Checkout" ),
    VSNetControl( "ReposExplorer", Position = 1 ) ]
    internal abstract class CheckoutCommand : CommandBase
    {
        #region ICommand Members
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            IsRepositoryFileVisitor v = new IsRepositoryFileVisitor();
            context.RepositoryController.VisitSelectedNodes( v );
            return v.IsFile ? vsCommandStatus.vsCommandStatusSupported :
                vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
        }
        #endregion

        #region CheckoutVisitor
        protected class CheckoutVisitor : RepositoryResourceVisitorBase
        {          
            public virtual IEnumerable Directories
            {
                get{ return this.directories; }
            }

            public override void VisitDirectory(RepositoryDirectory directory)
            {
                this.directories.Add( directory );
            }

            private ArrayList directories = new ArrayList();
        }
        #endregion
    }
}



