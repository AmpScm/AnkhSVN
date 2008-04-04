using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using System.Drawing;
using Utils.Win32;
using System.Runtime.InteropServices;

namespace Ankh.VS.SolutionExplorer
{
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

            int icon = GetProjectIcon(path);

            if (icon == -1)
                icon = GetOsIcon(path);

            return icon;

        }

        private int GetOsIcon(string path)
        {
            SHFILEINFO fileinfo = new SHFILEINFO();
            IntPtr sysImageList = Win32.SHGetFileInfo(path, 0, ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), Constants.SHGFI_SHELLICONSIZE |
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON);

            if (sysImageList == IntPtr.Zero)
                return -1;

            ProjectIconReference handle = new ProjectIconReference(sysImageList, (int)fileinfo.iIcon);

            return ResolveReference(handle);
        }

        int GetProjectIcon(string path)
        {
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
                _imageList.Images.Add(Icon.FromHandle(handle.GetHandle()));
            }

            int n = _imageList.Images.Count - 1;
            _iconMap.Add(handle, n);
            return n;
        }

        public ImageList ImageList
        {
            get { return _imageList; }
        }

        #region IFileIconMapper Members


        int _dirIcon;
        public int DirectoryIcon
        {
            get 
            {
                if (_dirIcon > 0)
                    return _dirIcon - 1;

                int n = GetSpecialIcon("", FileAttribute.Directory);

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

                int n = GetSpecialIcon("", FileAttribute.Normal);

                if (n >= 0)
                    _fileIcon = n + 1;

                return n;
            }
        }

        int GetSpecialIcon(string name, FileAttribute attr)
        {
            SHFILEINFO fileinfo = new SHFILEINFO();
            IntPtr sysImageList = Win32.SHGetFileInfo(name, (uint)(int)attr, ref fileinfo,
                (uint)Marshal.SizeOf(fileinfo), Constants.SHGFI_SHELLICONSIZE |
                Constants.SHGFI_SYSICONINDEX | Constants.SHGFI_SMALLICON | Constants.SHGFI_USEFILEATTRIBUTES);

            if (sysImageList == IntPtr.Zero)
                return -1;

            ProjectIconReference handle = new ProjectIconReference(sysImageList, (int)fileinfo.iIcon);

            return ResolveReference(handle);
        }

        #endregion

        #region IFileIconMapper Members


        public int GetIconForExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext))
                return _fileIcon;

            return GetSpecialIcon("c:\\file." + ext.Trim('.'), FileAttribute.Normal);
        }

        #endregion
    }
}
