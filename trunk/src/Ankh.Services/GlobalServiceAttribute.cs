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
