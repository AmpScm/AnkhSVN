using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.MergeWizard
{
    public partial class MergeBestPracticesPageControl : BasePageControl
    {
        public MergeBestPracticesPageControl()
        {
            InitializeComponent();
        }

        public void UpdateBestPracticeStatus(MergeBestPracticesPage.BestPractices bestPractice,
            bool passed)
        {
            PictureBox pBox;

            switch (bestPractice)
            {
                case MergeBestPracticesPage.BestPractices.NO_LOCAL_MODIFICATIONS:
                    pBox = noUncommitedModificationsPictureBox;
                    break;
                case MergeBestPracticesPage.BestPractices.NO_MIXED_REVISIONS:
                    pBox = singleRevisionPictureBox;
                    break;
                case MergeBestPracticesPage.BestPractices.NO_SWITCHED_CHILDREN:
                    pBox = noSwitchedChildrenPictureBox;
                    break;
                case MergeBestPracticesPage.BestPractices.COMPLETE_WORKING_COPY:
                    pBox = completeWorkingCopyPictureBox;
                    break;
                default:
                    pBox = null;
                    break;
            }

            if (pBox != null)
            {
                if (passed)
                    pBox.Image = Resources.SuccessImage;
                else
                    pBox.Image = Resources.ErrorImage;
            }
        }
    }
}
