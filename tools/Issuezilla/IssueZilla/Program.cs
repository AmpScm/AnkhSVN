using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;

namespace IssueZilla
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new MainForm() );
        }
    }

    public class WithEvent
    {
        public event EventHandler StuffHappened;

        public WithEvent()
        {
            this.synchronizationContext = SynchronizationContext.Current;

            
        }

        public void Run()
        {
            //this code is not really important, just sets up so the method is rnu from some other thread
            ThreadPool.QueueUserWorkItem( delegate { this.RunThisFromSomeOtherThread(); } );

        }

        private void RunThisFromSomeOtherThread()
        {
            this.RaiseEvent( StuffHappened );
        }

        private void RaiseEvent( EventHandler del )
        {
            if ( del != null )
            {
                synchronizationContext.Post( delegate
                {
                    del( this, EventArgs.Empty );
                } , null );
            }
        }

        private SynchronizationContext synchronizationContext;
    }
}