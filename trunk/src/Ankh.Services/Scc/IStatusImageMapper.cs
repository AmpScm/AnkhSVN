using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.Scc
{
    public interface IStatusImageMapper
    {
        AnkhGlyph GetStatusImageForSvnItem(SvnItem item);

        ImageList StatusImageList { get; }
    }
}
