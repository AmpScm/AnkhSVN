// Copyright 2008-2009 The AnkhSVN Project
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
using System.Collections.Generic;
using System.Windows.Forms;
using Ankh.Scc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Ankh.Services;

namespace Ankh.VS.SolutionExplorer
{
    [GlobalService(typeof(IFileIconMapper))]
    sealed class FileIconMapper : AnkhService, IFileIconMapper
    {
        const int LogicalIconSize = 16;

        readonly ImageList _imageList;
        readonly Dictionary<ProjectIconReference, int> _iconMap;
        readonly Dictionary<string, int> _monikerMap;
        readonly SortedList<WindowsSpecialFolder, int> _folderMap;
        readonly Dictionary<string, string> _fileTypeMap;
        readonly int _imageDpi;
        IVsImageService2 _imageService;
        readonly Dictionary<string, ImageMoniker> _monikers = new Dictionary<string, ImageMoniker>();
        AnkhServiceEvents _events;

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _events = GetService<AnkhServiceEvents>();
            _events.ThemeChanged += OnThemeChanged;
        }

        void OnThemeChanged(object sender, EventArgs e)
        {
            _imageService = null;
            // Keep indices stable: existing rows and tree nodes retain them.
            foreach (var entry in _monikers)
            {
                try
                {
                    using (Bitmap bitmap = RenderMoniker(entry.Value))
                    {
                        if (bitmap != null)
                            _imageList.Images[_monikerMap[entry.Key]] = bitmap;
                    }
                }
                catch (COMException) { }
                catch (ArgumentException) { }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _events != null)
                _events.ThemeChanged -= OnThemeChanged;
            base.Dispose(disposing);
        }

        public FileIconMapper(IAnkhServiceProvider context)
            : base(context)
        {
            _imageDpi = FileIconMapperDpiLogic.GetCurrentDpi();
            int imageSize = FileIconMapperDpiLogic.GetPixelSize(LogicalIconSize, _imageDpi);

            _imageList = new ImageList();
            _imageList.ImageSize = new Size(imageSize, imageSize);
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _iconMap = new Dictionary<ProjectIconReference, int>();
            _monikerMap = new Dictionary<string, int>(StringComparer.Ordinal);
            _folderMap = new SortedList<WindowsSpecialFolder, int>();
            _fileTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public int GetIcon(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            EnsureSpecialImages();

            // Prefer Visual Studio's image catalog. It supplies the correct
            // themed image for the current file type and can render it at the
            // DPI requested by this ImageList.
            int icon = GetThemeIcon(path);

            if (icon == -1)
                icon = GetProjectIcon(path);

            if (icon == -1)
                icon = GetOsIcon(path);

            return icon;

        }

        public string GetFileType(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            string extension = Path.GetExtension(item.FullPath).TrimStart('.');

            if (item.IsDirectory && !item.FullPath.EndsWith("\\"))
            {
                return SolutionExplorerStrings.ExplorerDirectoryName;
            }

            string rslt;
            lock (_fileTypeMap)
            {
                if (_fileTypeMap.TryGetValue(extension, out rslt))
                    return rslt;
            }

            rslt = GetTypeName(item.FullPath);

            lock (_fileTypeMap)
            {
                if (extension.Length > 0)
                    _fileTypeMap[extension] = rslt;
            }

            return rslt;
        }

        public string GetFileType(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return "";

            extension = extension.TrimStart('.');

            lock (_fileTypeMap)
            {
                string rslt;
                if (_fileTypeMap.TryGetValue(extension, out rslt))
                    return rslt;
            }

            string typeName = GetTypeNameForExtension(extension);

            lock (_fileTypeMap)
            {
                _fileTypeMap[extension] = typeName;
            }

            return typeName;
        }

        static string GetTypeName(string path)
        {
            NativeMethods.SHFILEINFO fileinfo = new NativeMethods.SHFILEINFO();
            IntPtr rslt = NativeMethods.SHGetFileInfoW(path, 0, ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), NativeMethods.SHGFI_TYPENAME);

            if (rslt == IntPtr.Zero)
                return null;

            return fileinfo.szTypeName;
        }

        static string GetTypeNameForExtension(string extension)
        {
            Debug.Assert(!string.IsNullOrEmpty(extension));

            string dummyPath = Path.Combine(Path.GetTempPath(), "Dummy." + extension);

            NativeMethods.SHFILEINFO fileinfo = new NativeMethods.SHFILEINFO();
            IntPtr rslt = NativeMethods.SHGetFileInfoW(dummyPath, (uint)(FileAttributes.Normal), ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), NativeMethods.SHGFI_TYPENAME | NativeMethods.SHGFI_USEFILEATTRIBUTES);

