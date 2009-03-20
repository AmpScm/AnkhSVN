// $Id$
//
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
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.VisualStudio;
using SharpSvn;
using System.Diagnostics;

namespace Ankh.VS.SolutionExplorer
{
    [GlobalService(typeof(IFileIconMapper))]
    class FileIconMapper : AnkhService, IFileIconMapper
    {
        readonly ImageList _imageList;
        readonly Dictionary<ProjectIconReference, int> _iconMap;
        readonly SortedList<WindowsSpecialFolder, int> _folderMap;
        readonly Dictionary<string, string> _fileTypeMap;

        public FileIconMapper(IAnkhServiceProvider context)
            : base(context)
        {
            _imageList = new ImageList();
            _imageList.ImageSize = new System.Drawing.Size(16, 16);
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _iconMap = new Dictionary<ProjectIconReference, int>();
            _folderMap = new SortedList<WindowsSpecialFolder, int>();
            _fileTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public int GetIcon(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            EnsureSpecialImages();

            int icon = GetProjectIcon(path);

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

        string GetTypeName(string path)
        {
            NativeMethods.SHFILEINFO fileinfo = new NativeMethods.SHFILEINFO();
            IntPtr rslt = NativeMethods.SHGetFileInfoW(path, 0, ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), NativeMethods.SHGFI_TYPENAME);

            if (rslt == IntPtr.Zero)
                return null;

            return fileinfo.szTypeName;
        }

        string GetTypeNameForExtension(string extension)
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

                if (icon == null)
                    return -1;

                try
                {
                    _imageList.Images.Add(icon);
                }
                catch (InvalidOperationException)
                {
                    // Unmanaged add icon operation failed (Reported on mailinglist)
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

                int n = GetSpecialIcon("", FileAttributes.Directory);

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

                int n = GetSpecialIcon("", FileAttributes.Normal);

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

            return GetSpecialIcon("c:\\file." + ext.Trim('.'), FileAttributes.Normal);
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
                if (VSConstants.S_OK != NativeMethods.SHGetFolderLocation(IntPtr.Zero, folder, IntPtr.Zero, 0, out pidl))
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

        void EnsureSpecialImages()
        {
            if ((_lvUp != 0))
                return;

            Image img = Bitmap.FromStream(typeof(FileIconMapper).Assembly.GetManifestResourceStream(
                typeof(FileIconMapper).Namespace + ".UpDnListView.png"));

            int count = img.Width / 16;
            _imageList.Images.AddStrip(img);

            _lvUp = _imageList.Images.Count - count + 1;
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
