﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.VS
{
    public interface IFileIconMapper
    {
        int GetIcon(string path);
        ImageList ImageList { get; }

        int DirectoryIcon { get; }
        int FileIcon { get; }

        int GetIconForExtension(string ext);
    }
}
