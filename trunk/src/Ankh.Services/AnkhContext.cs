using System;
using System.Collections.Generic;
using System.Text;
using Ankh.ContextServices;
using System.Windows.Forms;

namespace Ankh
{
    /// <summary>
    /// Globally available context; the entry point for the service framework.
    /// </summary>
    /// <remarks>Members should only be added for the most common operations. Everything else should be handled via the <see cref="IAnkhServiceProvider"/> methods</remarks>
    public class AnkhContext : IAnkhServiceProvider, IServiceProvider
    {
        readonly AnkhRuntime _runtime;
        IAnkhOperationLogger _logger;

        protected AnkhContext(AnkhRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            _runtime = runtime;
        }

        // Only add members which are really needed
        // Implementations should always ask their parent service provider for an instance first

        /// <summary>
        /// Starts a logged operation which can be closed by disposing the returned <see cref="IDisposable"/>
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A disposable object to end the loggin</returns>
        public IDisposable BeginOperation(string message)
        {
            IAnkhOperationLogger logger = _logger ?? (_logger = GetService<IAnkhOperationLogger>());

            if (logger != null)
                return logger.BeginOperation(message);

            return null;
        }

        #region IAnkhServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public T GetService<T>()
        {
            return _runtime.GetService<T>();
        }

        #endregion

        #region IServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return _runtime.GetService(serviceType);
        }

        #endregion

        internal static AnkhContext Create(AnkhRuntime runtime)
        {
            return new AnkhContext(runtime);
        }

        /// <summary>
        /// Gets a IWin32Window handle to a window which should be used as dialog owner
        /// </summary>
        /// <value>The dialog owner or null if no dialog owner is registered.</value>
        public IWin32Window DialogOwner
        {
            get
            {
                IAnkhDialogOwner owner = GetService<IAnkhDialogOwner>();
                if (owner != null)
                    return owner.DialogOwner;
                else
                    return null;
            }
        }
    }
}
