using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Diagnostics;
using EnvDTE;
using System.IO;

namespace Ankh
{
    [Service(typeof(IOperationManager))]
    public sealed class OperationManager : IOperationManager
    {
        public OperationManager(IServiceProvider serviceProvider)
        {
            operations = new Stack();
            this.serviceProvider = serviceProvider;
        }

        public IDisposable RunOperation(string caption)
        {
            Operation o = new Operation(this, caption);
            this.operations.Push(o);
            this.SetCaptions(o);
            return o;
        }

        public bool OperationRunning
        {
            get { return this.operations.Count > 0;}
        }

        private void SetCaptions(Operation operation)
        {
            string description = operation == null ? "Ready" : operation.caption;
            string statusbar = operation == null ? "Ready" : operation.caption + "...";
            try
            {
                this.DTE.StatusBar.Text = statusbar;
                this.DTE.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSync);
            }
            catch (Exception)
            {
                // Swallow, not critical
            }

            this.OutputPane.StartActionText(description);
        }

        private void OperationFinished(Operation operation)
        {
            Debug.Assert(this.operations.Count > 0, "Unneeded OperationFinished call");
            Debug.Assert((Operation)this.operations.Peek() == operation, "Use using pattern on OperationManager.RunOperation");
            
            this.operations.Pop();
            Operation o = this.operations.Count > 0? (Operation)this.operations.Peek() : null;

            this.SetCaptions(o);
        }

        private _DTE DTE
        {
            get { return (_DTE)this.serviceProvider.GetService(typeof(_DTE)); }
        }

        private OutputPaneTextWriter OutputPane
        {
            get { return ((IOutputPaneProvider)this.serviceProvider.GetService(typeof(IOutputPaneProvider))).OutputPaneWriter; }
        }

        class Operation:IDisposable
        {
            public Operation(OperationManager manager, string caption)
            {
                this.manager = manager;
                this.caption = caption;
            }

            internal readonly string caption;
            private readonly OperationManager manager;


            #region IDisposable Members

            void IDisposable.Dispose()
            {
                manager.OperationFinished(this);
            }

            #endregion
        }
        
        
        private readonly Stack operations;
        private readonly IServiceProvider serviceProvider;
    }
}
