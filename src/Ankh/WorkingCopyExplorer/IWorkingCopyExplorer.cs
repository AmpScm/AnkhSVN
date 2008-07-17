using System;
using System.Text;
using Ankh.UI;

namespace Ankh
{
    public interface IWorkingCopyExplorer
    {
        void AddRoot( string directory );
        bool IsRootSelected { get; }
        void RemoveSelectedRoot();

        // Temporary helper
        void SetControl(WorkingCopyExplorerControl wcControl);
    }
}
