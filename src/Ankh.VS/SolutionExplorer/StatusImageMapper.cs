// $Id$
//
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
using System.Diagnostics;
using System.Globalization;
using SharpGit;

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

                Bitmap bitmap = (Bitmap)Image.FromStream(images, true);

                ImageList imageList = new ImageList();
                imageList.ImageSize = new Size(width, bitmap.Height);
                bitmap.MakeTransparent(bitmap.GetPixel(0, 0));

                imageList.Images.AddStrip(bitmap);

                return imageList;
            }
        }

        public AnkhGlyph GetStatusImageForSvnItem(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.IsConflicted || item.IsObstructed || item.IsTreeConflicted)
                return AnkhGlyph.InConflict;
            else if (item.IsReadOnlyMustLock)
                return AnkhGlyph.MustLock;
            else if (!item.IsVersioned)
            {
                if (!item.Exists)
                    return AnkhGlyph.FileMissing;
                else if (item.IsIgnored)
                    return AnkhGlyph.Ignored;
                else if (item.IsVersionable)
                {
                    if (item.InSolution)
                        return item.IsSccExcluded ? AnkhGlyph.Ignored : AnkhGlyph.ShouldBeAdded;
                    else
                        return AnkhGlyph.None;
                }
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
                    if (item.IsCasingConflicted)
                        return AnkhGlyph.InConflict;
                    else
                        return AnkhGlyph.Deleted;
                case SvnStatus.Deleted:
                    if (item.Exists && item.InSolution)
                        return item.IsSccExcluded ? AnkhGlyph.Ignored : AnkhGlyph.ShouldBeAdded;
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


        public AnkhGlyph GetStatusImageForGitItem(GitItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.IsConflicted)
                return AnkhGlyph.InConflict;
            else if (!item.IsVersioned)
            {
                if (!item.Exists)
                    return AnkhGlyph.FileMissing;
                else if (item.IsIgnored)
                    return AnkhGlyph.Ignored;
                else if (item.IsVersionable)
                {
                    if (item.InSolution)
                        return item.IsSccExcluded ? AnkhGlyph.Ignored : AnkhGlyph.ShouldBeAdded;
                    else
                        return AnkhGlyph.None;
                }
                else
                    return AnkhGlyph.None;
            }

            GitStatus status = item.Status.WorkingStatus;

            if (status == GitStatus.Normal || status == GitStatus.None)
                status = item.Status.IndexStatus;

            switch (status)
            {
                case GitStatus.Normal:
                    if (item.IsDocumentDirty)
                        return AnkhGlyph.FileDirty;
                    else
                        return AnkhGlyph.Normal;
                    return AnkhGlyph.Modified;
                /*case GitStatus.Replaced:
                    return AnkhGlyph.CopiedOrMoved;*/
                case GitStatus.New:
                    return false ? AnkhGlyph.CopiedOrMoved : AnkhGlyph.Added;

                case GitStatus.Renamed:
                    return AnkhGlyph.CopiedOrMoved;

                case GitStatus.Deleted:
                    if (item.Exists && item.InSolution)
                        return item.IsSccExcluded ? AnkhGlyph.Ignored : AnkhGlyph.ShouldBeAdded;
                    return AnkhGlyph.Deleted;

                /*case GitStatus.Conflicted: // Should have been handled above
                case GitStatus.Obstructed:
                    return AnkhGlyph.InConflict;*/

                case GitStatus.Unreadable:
                    return AnkhGlyph.InConflict;
                /*case GitStatus.External:
                case GitStatus.Incomplete:
                    return AnkhGlyph.InConflict;

                case GitStatus.Zero:*/
                default:
                    return AnkhGlyph.None;
            }
            return AnkhGlyph.ShouldBeAdded;
        }
    }
}
