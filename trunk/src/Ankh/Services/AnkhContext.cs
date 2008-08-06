// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.UI;
using Ankh.VS;

using SharpSvn;
using IServiceProvider = System.IServiceProvider;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;

namespace Ankh
{
    /// <summary>
    /// General context object for the Ankh addin. Contains pointers to objects
    /// required by commands.
    /// </summary>
    [GlobalService(typeof(IContext), AllowPreRegistered=true)]
    class OldAnkhContext : AnkhService, IContext
    {
        OutputPaneWriter _outputPane;

        public OldAnkhContext(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IUIShell _uiShell;
        /// <summary>
        /// The UI shell service
        /// </summary>
        public IUIShell UIShell
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _uiShell ?? (_uiShell = GetService<IUIShell>()); }
        }

        /// <summary>
        /// The output pane.
        /// </summary>
        public OutputPaneWriter OutputPane
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _outputPane ?? (_outputPane = new OutputPaneWriter(this, "AnkhSVN")); }
        }

        ISvnClientPool _clientPool;
        /// <summary>
        /// Gets the client pool service
        /// </summary>
        public ISvnClientPool ClientPool
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return this._clientPool ?? (this._clientPool = GetService<ISvnClientPool>()); }
        }

        /// <summary>
        /// Was called before starting any lengthy operation
        /// </summary>
        public IDisposable StartOperation(string description)
        {
            //TODO: maybe refactor this?
            string oldText = null;
            bool hadText = false;
            
            IVsStatusbar sb = GetService<IVsStatusbar>(typeof(SVsStatusbar));

            if (sb == null)
                return null;

            if (ErrorHandler.Succeeded(sb.GetText(out oldText)))
            {
                hadText = true;
                sb.SetText(description);

                object icon = (short)Constants.SBAI_Synch;
                sb.Animation(1, ref icon);
            }
            return new OperationCompleter(OutputPane.StartActionText(description), sb, hadText, oldText);
        }

        sealed class OperationCompleter : IDisposable
        {
            readonly IVsStatusbar _sb;
            readonly IDisposable _disp2;
            readonly bool _hadText;
            readonly string _oldText;
            bool _disposed;

            public OperationCompleter(IDisposable disp2, IVsStatusbar sb, bool hadText, string oldText)
            {
                if (sb == null)
                    throw new ArgumentNullException("sb");

                _sb = sb;
                _disp2 = disp2;
                _hadText = hadText;
                _oldText = oldText;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;
                _disposed = true;

                if (_hadText)
                    _sb.SetText(_oldText);

                object icon = (short)Constants.SBAI_Synch;
                _sb.Animation(0, ref icon);

                if (_disp2 != null)
                    _disp2.Dispose();
            }
        }  
    }
}
