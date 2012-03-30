using System;

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
