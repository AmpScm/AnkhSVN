﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI;

namespace Ankh.VS
{
    public interface IAnkhDialogOwner
    {
        /// <summary>
        /// Gets the dialog owner.
        /// </summary>
        /// <value>The dialog owner.</value>
        IWin32Window DialogOwner { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="eventArgs"></param>
        /// <returns></returns>
        IDisposable InstallFormRouting(VSContainerForm container, EventArgs eventArgs);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vSContainerForm"></param>
        void OnContainerCreated(VSContainerForm vSContainerForm);
    }
}
