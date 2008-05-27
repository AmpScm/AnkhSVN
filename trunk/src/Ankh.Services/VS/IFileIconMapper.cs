using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.VS
{
    public enum SpecialIcon
    {
        SortUp=0,
        SortDown,
        Servers,        
        Db,
        Server
    }

    public interface IFileIconMapper
    {
        int GetIcon(string path);
        ImageList ImageList { get; }

        int DirectoryIcon { get; }
        int FileIcon { get; }

        int GetIconForExtension(string ext);

        int HeaderUpIcon { get; }
        int HeaderDownIcon { get; }

        int GetSpecialIcon(SpecialIcon icon);
    }
}
