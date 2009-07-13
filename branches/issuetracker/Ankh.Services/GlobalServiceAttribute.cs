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

namespace Ankh
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
    public sealed class GlobalServiceAttribute : Attribute
    {
        readonly Type _serviceType;
        bool _publicService;
        bool _skipIfPreRegistered;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhGlobalServiceAttribute"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        public GlobalServiceAttribute(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            _serviceType = serviceType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnkhGlobalServiceAttribute"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="publicService">if set to <c>true</c> [public service].</param>
        public GlobalServiceAttribute(Type serviceType, bool publicService)
            : this(serviceType)
        {
            PublicService = publicService;
        }

        /// <summary>
        /// Gets the type of the service to register.
        /// </summary>
        /// <value>The type of the service.</value>
        public Type ServiceType
        {
            get { return _serviceType; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the service should also be registered in VS.
        /// </summary>
        /// <value><c>true</c> if [public service]; otherwise, <c>false</c>.</value>
        /// <remarks>Passed as 'promote' parameter to the package service provider</remarks>
        public bool PublicService
        {
            get { return _publicService; }
            set { _publicService = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to check if the service is registered before this
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [allow other implemenations]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowPreRegistered
        {
            get { return _skipIfPreRegistered; }
            set { _skipIfPreRegistered = value; }
        }
    }
}
