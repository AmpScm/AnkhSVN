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
using System.Globalization;
using MsVsShell = Microsoft.VisualStudio.Shell;

namespace Ankh.VSPackage.Attributes
{
    /// <summary>
    /// This attribute registers the visibility of a Tools/Options property page.
    /// While Microsoft.VisualStudio.Shell allow registering a tools options page 
    /// using the ProvideOptionPageAttribute attribute, currently there is no better way
    /// of declaring the options page visibility, so a custom attribute needs to be used.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	internal sealed class ProvideToolsOptionsPageVisibilityAttribute : MsVsShell.RegistrationAttribute
    {
        private string _categoryName = null;
        private string _pageName = null;
        private Guid _commandUIGuid;

        /// <summary>
        /// </summary>
        public ProvideToolsOptionsPageVisibilityAttribute(string categoryName, string pageName, string commandUIGuid)
        {
            _categoryName = categoryName;
            _pageName = pageName;
            _commandUIGuid = new Guid(commandUIGuid);
        }

        /// <summary>
        /// The programmatic name for this category (non localized).
        /// </summary>
        public string CategoryName
        {
            get { return _categoryName; }
        }

        /// <summary>
        /// The programmatic name for this page (non localized).
        /// </summary>
        public string PageName
        {
            get { return _pageName; }
        }

        /// <summary>
        /// Get the command UI guid controlling the visibility of the page.
        /// </summary>
        public Guid CommandUIGuid
        {
            get { return _commandUIGuid; }
        }

        private string RegistryPath
        {
            get { return string.Format(CultureInfo.InvariantCulture, "ToolsOptionsPages\\{0}\\{1}\\VisibilityCmdUIContexts", CategoryName, PageName); }
        }

        /// <summary>
        ///     Called to register this attribute with the given context.  The context
        ///     contains the location where the registration inforomation should be placed.
        ///     It also contains other information such as the type being registered and path information.
        /// </summary>
        public override void Register(RegistrationContext context)
        {
            // Write to the context's log what we are about to do
            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture, "Opt.Page Visibility:\t{0}\\{1}, {2}\n", CategoryName, PageName, CommandUIGuid.ToString("B")));

            // Create the visibility key.
            using (Key childKey = context.CreateKey(RegistryPath))
            {
                // Set the value for the command UI guid.
                childKey.SetValue(CommandUIGuid.ToString("B").ToUpperInvariant(), 1);
            }
        }

        /// <summary>
        /// Unregister this visibility entry.
        /// </summary>
        public override void Unregister(RegistrationContext context)
        {
            context.RemoveValue(RegistryPath, CommandUIGuid.ToString("B"));
        }
    }
}
