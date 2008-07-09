using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    /// <summary>
    /// Todo: make some subscriber model when we need more than 1 listener
    /// </summary>
    public interface ILastChangeInfo
    {
        void SetLastChange(string caption, string value);
    }
}
