using System;

namespace Ankh
{
    /// <summary>
    /// Represents a class that handles an Ankh error.
    /// </summary>
    public interface IAnkhErrorHandler
    {
        void OnError(Exception ex);
    }
}
