using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ankh.UI
{
    public interface IAnkhThreadedWaitDialog : IDisposable
    {
        void Tick();
    }

    public interface IAnkhThreadedWaitService
    {
        IAnkhThreadedWaitDialog Start(string caption, string message);
    }
}
