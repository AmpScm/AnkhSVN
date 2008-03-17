using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Ankh.Scc;

namespace Ankh
{
    public class StatusImages
    {
        public static ImageList StatusImageList
        {
            get
            {
                if (statusImageList == null)
                {
                    Bitmap statusImages = (Bitmap)Image.FromStream(
                        typeof(StatusImages).Assembly.GetManifestResourceStream(STATUS_IMAGES));
                    statusImages.MakeTransparent(statusImages.GetPixel(0, 0));

                    statusImageList = new ImageList();
                    statusImageList.ImageSize = new Size(8, statusImages.Height);
                    statusImageList.Images.AddStrip(statusImages);
                }
                return statusImageList;
            }
        }

        public static int GetStatusImageForNodeStatus(NodeStatus status)
        {
            if (statusMap.Contains(status.Kind))
            {
                return (int)statusMap[status.Kind];
            }
            else
            {
                return 0;
            }
        }


        static StatusImages()
        {
            statusMap[NodeStatusKind.Normal] = 1;
            statusMap[NodeStatusKind.Added] = 2;
            statusMap[NodeStatusKind.Deleted] = 3;
            statusMap[NodeStatusKind.Replaced] = 4;
            statusMap[NodeStatusKind.IndividualStatusesConflicting] = 7;
            statusMap[NodeStatusKind.Conflicted] = 6;
            statusMap[NodeStatusKind.Unversioned] = 8;
            statusMap[NodeStatusKind.Modified] = 9;
        }

        internal const int STATUS_IMAGE_COUNT = 10;
        private const string STATUS_IMAGES = "Ankh.status_icons.bmp";
        private static ImageList statusImageList = null;
        private static readonly IDictionary statusMap = new Hashtable();

        public class TempStatusImageMapper : IStatusImageMapper
        {
            #region IStatusImageMapper Members

            public int GetStatusImageForNodeStatus(NodeStatus status)
            {
                return StatusImages.GetStatusImageForNodeStatus(status);
            }

            public ImageList StatusImageList
            {
                get { return StatusImages.StatusImageList; }
            }

            #endregion
        }
    }
}
