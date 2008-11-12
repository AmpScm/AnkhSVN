using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Ankh.VS;

namespace Ankh.UI
{
    public class VSEditorControl : UserControl
    {
        IAnkhServiceProvider _context;

        public VSEditorControl()
        {
            /*ShowInTaskbar = false;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            FormBorderStyle = FormBorderStyle.None;*/
        }        

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        /// <value>The context.</value>
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            protected set { _context = value; }
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Localizable(true)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        /*/// <summary>
        /// Gets or sets a value indicating whether the form is displayed in the Windows taskbar.
        /// </summary>
        /// <value></value>
        /// <returns>true to display the form in the Windows taskbar at run time; otherwise, false. The default is true.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        public new bool ShowInTaskbar
        {
            get { return base.ShowInTaskbar; }
            set { base.ShowInTaskbar = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Maximize button is displayed in the caption bar of the form.
        /// </summary>
        /// <value></value>
        /// <returns>true to display a Maximize button for the form; otherwise, false. The default is true.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        public new bool MaximizeBox
        {
            get { return base.MaximizeBox; }
            set { base.MaximizeBox = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Minimize button is displayed in the caption bar of the form.
        /// </summary>
        /// <value></value>
        /// <returns>true to display a Minimize button for the form; otherwise, false. The default is true.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        public new bool MinimizeBox
        {
            get { return base.MinimizeBox; }
            set { base.MinimizeBox = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a control box is displayed in the caption bar of the form.
        /// </summary>
        /// <value></value>
        /// <returns>true if the form displays a control box in the upper left corner of the form; otherwise, false. The default is true.
        /// </returns>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// 	<IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(false)]
        public new bool ControlBox
        {
            get { return base.ControlBox; }
            set { base.ControlBox = value; }
        }
         
        [DefaultValue(FormBorderStyle.None)]
        public new FormBorderStyle FormBorderStyle
        {
            get { return base.FormBorderStyle; }
            set { base.FormBorderStyle = value; }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }
         */

        IAnkhDynamicEditorFactory _dialogOwner;
        [CLSCompliant(false)]
        protected IAnkhDynamicEditorFactory DynamicFactory
        {
            get { return _dialogOwner ?? (_dialogOwner = Context.GetService<IAnkhDynamicEditorFactory>()); }
        }

        public void Create(IAnkhServiceProvider context, string fullPath)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;

            DynamicFactory.CreateEditor(fullPath, this);
            OnFrameCreated(EventArgs.Empty);
        }        

        protected virtual void OnFrameCreated(EventArgs e)
        {
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // VSDocumentForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Name = "VSDocumentForm";
            this.ResumeLayout(false);
        }

        [CLSCompliant(false)]
        public void AddCommandTarget(Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget commandTarget)
        {
            if (commandTarget == null)
                throw new ArgumentNullException("commandTarget");

            //throw new NotImplementedException();
        }
    }
}
