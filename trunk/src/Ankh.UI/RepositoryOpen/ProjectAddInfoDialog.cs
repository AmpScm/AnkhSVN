using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ankh.UI.RepositoryOpen
{
    public enum ProjectAddMode
    {
        External,
        Copy,
        SlnConnection,
        Unconnected,
        Unversioned
    }
    public partial class ProjectAddInfoDialog : VSDialogForm
    {
        public ProjectAddInfoDialog()
        {
            InitializeComponent();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        public bool EnableExternal 
        {
            get { return externalRadio.Enabled; }
            set
            {
                externalRadio.Enabled = value;
                externalBlock.Enabled = value;
                if (externalRadio.Checked && !value)
                {
                    if (EnableCopy)
                        copyRadio.Checked = true;
                    else
                        unconnectedRadio.Checked = true;
                }
                externalDefInfo.Visible =!value;
            }
        }

        public bool EnableCopy
        {
            get { return copyRadio.Enabled; }
            set
            {
                copyRadio.Enabled = value;
                copyBlock.Enabled = value;
                if (copyRadio.Checked && !value)
                {
                    if (EnableExternal)
                        externalRadio.Checked = true;
                    else
                        unconnectedRadio.Checked = true;
                }
                copyDefInfo.Visible = !value;
            }
        }

        public bool EnableSlnConnection
        {
            get { return slnRadio.Enabled; }
            set
            {
                slnRadio.Enabled = value;
                slnBlock.Enabled = value;
                if (slnRadio.Checked)
                    unconnectedRadio.Checked = true;
            }
        }

        public string ExternalLocation
        {
            get { return (string)externalPropLocations.SelectedItem; }
        }

        public bool ExternalLocked
        {
            get { return lockExternal.Checked; }
        }

        public void SetExternalDirs(IEnumerable<string> dirs)
        {
            externalPropLocations.Items.Clear();
            foreach (string path in dirs)
            {
                externalPropLocations.Items.Add(path);
            }
            if (externalPropLocations.Items.Count > 0)
                externalPropLocations.SelectedIndex = 0;
        }

        public ProjectAddMode SelectedMode
        {
            get
            {
                if (externalRadio.Checked)
                    return ProjectAddMode.External;
                if (copyRadio.Checked)
                    return ProjectAddMode.Copy;
                if (slnRadio.Checked)
                    return ProjectAddMode.SlnConnection;
                if (unconnectedRadio.Checked)
                    return ProjectAddMode.Unconnected;
                if (notVersionedRadio.Checked)
                    return ProjectAddMode.Unversioned;

                return ProjectAddMode.Unconnected;
            }
        }
    }
}
