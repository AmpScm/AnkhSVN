using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace IssueZilla
{
    public interface IBackgroundWorkerService
    {
        DialogResult DoWork( string caption, DoWorkEventHandler handler );
        DialogResult DoWork( string caption, IBackgroundOperation operation );
    }

    public class BackgroundWorkerService : IBackgroundWorkerService
    {
        public BackgroundWorkerService( IWin32Window window )
        {
            this.window = window;
        }

        public DialogResult DoWork( string caption, DoWorkEventHandler handler )
        {
            using ( BackgroundWorkerForm form = new BackgroundWorkerForm( handler ) )
            {
                return ShowDialog( caption, form );
            }
        }       

        public DialogResult DoWork( string caption, IBackgroundOperation operation )
        {
            using ( BackgroundWorkerForm form = new BackgroundWorkerForm( operation ) )
            {
                return ShowDialog( caption, form );
            }
        }

        private DialogResult ShowDialog( string caption, BackgroundWorkerForm form )
        {
            form.Caption = caption;
            form.ShowDialog( this.window );
            return form.DialogResult;
        }

        private IWin32Window window;

    }
}
