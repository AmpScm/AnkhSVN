using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc;
using System.Drawing;

namespace Ankh.UI.PendingChanges
{
    class FileIconMap : AnkhService
    {
        readonly ImageList _imageList;
        readonly Dictionary<ProjectIconReference, int> _iconMap;        

        public FileIconMap(IAnkhServiceProvider context)
            : base(context)
        {
            _imageList = new ImageList();
            _imageList.ImageSize = new System.Drawing.Size(16, 16);
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _iconMap = new Dictionary<ProjectIconReference, int>();
        }

        public int GetIcon(string path)
        {
            IProjectFileMapper map = GetService<IProjectFileMapper>();

            if (map == null)
                return -1;

            ProjectIconReference handle = map.GetPathIconHandle(path);

            if (handle == null)
                return -1;

            int value;
            if (_iconMap.TryGetValue(handle, out value))
                return value;

            using(handle)
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
    }
}
