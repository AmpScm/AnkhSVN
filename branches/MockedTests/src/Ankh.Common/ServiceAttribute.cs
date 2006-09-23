using System;

namespace Ankh
{
    /// <summary>
    /// Attribute used to have a class registered as service. The class should implement a public constructor
    /// that takes an System.IServiceProvider. Services are loaded in undetermined order, move initialization that
    /// depends on other services being created to void Init() and set the CallMyInit property to true
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ServiceAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">The type used to store this service</param>
        public ServiceAttribute(Type type)
        {
            this.type = type;
        }

        /// <summary>
        /// The type used to store this service
        /// </summary>
        public Type ServiceType
        {
            get { return this.type; }
        }

        /// <summary>
        /// If set to true, the loader attempts to call <c>void Init()</c> on the service
        /// </summary>
        public bool CallMyInit
        {
            get { return this.callInit; }
            set { this.callInit = value; }
        }

        private bool callInit;
        private Type type;
    }
}
