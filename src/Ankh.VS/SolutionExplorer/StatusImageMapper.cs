// Copyright 2006-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using Ankh.Scc;
using SharpSvn;
using System.IO;
using System.Globalization;
using System.Drawing.Drawing2D;

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
            get { return _statusImageList ?? (_statusImageList = CreateStatusImageList(true)); }
        }

        ImageList IStatusImageMapper.CreateStatusImageList()
        {
            return CreateStatusImageList(false);
        }

        public ImageList CreateStatusImageList(bool width8)
        {
            int width = width8 ? 8 : 7;
            using (Stream images = typeof(StatusImageMapper).Assembly.GetManifestResourceStream(typeof(StatusImageMapper).Namespace + string.Format(CultureInfo.InvariantCulture, ".StatusGlyphs{0}.bmp", width)))
            {
                if (images == null)
                    return null;

                using (Bitmap bitmap = (Bitmap)Image.FromStream(images, true))
                {
                    bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                    int dpi = FileIconMapperDpiLogic.GetCurrentDpi();
                    int targetWidth = FileIconMapperDpiLogic.GetPixelSize(width, dpi);
                    int targetHeight = FileIconMapperDpiLogic.GetPixelSize(bitmap.Height, dpi);
                    int count = bitmap.Width / width;

                    ImageList imageList = new ImageList
                    {
                        ColorDepth = ColorDepth.Depth32Bit,
                        ImageSize = new Size(targetWidth, targetHeight)
                    };

                    for (int i = 0; i < count; i++)
                    {
                        Bitmap glyph = new Bitmap(targetWidth, targetHeight);
                        using (Graphics graphics = Graphics.FromImage(glyph))
                        {
                            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                            graphics.PixelOffsetMode = PixelOffsetMode.Half;
                            graphics.DrawImage(
                                bitmap,
                                new Rectangle(0, 0, targetWidth, targetHeight),
                                new Rectangle(i * width, 0, width, bitmap.Height),
                                GraphicsUnit.Pixel);
                        }

                        imageList.Images.Add(glyph);
                    }

                    return imageList;
                }
            }
        }

        public AnkhGlyph GetStatusImageForSvnItem(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            return StatusImageMapperLogic.GetGlyph(
                new StatusImageInfo(
                    item.IsConflicted,
                    item.IsObstructed,
                    item.IsTreeConflicted,
                    item.IsReadOnlyMustLock,
                    item.IsVersioned,
                    item.Exists,
                    item.IsIgnored,
                    item.IsVersionable,
                    item.InSolution,
                    item.IsSccExcluded,
                    item.Status.CombinedStatus,
                    item.IsDocumentDirty,
                    item.IsLocked,
                    item.Status.IsCopied,
                    item.IsCasingConflicted));
        }
    }
}
