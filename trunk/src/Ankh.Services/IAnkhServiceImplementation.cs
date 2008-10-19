using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh
{
    public interface IAnkhServiceImplementation
    {
        /// <summary>
        /// Called when the service is instantiated
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnPreInitialize();

        /// <summary>
        /// Called after all modules and services received their OnPreInitialize
        /// </summary>
        /// <param name="e"></param>
        void OnInitialize();
    }
}
