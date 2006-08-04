using System;
using System.Text;

namespace Ankh.UI
{
    public interface IFileSystemItem
    {
        event EventHandler Changed;
    
        bool IsContainer
        {
            get;
        }

        string Text
        {
            get;
        }

        void Open();

        IFileSystemItem[] GetChildren();
    }
}
