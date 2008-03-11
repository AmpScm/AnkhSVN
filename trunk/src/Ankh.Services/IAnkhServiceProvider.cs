using System;
using System.Collections.Generic;
using System.Text;

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
}
