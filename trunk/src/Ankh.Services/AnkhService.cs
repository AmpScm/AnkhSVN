// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.ComponentModel;

namespace Ankh
{
    /// <summary>
    /// Generic service baseclass
    /// </summary>
    public abstract class AnkhService : IAnkhServiceProvider, IComponent, IAnkhServiceImplementation
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
        /// Called when the service is instantiated
        /// </summary>
        protected virtual void OnPreInitialize()
        {
        }

        /// <summary>
        /// Called when the service is instantiated
        /// </summary>
        void IAnkhServiceImplementation.OnPreInitialize()
        {
            OnPreInitialize();
        }

        /// <summary>
        /// Called after all modules and services received their OnPreInitialize
        /// </summary>
        protected virtual void OnInitialize()
        {
        }

        /// <summary>
        /// Called after all modules and services received their OnPreInitialize
        /// </summary>
        void IAnkhServiceImplementation.OnInitialize()
        {
            OnInitialize();
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        protected IAnkhServiceProvider Context
        {
            [DebuggerStepThrough]
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
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
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
        [DebuggerStepThrough]
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
            [DebuggerStepThrough]
            get { return _context.GetService<IServiceContainer>(); }
        }

        #region IComponent Members

        /// <summary>
        /// Represents the method that handles the <see cref="E:System.ComponentModel.IComponent.Disposed"/> event of a component.
        /// </summary>
        public event EventHandler Disposed;

        ISite _site;
        /// <summary>
        /// Gets or sets the <see cref="T:System.ComponentModel.ISite"/> associated with the <see cref="T:System.ComponentModel.IComponent"/>.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.ISite"/> object associated with the component; or null, if the component does not have a site.</returns>
        ISite IComponent.Site
        {
            get { return _site; }
            set { _site = value; }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        #endregion

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed != null)
                Disposed(this, EventArgs.Empty);
        }
    }
}
