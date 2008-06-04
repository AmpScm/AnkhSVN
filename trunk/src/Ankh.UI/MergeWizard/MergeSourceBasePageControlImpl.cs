using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Resources;

using SharpSvn;
using WizardFramework;
using System.Threading;

namespace Ankh.UI.MergeWizard
{
    
    public partial class MergeSourceBasePageControlImpl: UserControl
        
    {
        private readonly WizardMessage INVALID_FROM_URL = new WizardMessage(Resources.InvalidFromUrl,
            WizardMessage.ERROR);
        
        

        /// <summary>
        /// Constructor.
        /// </summary>
        public MergeSourceBasePageControlImpl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enables/Disables the Select button.
        /// </summary>
        public void EnableSelectButton(bool enabled)
        {
            selectButton.Enabled = enabled;
            selectButton.Visible = enabled;
        }

        #region Base Functionality


        internal string MergeSource
        {
            get { return mergeFromComboBox.Text; }
        }

        



        

       
        #endregion


    }
}
