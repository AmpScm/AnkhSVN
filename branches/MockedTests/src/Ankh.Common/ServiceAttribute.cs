using System;

namespace Ankh
{
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
