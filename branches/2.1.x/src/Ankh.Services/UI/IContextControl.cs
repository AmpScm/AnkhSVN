using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    public interface IContextControl
    {
        IAnkhServiceProvider Context { get; }
    }
}
