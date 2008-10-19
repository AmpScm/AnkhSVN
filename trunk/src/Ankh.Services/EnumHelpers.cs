using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh
{
    public static class EnumTools
    {
        /// <summary>
        /// Gets the first item from an IEnumerable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static T GetFirst<T>(IEnumerable<T> item)
        {
            if(item != null)
                foreach (T i in item)
                {
                    return i;
                }

            return default(T);
        }
    }
}
