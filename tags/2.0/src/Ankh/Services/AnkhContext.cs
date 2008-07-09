// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.UI;
using Ankh.VS;

using SharpSvn;
using IServiceProvider = System.IServiceProvider;

namespace Ankh
{
    /// <summary>
    /// General context object for the Ankh addin. Contains pointers to objects
    /// required by commands.
    /// </summary>
    public class OldAnkhContext : AnkhService, IContext
    {
        OutputPaneWriter _outputPane;

        public OldAnkhContext(IAnkhPackage package)
            : this(package, null)
        {
        }

        public OldAnkhContext(IAnkhPackage package, IUIShell uiShell)
            : base(package)
        {
            if (uiShell != null)
                _uiShell = uiShell;

            this._outputPane = new OutputPaneWriter(this, "AnkhSVN");
        }

        EnvDTE._DTE _dte;
        /// <summary>
        /// The top level automation object.
        /// </summary>
        EnvDTE._DTE DTE
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _dte ?? (_dte = GetService<EnvDTE._DTE>(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE))); }
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
            get { return this._outputPane; }
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

        bool _operationRunning;
        /// <summary>
        /// Should be called before starting any lengthy operation
        /// </summary>
        public IDisposable StartOperation(string description)
        {
            //TODO: maybe refactor this?
            _operationRunning = true;
            try
            {
                this.DTE.StatusBar.Text = description + "...";
                this.DTE.StatusBar.Animate(true, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);
            }
            catch (Exception)
            {
                // Swallow, not critical
            }

            return new OperationCompleter(this, this.OutputPane.StartActionText(description));
        }

        class OperationCompleter : IDisposable
        {
            OldAnkhContext _context;
            IDisposable _disp2;

            public OperationCompleter(OldAnkhContext context, IDisposable disp2)
            {
                _context = context;
                _disp2 = disp2;
            }

            public void Dispose()
            {
                _context.EndOperation();
                _context = null;
                _disp2.Dispose();
                _disp2 = null;
            }
        }

        /// <summary>
        ///  called at the end of any lengthy operation
        /// </summary>
        public void EndOperation()
        {
            if (_operationRunning)
            {
                try
                {
                    this.DTE.StatusBar.Text = "Ready";
                    this.DTE.StatusBar.Animate(false, EnvDTE.vsStatusAnimation.vsStatusAnimationSync);
                }
                catch (Exception)
                {
                    // swallow, not critical
                }
                _operationRunning = false;
            }
        }
    }
}