            if (rslt == IntPtr.Zero)
                return null;

            return fileinfo.szTypeName;
        }

        IVsImageService2 ImageService
        {
            get
            {
                if (_imageService == null)
                {
                    _imageService = GetService<IVsImageService2>(typeof(SVsImageService));
                }

                return _imageService;
            }
        }

        int GetThemeIcon(string path)
        {
            IVsImageService2 imageService = ImageService;
            if (imageService == null)
                return -1;

            try
            {
                return ResolveMoniker(imageService.GetImageMonikerForFile(path));
            }
            catch (COMException)
            {
                return -1;
            }
            catch (ArgumentException)
            {
                return -1;
            }
        }

        static bool IsEmptyMoniker(ImageMoniker moniker)
        {
            return moniker.Guid == Guid.Empty && moniker.Id == 0;
        }

        static string GetMonikerKey(ImageMoniker moniker)
        {
            return moniker.Guid.ToString("N") + ":" + moniker.Id.ToString();
        }

        Bitmap RenderMoniker(ImageMoniker moniker)
        {
            if (IsEmptyMoniker(moniker) || ImageService == null)
                return null;

            ImageAttributes attributes = new ImageAttributes
            {
                StructSize = Marshal.SizeOf(typeof(ImageAttributes)),
                ImageType = (uint)_UIImageType.IT_Bitmap,
                Format = (uint)_UIDataFormat.DF_WinForms,
                LogicalWidth = LogicalIconSize,
                LogicalHeight = LogicalIconSize,
                Dpi = _imageDpi,
                Flags = unchecked((uint)_ImageAttributesFlags.IAF_RequiredFlags)
            };

            IVsUIObject image = ImageService.GetImage(moniker, attributes);
            if (image == null)
                return null;

            object data;
            if (!VSErr.Succeeded(image.get_Data(out data)))
                return null;

            Bitmap bitmap = data as Bitmap;
            if (bitmap == null)
                return null;

            // Detach the managed bitmap from the COM image object before the
            // latter is released. ImageList owns this cloned bitmap afterward.
            return new Bitmap(bitmap);
        }

        int ResolveMoniker(ImageMoniker moniker)
        {
            if (IsEmptyMoniker(moniker))
                return -1;

            string key = GetMonikerKey(moniker);
            int value;
            if (_monikerMap.TryGetValue(key, out value))
                return value;

            Bitmap bitmap;
            try
            {
                bitmap = RenderMoniker(moniker);
            }
            catch (COMException)
            {
                return -1;
            }
            catch (ArgumentException)
            {
                return -1;
            }

            if (bitmap == null)
                return -1;

            try
            {
                _imageList.Images.Add(bitmap);
            }
            catch
            {
                bitmap.Dispose();
                throw;
            }

            value = _imageList.Images.Count - 1;
            _monikerMap[key] = value;
            _monikers[key] = moniker;
            return value;
        }

        int GetOsIcon(string path)
        {
            NativeMethods.SHFILEINFO fileinfo = new NativeMethods.SHFILEINFO();
            IntPtr sysImageList = NativeMethods.SHGetFileInfoW(path, 0, ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), NativeMethods.SHGFI_SHELLICONSIZE |
                NativeMethods.SHGFI_SYSICONINDEX | NativeMethods.SHGFI_SMALLICON);

            if (sysImageList == IntPtr.Zero)
                return -1;

            ProjectIconReference handle = new ProjectIconReference(sysImageList, fileinfo.iIcon);

            return ResolveReference(handle);
        }

        int GetProjectIcon(string path)
        {
            EnsureSpecialImages();

            IProjectFileMapper map = GetService<IProjectFileMapper>();

            if (map == null)
                return -1;

            ProjectIconReference handle = map.GetPathIconHandle(path);

            return ResolveReference(handle);
        }

        private int ResolveReference(ProjectIconReference handle)
        {
            if (handle == null)
                return -1;

            int value;
            if (_iconMap.TryGetValue(handle, out value))
                return value;

            using (handle)
            {
                IntPtr iconHandle = handle.GetHandle();

                if (iconHandle == IntPtr.Zero)
                    return -1;

                Icon icon;
                try
                {
                    icon = Icon.FromHandle(iconHandle);
                }
                catch (ArgumentException)
                {   // Win32 handle that was passed to Icon is not valid or is the wrong type. 
                    return -1;
                }

                try
                {
                    // Clone the icon into a managed bitmap while the native icon
                    // handle is still valid. Keeping Icon.FromHandle() instances in
                    // the ImageList can leave it holding images backed by handles
                    // that ProjectIconReference disposes below. Newer WinForms
                    // (notably VS 2026) validates those images when the ImageList
                    // handle is created and throws ArgumentException.
                    // ImageList defers creation of its native HIMAGELIST until the
                    // control asks for Handle. Keep the managed bitmap alive until
                    // ImageList itself is disposed; disposing it immediately after
                    // Images.Add() leaves newer WinForms holding an invalid image.
                    Bitmap bitmap = icon.ToBitmap();
                    _imageList.Images.Add(bitmap);
                }
                catch (InvalidOperationException)
                {
                    // Unmanaged add icon operation failed (Reported on mailinglist)
                    return -1;
                }
                catch (ArgumentException)
                {
                    // Invalid or stale native icon data must not prevent tool
                    // windows such as Pending Changes from being constructed.
                    return -1;
                }
            }

            int n = _imageList.Images.Count - 1;
            _iconMap.Add(handle, n);
            return n;
        }

        public ImageList ImageList
        {
            get { return _imageList; }
        }

        int _dirIcon;
        public int DirectoryIcon
        {
            get
            {
                if (_dirIcon > 0)
                    return _dirIcon - 1;

                EnsureSpecialImages();

                int n = ResolveMoniker(KnownMonikers.FolderClosed);
                if (n < 0)
                    n = GetSpecialIcon(Path.GetTempPath(), FileAttributes.Directory);

                if (n >= 0)
                    _dirIcon = n + 1;

                return n;
            }
        }

        int _fileIcon;
        public int FileIcon
        {
            get
            {
                if (_fileIcon > 0)
                    return _fileIcon - 1;

                EnsureSpecialImages();

                int n = ResolveMoniker(KnownMonikers.Document);
                if (n < 0)
                {
                    string dummyPath = Path.Combine(Path.GetTempPath(), "Dummy");
                    n = GetSpecialIcon(dummyPath, FileAttributes.Normal);
                }

                if (n >= 0)
                    _fileIcon = n + 1;

                return n;
            }
        }

        int GetSpecialIcon(string name, FileAttributes attr)
        {
            EnsureSpecialImages();

            NativeMethods.SHFILEINFO fileinfo = new NativeMethods.SHFILEINFO();
            IntPtr sysImageList = NativeMethods.SHGetFileInfoW(name, (uint)(int)attr, ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), NativeMethods.SHGFI_SHELLICONSIZE |
                NativeMethods.SHGFI_SYSICONINDEX | NativeMethods.SHGFI_SMALLICON | NativeMethods.SHGFI_USEFILEATTRIBUTES);

            if (sysImageList == IntPtr.Zero)
                return -1;

            ProjectIconReference handle = new ProjectIconReference(sysImageList, fileinfo.iIcon);

            return ResolveReference(handle);
        }

        public int GetIconForExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return FileIcon;

            EnsureSpecialImages();

            string dummyPath = "c:\\file." + ext.Trim('.');
            int icon = GetThemeIcon(dummyPath);

            if (icon < 0)
                icon = GetSpecialIcon(dummyPath, FileAttributes.Normal);

            return icon;
        }

        int _lvUp;
        public int GetSpecialIcon(SpecialIcon icon)
        {
            EnsureSpecialImages();

            return _lvUp - (int)(SpecialIcon.SortUp) + (int)icon;
        }

        public int GetSpecialFolderIcon(Environment.SpecialFolder folder)
        {
            return GetSpecialFolderIcon((WindowsSpecialFolder)(int)folder);
        }

        public int GetSpecialFolderIcon(WindowsSpecialFolder folder)
        {
            EnsureSpecialImages();

            int index;

            if (_folderMap.TryGetValue(folder, out index))
                return index;

            IntPtr pidl = IntPtr.Zero;
            try
            {
                if (VSErr.S_OK != NativeMethods.SHGetFolderLocation(IntPtr.Zero, folder, IntPtr.Zero, 0, out pidl))
                    return -1;


                NativeMethods.SHFILEINFO fileinfo = new NativeMethods.SHFILEINFO();
                IntPtr sysImageList = NativeMethods.SHGetFileInfoW(pidl, (uint)(int)FileAttributes.Directory, ref fileinfo,
                                                            (uint)Marshal.SizeOf(fileinfo), NativeMethods.SHGFI_SHELLICONSIZE |
                                                            NativeMethods.SHGFI_SYSICONINDEX | NativeMethods.SHGFI_SMALLICON | NativeMethods.SHGFI_PIDL);

                if (sysImageList == IntPtr.Zero)
                    return -1;

                ProjectIconReference handle = new ProjectIconReference(sysImageList, fileinfo.iIcon);

                return _folderMap[folder] = ResolveReference(handle);
            }
            finally
            {
                if (pidl != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pidl);
            }
        }

        public ImageList StateImageList
        {
            // For now we use the same image list for both; the first few icons are just reused
            // Our api allows us to change this later on
            get { return ImageList; }
        }

        public int GetStateIcon(StateIcon icon)
        {
            // For now we use the same image list for both; the first few icons are just reused
            // Our api allows us to change this later on
            SpecialIcon si;
            switch (icon)
            {
                case StateIcon.Blank:
                    si = SpecialIcon.Blank;
                    break;
                case StateIcon.Incoming:
                    si = SpecialIcon.Incoming;
                    break;
                case StateIcon.Outgoing:
                    si = SpecialIcon.Outgoing;
                    break;
                case StateIcon.Collision:
                    si = SpecialIcon.Collision;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("icon", icon, "Icon out of range");
            }

            return GetSpecialIcon(si);
        }

        static ImageMoniker GetSpecialMoniker(SpecialIcon icon)
        {
            switch (icon)
            {
                case SpecialIcon.Blank:
                    return KnownMonikers.Blank;
                case SpecialIcon.SortUp:
                    return KnownMonikers.SortAscending;
                case SpecialIcon.SortDown:
                    return KnownMonikers.SortDescending;
                case SpecialIcon.Servers:
                case SpecialIcon.Server:
                    return KnownMonikers.DataServer;
                case SpecialIcon.Db:
                    return KnownMonikers.Database;
                case SpecialIcon.Collision:
                    return KnownMonikers.Conflict;
                default:
                    // Incoming/outgoing are Ankh-specific synchronization
                    // concepts; retain their existing artwork until there is a
                    // semantically equivalent Visual Studio catalog image.
                    return default(ImageMoniker);
            }
        }

        Bitmap RenderLegacySpecialImage(Image strip, int index)
        {
            const int sourceIconSize = 16;
            Size iconSize = _imageList.ImageSize;
            Bitmap icon = new Bitmap(iconSize.Width, iconSize.Height);

            using (Graphics graphics = Graphics.FromImage(icon))
            {
                graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                graphics.PixelOffsetMode = PixelOffsetMode.Half;
                graphics.DrawImage(strip,
                    new Rectangle(0, 0, iconSize.Width, iconSize.Height),
                    new Rectangle(index * sourceIconSize, 0, sourceIconSize, sourceIconSize),
                    GraphicsUnit.Pixel);
            }

            return icon;
        }

        void EnsureSpecialImages()
        {
            if (_lvUp != 0)
                return;

            using (Image strip = Bitmap.FromStream(typeof(FileIconMapper).Assembly.GetManifestResourceStream(
                typeof(FileIconMapper).Namespace + ".UpDnListView.png")))
            {
                int baseIndex = _imageList.Images.Count;
                int count = Enum.GetValues(typeof(SpecialIcon)).Length;

                for (int i = 0; i < count; i++)
                {
                    SpecialIcon special = (SpecialIcon)i;
                    ImageMoniker moniker = GetSpecialMoniker(special);
                    Bitmap icon = null;

                    if (!IsEmptyMoniker(moniker))
                    {
                        try
                        {
                            icon = RenderMoniker(moniker);
                        }
                        catch (COMException)
                        {
                        }
                        catch (ArgumentException)
                        {
                        }
                    }

                    if (icon == null)
                        icon = RenderLegacySpecialImage(strip, i);

                    _imageList.Images.Add(icon);

                    if (!IsEmptyMoniker(moniker))
                        _monikerMap[GetMonikerKey(moniker)] = baseIndex + i;
                }

                _lvUp = baseIndex + (int)SpecialIcon.SortUp;
            }
        }

        static class NativeMethods
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public int iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            }

            public const uint SHGFI_SMALLICON = 0x1;
            public const uint SHGFI_SHELLICONSIZE = 0x4;
            public const uint SHGFI_PIDL = 0x000000008;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x10;
            public const uint SHGFI_TYPENAME = 0x000000400;
            public const uint SHGFI_SYSICONINDEX = 0x4000;



            [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            public static extern IntPtr SHGetFileInfoW(string pszPath, uint dwFileAttributes,
                ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("shell32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
            public static extern IntPtr SHGetFileInfoW(IntPtr pidl, uint dwFileAttributes,
                ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

            [DllImport("shell32.dll")]
            public static extern int SHGetFolderLocation(IntPtr hwndOwner,
                [MarshalAs(UnmanagedType.I4)]WindowsSpecialFolder nFolder,
                IntPtr hToken, uint dwReserved, out IntPtr ppidl);
        }
    }
}
