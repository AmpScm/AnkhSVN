using System;
using System.Text;
using Ankh.UI;

namespace Ankh
{
    public interface IWorkingCopyExplorer : ISelectionContainer
    {
        void AddRoot( string directory );
        IContextMenu ContextMenu { get; set; }
        bool IsRootSelected { get; }
        void RemoveSelectedRoot();


        // Temporary helper
        void SetControl(WorkingCopyExplorerControl wcControl);
    }
}
