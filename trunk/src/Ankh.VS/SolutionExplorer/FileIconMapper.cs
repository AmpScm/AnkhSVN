using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace Ankh.VS.SolutionExplorer
{
    [GlobalService(typeof(IFileIconMapper))]
    class FileIconMapper : AnkhService, IFileIconMapper
    {
        readonly ImageList _imageList;
        readonly Dictionary<ProjectIconReference, int> _iconMap;        

        public FileIconMapper(IAnkhServiceProvider context)
            : base(context)
        {
            _imageList = new ImageList();
            _imageList.ImageSize = new System.Drawing.Size(16, 16);
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _iconMap = new Dictionary<ProjectIconReference, int>();
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

        int GetOsIcon(string path)
        {
            NativeMethods.SHFILEINFO fileinfo = new NativeMethods.SHFILEINFO();
            IntPtr sysImageList = NativeMethods.SHGetFileInfo(path, 0, ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), NativeMethods.SHGFI_SHELLICONSIZE |
                NativeMethods.SHGFI_SYSICONINDEX | NativeMethods.SHGFI_SMALLICON);

            if (sysImageList == IntPtr.Zero)
                return -1;

            ProjectIconReference handle = new ProjectIconReference(sysImageList, (int)fileinfo.iIcon);

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

                Icon icon = Icon.FromHandle(iconHandle);

                if (icon == null)
                    return -1;

                try
                {
                    _imageList.Images.Add(icon);
                }
                catch(InvalidOperationException) 
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
            IntPtr sysImageList = NativeMethods.SHGetFileInfo(name, (uint)(int)attr, ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), NativeMethods.SHGFI_SHELLICONSIZE |
                NativeMethods.SHGFI_SYSICONINDEX | NativeMethods.SHGFI_SMALLICON | NativeMethods.SHGFI_USEFILEATTRIBUTES);

            if (sysImageList == IntPtr.Zero)
                return -1;

            ProjectIconReference handle = new ProjectIconReference(sysImageList, (int)fileinfo.iIcon);

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
            if ((_lvUp == 0))
                EnsureSpecialImages();

            return _lvUp - (int)(SpecialIcon.SortUp) + (int)icon;
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
            switch(icon)
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
            [StructLayout(LayoutKind.Sequential)]
            public struct SHFILEINFO
            {
                public IntPtr hIcon;
                public IntPtr iIcon;
                public uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
                public string szTypeName;
            }

            public const uint SHGFI_SMALLICON = 0x1; 
            public const uint SHGFI_SYSICONINDEX = 0x4000;
            public const uint SHGFI_SHELLICONSIZE = 0x4;
            public const uint SHGFI_USEFILEATTRIBUTES = 0x10;

            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes,
                ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
        }
    }
}
