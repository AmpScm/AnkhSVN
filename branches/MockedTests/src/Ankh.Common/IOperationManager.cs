using System;

namespace Ankh
{
    /// <summary>
    /// Defines a factory for writing operation feedback with a using pattern
    /// </summary>
    /// <example>
    /// using(OperationManager.RunOperation("Foo"))
    /// {
    ///     doStuff();
    /// }
    /// </example>
    public interface IOperationManager
    {
        /// <summary>
        /// To be called when starting the operation, passing the caption that should be output.
        /// </summary>
        /// <param name="caption"></param>
        /// <returns></returns>
        IDisposable RunOperation(string caption);
        
        /// <summary>
        /// Gets a value indicating wether one or more (nested) operations are running
        /// </summary>
        bool OperationRunning { get;}
    }
}
