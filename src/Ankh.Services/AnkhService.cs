using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;

namespace Ankh
{
    /// <summary>
    /// Generic service baseclass
    /// </summary>
    public abstract class AnkhService : IAnkhServiceProvider
    {
        readonly IAnkhServiceProvider _context;
        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected AnkhService(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;

        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        #region IAnkhServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        T IAnkhServiceProvider.GetService<T>()
        {
            return _context.GetService<T>();
        }

        /// <summary>
        /// Gets the service of the specified type safely casted to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        T IAnkhServiceProvider.GetService<T>(Type type)
        {
            return _context.GetService<T>(type);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        protected T GetService<T>()
            where T : class
        {
            return _context.GetService<T>();
        }

        /// <summary>
        /// Gets the service of the specified type safely casted to T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        protected T GetService<T>(Type type)
            where T : class
        {
            return _context.GetService<T>(type);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        object IServiceProvider.GetService(Type serviceType)
        {
            return _context.GetService(serviceType);
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        protected object GetService(Type serviceType)
        {
            return _context.GetService(serviceType);
        }

        #endregion

        /// <summary>
        /// Gets the service container.
        /// </summary>
        /// <value>The service container.</value>
        protected IServiceContainer ServiceContainer
        {
            get { return _context.GetService<IServiceContainer>(); }
        }
    }
}
