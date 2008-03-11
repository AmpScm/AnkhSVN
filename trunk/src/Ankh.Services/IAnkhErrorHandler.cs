using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh
{
    public interface IAnkhErrorHandler
    {
        void OnError(Exception e);
    }
}
