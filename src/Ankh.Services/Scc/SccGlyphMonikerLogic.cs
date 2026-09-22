// Copyright 2026 The AnkhSVN Project
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    /// <summary>
    /// Maps Ankh working-copy states to Visual Studio Image Catalog monikers.
    /// The array order intentionally matches <see cref="AnkhGlyph"/>.
    /// </summary>
    internal static class SccGlyphMonikerLogic
    {
        private static readonly ImageMoniker[] _monikers =
        {
            default(ImageMoniker),                  // None
            KnownMonikers.OverlayLock,              // MustLock
            KnownMonikers.PendingChangeNode,        // Modified
            KnownMonikers.PendingDeleteNode,        // Deleted
            KnownMonikers.PendingChangeNode,        // FileDirty
            default(ImageMoniker),                  // Blank
            KnownMonikers.CheckedInNode,            // Normal
            KnownMonikers.DocumentWarning,          // FileMissing
            KnownMonikers.PendingAddNode,           // CopiedOrMoved
            KnownMonikers.Lock,                     // LockedNormal
            KnownMonikers.CheckedOutForEditNode,    // LockedModified
            KnownMonikers.StatusExcluded,           // Ignored
            KnownMonikers.PendingAddNode,           // Added
            KnownMonikers.CheckAdd,                 // ShouldBeAdded
            KnownMonikers.Conflict,                 // InConflict
            KnownMonikers.PendingChangeNode         // ChildChanged
        };

        public static int Count
        {
            get { return _monikers.Length; }
        }

        public static ImageMoniker GetMoniker(AnkhGlyph glyph)
        {
            int index = (int)glyph;
            if (index < 0 || index >= _monikers.Length)
                throw new ArgumentOutOfRangeException("glyph");

            return _monikers[index];
        }

        public static IVsImageMonikerImageList CreateImageList()
        {
            return new SccGlyphMonikerImageList(_monikers);
        }

        private sealed class SccGlyphMonikerImageList : IVsImageMonikerImageList
        {
            readonly ImageMoniker[] _images;

            public SccGlyphMonikerImageList(ImageMoniker[] images)
            {
                _images = (ImageMoniker[])images.Clone();
            }

            public int ImageCount
            {
                get { return _images.Length; }
            }

            public void GetImageMonikers(
                int firstImageIndex,
                int imageMonikerCount,
                ImageMoniker[] imageMonikers)
            {
                if (imageMonikers == null)
                    throw new ArgumentNullException("imageMonikers");
                if (firstImageIndex < 0
                    || imageMonikerCount < 0
                    || firstImageIndex + imageMonikerCount > _images.Length
                    || imageMonikerCount > imageMonikers.Length)
                {
                    throw new ArgumentOutOfRangeException("imageMonikerCount");
                }

                Array.Copy(
                    _images,
                    firstImageIndex,
                    imageMonikers,
                    0,
                    imageMonikerCount);
            }
        }
    }
}
