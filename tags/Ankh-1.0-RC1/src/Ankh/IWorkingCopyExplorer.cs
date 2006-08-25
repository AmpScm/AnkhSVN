using System;
using System.Text;
using Ankh.UI;

namespace Ankh
{
    public interface IWorkingCopyExplorer : ISelectionContainer
    {
        void AddRoot( string directory );

        void RemoveRoot( string directory );

        IContextMenu ContextMenu { get; set; }
    }
}
