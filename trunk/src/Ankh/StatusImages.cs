using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Ankh.Scc;
using SharpSvn;

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
            public ImageList StatusImageList
            {
                get { return StatusImages.StatusImageList; }
            }

            public AnkhGlyph GetStatusImageForSvnItem(SvnItem item)
            {
                if (item == null)
                    return AnkhGlyph.None;
                else if (item.ReadOnlyMustLock)
                    return AnkhGlyph.MustLock;
                else if (item.InConflict)
                    return AnkhGlyph.InConflict;
                else if (item.IsIgnored)
                    return AnkhGlyph.Ignored;
                else if (!item.IsVersioned)
                {
                    if (!item.Exists)
                        return AnkhGlyph.FileMissing;
                    else if (item.IsVersionable)
                        return AnkhGlyph.Blank; // Scc provider will apply ShouldBeAdded if in a project
                    else
                        return AnkhGlyph.None;
                }
                SvnStatus status = item.Status.LocalContentStatus;

                if (status == SvnStatus.Normal)
                    status = item.Status.LocalContentStatus;

                switch (status)
                {
                    case SvnStatus.Normal:
                        return item.IsLocked ? AnkhGlyph.LockedNormal : AnkhGlyph.Normal;
                    case SvnStatus.Modified:
                        return item.IsLocked ? AnkhGlyph.LockedModified : AnkhGlyph.Modified;

                    case SvnStatus.Added:
                    case SvnStatus.Replaced:
                        return item.Status.LocalCopied ? AnkhGlyph.CopiedOrMoved : AnkhGlyph.Added;

                    case SvnStatus.Missing:
                    case SvnStatus.Deleted:
                        return AnkhGlyph.Deleted;

                    case SvnStatus.Conflicted: // Should have been handled above
                    case SvnStatus.Obstructed:
                        return AnkhGlyph.InConflict;

                    case SvnStatus.Ignored: // Should have been handled abov
                        return AnkhGlyph.Ignored;

                    case SvnStatus.External:
                    case SvnStatus.Incomplete:
                        return AnkhGlyph.InConflict;
       
                    case SvnStatus.Zero:
                    default:
                        return AnkhGlyph.None;
                }
            }       
        }
    }
}
