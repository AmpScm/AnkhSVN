// $Id$
using System;
using NSvn;
using NSvn.Common;
using NSvn.Core;
using Ankh.UI;
using System.Windows.Forms;
using System.Collections;
using EnvDTE;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;

namespace Ankh
{
	/// <summary>
	/// Summary description for SvnContext.
	/// </summary>
    internal class SvnContext : NSvnContext
    {
        public SvnContext( AnkhContext ankhContext )
        {
            this.ankhContext = ankhContext;
            this.AddAuthenticationProvider( new DialogProvider() );

            OutputWindow outputWindow = (OutputWindow)this.ankhContext.DTE.Windows.Item(
                EnvDTE.Constants.vsWindowKindOutput).Object;

            this.outputPane = outputWindow.OutputWindowPanes.Add( "Subversion" );
        }
        /// <summary>
        /// Invokes the LogMessage dialog.
        /// </summary>
        /// <param name="commitItems"></param>
        /// <returns></returns>
        protected override string LogMessageCallback(NSvn.Core.CommitItem[] commitItems)
        {
            string templateText = this.GetTemplate();
            LogMessageTemplate template = new LogMessageTemplate( templateText );

            using( CommitDialog dialog = new CommitDialog( commitItems ) )
            {
                dialog.LogMessageTemplate = template;

                dialog.DiffWanted += new EventHandler( this.DiffWanted );
                if ( dialog.ShowDialog() == DialogResult.OK )
                {
                    return dialog.LogMessage;
                }
                else
                    return null;
            }
        }

        protected override void NotifyCallback(NSvn.Core.Notification notification)
        {
            this.outputPane.Activate();

            this.outputPane.OutputString( string.Format( "File: {0}\tAction: {1}{2}", 
                notification.Path, notification.Action, Environment.NewLine ) );         
       
        }
        
        private string GetTemplate()
        {
             return @"# All lines starting with a # will be ignored
***# %path%";
        }


        private void DiffWanted( object sender, EventArgs args )
        {  
            DiffVisitor visitor = new DiffVisitor();

            this.ankhContext.SolutionExplorer.VisitSelectedItems( visitor, true );

            CommitDialog dialog = (CommitDialog)sender;
            dialog.Diff = visitor.Diff;    
        }

            #region DialogProvider
        private class DialogProvider : IAuthenticationProvider
        {
            

            #region Implementation of IAuthenticationProvider
            public NSvn.Common.ICredential NextCredentials( ICollection parameters )
            {               

                if ( loginDialog.ShowDialog() == DialogResult.OK )
                    return this.lastCredential = new SimpleCredential( loginDialog.Username, 
                        loginDialog.Password );
                else
                    return this.lastCredential = null;
            }
            public NSvn.Common.ICredential FirstCredentials( ICollection parameters )
            {
                if ( this.savedCredential != null )
                    return this.savedCredential;

                if ( loginDialog.ShowDialog() == DialogResult.OK )
                    return this.lastCredential = new SimpleCredential( loginDialog.Username, 
                        loginDialog.Password );
                else
                    return this.lastCredential = null;
            }

            public bool SaveCredentials( ICollection parameters )
            {
                this.savedCredential = this.lastCredential;
                return true;
            }
            public string Kind
            {
                get
                {
                    return SimpleCredential.AuthKind;
                }
            }
            #endregion

            private LoginDialog loginDialog = new LoginDialog();
            private ICredential lastCredential;
            private ICredential savedCredential;
        }
        #endregion

       
        private AnkhContext ankhContext;
        private OutputWindowPane outputPane;
        private static IDictionary map = new Hashtable();
    }
}
