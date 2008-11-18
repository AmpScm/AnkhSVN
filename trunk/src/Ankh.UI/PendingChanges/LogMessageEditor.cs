//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
using OLEConstants = Microsoft.VisualStudio.OLE.Interop.Constants;
using System.Security.Permissions;
using Ankh.Scc.UI;
using Ankh.Ids;
using Ankh.UI.PendingChanges.Commits;

namespace Ankh.UI.PendingChanges
{
    /// <summary>
    /// This class is used to implement CodeEditorUserControl
    /// </summary>
    /// <seealso cref="UserControl"/>
    public class LogMessageEditor : VSTextEditor
    {
        
        public LogMessageEditor()
        {
            BackColor = SystemColors.Window;
            base.ForceLanguageService = new Guid(AnkhId.LogMessageLanguageServiceId);
        }

        public LogMessageEditor(IContainer container)
            : base(container)
        {
            base.ForceLanguageService = new Guid(AnkhId.LogMessageLanguageServiceId);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Guid? ForceLanguageService
        {
            get { return base.ForceLanguageService; }
            set { throw new InvalidOperationException(); }
        }

        IPendingChangeSource _pasteSrc;
        /// <summary>
        /// Gets or sets the paste source.
        /// </summary>
        /// <value>The paste source.</value>
        [DefaultValue(null)]
        public IPendingChangeSource PasteSource
        {
            get { return _pasteSrc; }
            set { _pasteSrc = value; }
        }             
    }    
}

