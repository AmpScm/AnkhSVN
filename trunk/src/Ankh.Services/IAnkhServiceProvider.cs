using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Ankh
{
    public interface IAnkhServiceProvider : IServiceProvider
    {
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        T GetService<T>();
    }

    /// <summary>
    /// Simple service container implementing <see cref="IAnkhServiceProvider"/>
    /// </summary>
    public class AnkhServiceContainer : ServiceContainer, IAnkhServiceProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhServiceContainer"/> class.
        /// </summary>
        /// <param name="parentProvider">A parent service provider.</param>
        public AnkhServiceContainer(IServiceProvider parentProvider)
            : base(parentProvider)
        {
            AddService(typeof(IAnkhServiceProvider), this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhServiceContainer"/> class.
        /// </summary>
        public AnkhServiceContainer()
        {
            AddService(typeof(IAnkhServiceProvider), this);
        }

        #region IAnkhServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get</typeparam>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        [DebuggerStepThrough]
        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        #endregion
    }

}
