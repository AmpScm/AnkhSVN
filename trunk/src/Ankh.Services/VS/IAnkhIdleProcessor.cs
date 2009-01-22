using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    public interface IAnkhIdleProcessor
    {
        bool OnIdle(bool periodic);
    }
}
