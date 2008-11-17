using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.VS
{
    public enum SpecialIcon
    {
        Blank = 0,
        SortUp,
        SortDown,
        Servers,
        Db,
        Server,
        Outgoing,
        Collision,
        Incoming
    }

    public enum StateIcon
    {
        Blank = 0,
        Outgoing,
        Collision,
        Incoming
    }

    public interface IFileIconMapper
    {
        int GetIcon(string path);
        ImageList ImageList { get; }

        ImageList StateImageList { get; }

        int DirectoryIcon { get; }
        int FileIcon { get; }

        int GetIconForExtension(string ext);

        int GetSpecialIcon(SpecialIcon icon);

        int GetStateIcon(StateIcon icon);
    }
}
