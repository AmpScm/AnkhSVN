using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.Scc
{
    public interface IStatusImageMapper
    {
        int GetStatusImageForNodeStatus(NodeStatus status);

        ImageList StatusImageList { get; }
    }
}
