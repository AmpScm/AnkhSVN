using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Ankh.Scc;
using SharpSvn;
using System.IO;
using System.Diagnostics;

namespace Ankh
{
    sealed class StatusImageMapper : IStatusImageMapper
    {
        readonly IAnkhServiceProvider _context;

        public StatusImageMapper(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
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
            
			switch (item.Status.CombinedStatus)
            {
                case SvnStatus.Normal:
                    return item.IsLocked ? AnkhGlyph.LockedNormal : AnkhGlyph.Normal;
                case SvnStatus.Modified:
                    return item.IsLocked ? AnkhGlyph.LockedModified : AnkhGlyph.Modified;

                case SvnStatus.Added:
                case SvnStatus.Replaced:
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
