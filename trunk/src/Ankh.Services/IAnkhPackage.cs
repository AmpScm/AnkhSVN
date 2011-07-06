// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using System.Windows.Forms;
using Ankh.VS;
using Microsoft.Win32;

namespace Ankh.UI
{
    public enum FrameCloseMode
    {
        NoSave = Microsoft.VisualStudio.Shell.Interop.__FRAMECLOSE.FRAMECLOSE_NoSave,
        SaveIfDirty = Microsoft.VisualStudio.Shell.Interop.__FRAMECLOSE.FRAMECLOSE_SaveIfDirty,
        PromptSave = Microsoft.VisualStudio.Shell.Interop.__FRAMECLOSE.FRAMECLOSE_PromptSave,
    }
    /// <summary>
    /// Public api of the ankh package as used by other components
    /// </summary>
    public interface IAnkhPackage : IAnkhServiceProvider, System.ComponentModel.Design.IServiceContainer, IAnkhQueryService
    {
        /// <summary>
        /// Gets the UI version. Retrieved from the registry after being installed by our MSI
        /// </summary>
        /// <value>The UI version.</value>
        Version UIVersion { get; }

        /// <summary>
        /// Gets the package version. The assembly version of Ankh.Package.dll
        /// </summary>
        /// <value>The package version.</value>
        Version PackageVersion { get; }

        void ShowToolWindow(AnkhToolWindow window);
        void ShowToolWindow(AnkhToolWindow window, int id, bool create);

        void CloseToolWindow(AnkhToolWindow toolWindow, int id, FrameCloseMode close);

        void RegisterIdleProcessor(IAnkhIdleProcessor processor);
        void UnregisterIdleProcessor(IAnkhIdleProcessor processor);

        AmbientProperties AmbientProperties { get; }

        bool LoadUserProperties(string streamName);

        // Summary:
        //     Gets the root registry key of the current Visual Studio registry hive.
        //
        // Returns:
        //     The root Microsoft.Win32.RegistryKey of the Visual Studio registry hive.
        RegistryKey ApplicationRegistryRoot { get; }

		/// <summary>
		/// Gets a registry key that can be used to store user data. 
		/// </summary>
		RegistryKey UserRegistryRoot { get; }
    }
}
