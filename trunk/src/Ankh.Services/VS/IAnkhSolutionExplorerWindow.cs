using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    public interface IAnkhSolutionExplorerWindow
    {
        void Show();

        void EnableAnkhIcons(bool enabled);
    }
}
