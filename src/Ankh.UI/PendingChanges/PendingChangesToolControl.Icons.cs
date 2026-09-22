using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.UI.PendingChanges
{
    partial class PendingChangesToolControl
    {
        readonly Dictionary<ToolStripItem, Bitmap> _navigationIcons = new Dictionary<ToolStripItem, Bitmap>();

        void RefreshNavigationIcons()
        {
            if (Context == null || IsDisposed || Disposing || pendingChangesTabs == null)
                return;

            IVsImageService2 images = Context.GetService<IVsImageService2>(typeof(SVsImageService));
            if (images == null)
                return; // Keep the designer/fallback resources until VS is ready.

            SetNavigationIcon(images, fileChangesButton, KnownMonikers.PendingChange);
            SetNavigationIcon(images, issuesButton, KnownMonikers.TaskList);
            SetNavigationIcon(images, recentChangesButton, KnownMonikers.ShowFullHistory);
            SetNavigationIcon(images, conflictsButton, KnownMonikers.Conflict);
        }

        void SetNavigationIcon(IVsImageService2 images, ToolStripItem button, ImageMoniker moniker)
        {
            Bitmap bitmap = RenderNavigationIcon(images, moniker, pendingChangesTabs.BackColor, pendingChangesTabs.DeviceDpi);
            if (bitmap == null)
                return;

            Bitmap previous;
            _navigationIcons.TryGetValue(button, out previous);
            // The service already renders at the target DPI; ToolStrip must not
            // resize that result back to its designer-time 32 pixel size.
            button.ImageScaling = ToolStripItemImageScaling.None;
            button.Image = bitmap;
            _navigationIcons[button] = bitmap;
            if (previous != null)
                previous.Dispose();
        }

        internal static Bitmap RenderNavigationIcon(IVsImageService2 images, ImageMoniker moniker, Color background, int dpi)
        {
            var attributes = new ImageAttributes
            {
                StructSize = Marshal.SizeOf(typeof(ImageAttributes)),
                ImageType = (uint)_UIImageType.IT_Bitmap,
                Format = (uint)_UIDataFormat.DF_WinForms,
                LogicalWidth = 24,
                LogicalHeight = 24,
                Dpi = dpi,
                Background = unchecked((uint)background.ToArgb()),
                Flags = unchecked((uint)(_ImageAttributesFlags.IAF_RequiredFlags | _ImageAttributesFlags.IAF_Background))
            };

            try
            {
                IVsUIObject image = images.GetImage(moniker, attributes);
                object data;
                if (image == null || image.get_Data(out data) < 0)
                    return null;
                Bitmap bitmap = data as Bitmap;
                return bitmap != null ? new Bitmap(bitmap) : null;
            }
            catch (COMException) { return null; }
            catch (ArgumentException) { return null; }
        }

        void DisposeNavigationIcons()
        {
            foreach (Bitmap bitmap in _navigationIcons.Values)
                bitmap.Dispose();
            _navigationIcons.Clear();
        }
    }
}
