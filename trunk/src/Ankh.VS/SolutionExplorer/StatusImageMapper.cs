using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Ankh.Scc;
using SharpSvn;
using System.IO;
using System.Diagnostics;

namespace Ankh.VS.SolutionExplorer
{
    [GlobalService(typeof(IStatusImageMapper))]
    sealed class StatusImageMapper : AnkhService, IStatusImageMapper
    {
        public StatusImageMapper(IAnkhServiceProvider context)
            : base(context)
        {
        }

        ImageList _statusImageList;
        public ImageList StatusImageList
        {
            get { return _statusImageList ?? (_statusImageList = CreateStatusImageList()); }
        }

        public ImageList CreateStatusImageList()
        {
            using (Stream images = typeof(StatusImageMapper).Assembly.GetManifestResourceStream(typeof(StatusImageMapper).Namespace + ".StatusGlyphs.bmp"))
            {
                if (images == null)
                    return null;

                Bitmap bitmap = (Bitmap)Image.FromStream(images, true);

                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(8, bitmap.Height);
                bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                imageList.Images.AddStrip(bitmap);

                return imageList;
            }
        }

        public AnkhGlyph GetStatusImageForSvnItem(SvnItem item)
        {
            if (item == null)
                return AnkhGlyph.None;
            else if (item.IsConflicted || item.IsObstructed)
                return AnkhGlyph.InConflict;
            else if (item.ReadOnlyMustLock)
                return AnkhGlyph.MustLock;
            else if (!item.IsVersioned)
            {
                if (!item.Exists)
                    return AnkhGlyph.FileMissing;
                else if (item.IsIgnored)
                    return AnkhGlyph.Ignored;
                else if (item.IsVersionable)
                    return item.InSolution ? AnkhGlyph.ShouldBeAdded : AnkhGlyph.Blank;
                else
                    return AnkhGlyph.None;
            }
            
			switch (item.Status.CombinedStatus)
            {
                case SvnStatus.Normal:
                    if (item.IsDocumentDirty)
                        return AnkhGlyph.FileDirty;
                    else if (item.IsLocked)
                        return AnkhGlyph.LockedNormal;
                    else
                        return AnkhGlyph.Normal;
                case SvnStatus.Modified:
                    return item.IsLocked ? AnkhGlyph.LockedModified : AnkhGlyph.Modified;
                case SvnStatus.Replaced:
                    return AnkhGlyph.CopiedOrMoved;
                case SvnStatus.Added:
                    return item.Status.IsCopied ? AnkhGlyph.CopiedOrMoved : AnkhGlyph.Added;

                case SvnStatus.Missing:
                case SvnStatus.Deleted:
                    return AnkhGlyph.Deleted;

                case SvnStatus.Conflicted: // Should have been handled above
                case SvnStatus.Obstructed:
                    return AnkhGlyph.InConflict;

                case SvnStatus.Ignored: // Should have been handled above
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
