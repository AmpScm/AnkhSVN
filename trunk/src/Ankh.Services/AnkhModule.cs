﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace Ankh
{
    public abstract class Module : IAnkhServiceProvider
    {
        readonly IServiceContainer _container;
        readonly AnkhRuntime _runtime;
        readonly AnkhContext _context;

        protected Module(AnkhRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            _container = runtime.GetService<IServiceContainer>();
            _runtime = runtime;
            _context = runtime.Context;
        }

        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>The container.</value>
        public IServiceContainer Container
        {
            get { return _container; }
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public AnkhContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the runtime.
        /// </summary>
        /// <value>The runtime.</value>
        public AnkhRuntime Runtime
        {
            get { return _runtime; }
        }

        /// <summary>
        /// Called when added to the <see cref="AnkhRuntime"/>
        /// </summary>
        public abstract void OnPreInitialize();

        /// <summary>
        /// Called when <see cref="AnkhRuntime.Start"/> is called
        /// </summary>
        public abstract void OnInitialize();


        /// <summary>
        /// Ensures the service exists when using the testing infrastructure; skipped in release builds
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [Conditional("Debug")]
        protected void EnsureService<T>()
        {
            if (Runtime.PreloadServicesViaEnsure)
            {
                Debug.Assert(GetService(typeof(T)) != null, string.Format("{0} service is not registered", typeof(T).FullName));
            }
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

        #region IServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        [DebuggerStepThrough]
        public object GetService(Type serviceType)
        {
            return _container.GetService(serviceType);
        }

        #endregion
    }
}
