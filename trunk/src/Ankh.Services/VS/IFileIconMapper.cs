using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.VS
{
    public enum SpecialIcon
    {
        Blank=0,
        SortUp,
        SortDown,
        Servers,        
        Db,
        Server,
        Incoming,
        Collision,
        Outgoing        
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
