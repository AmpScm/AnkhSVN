﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.Ids;

namespace Ankh.VSPackage.Attributes
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class ProvideTextMarkerAttribute : RegistrationAttribute
    {
        readonly Type _providerType;
        readonly Type _markerType;
        readonly string _displayName;
        readonly string _regname;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvideTextMarkerAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="regName">Name of the reg.</param>
        public ProvideTextMarkerAttribute(Type providerType, Type markerType, string displayName, string regName)
        {
            _type = type;
            _displayName = displayName;
            _regname = regName;
        }

        /// <summary>
        /// Gets the registry path.
        /// </summary>
        /// <value>The registry path.</value>
        public string RegistryPath
        {
            get { return string.Format("Text Editor\\External Markers\\{0}", _markerType.GUID.ToString("B").ToUpper()); }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <devdoc>
        /// Called to register this attribute with the given context.  The context
        /// contains the location where the registration information should be placed.
        /// It also contains such as the type being registered, and path information.
        /// This method is called both for registration and unregistration.  The difference is
        /// that unregistering just uses a hive that reverses the changes applied to it.
        /// </devdoc>
        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            using (Key key = context.CreateKey(RegistryPath))
            {
                key.SetValue("", RegName);
                key.SetValue("Service", _providerType.GUID.ToString("B").ToUpperInvariant());
                key.SetValue("Package", PackageGuid.ToString("B").ToUpperInvariant());
                key.SetValue("DisplayName", DisplayName);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <include file="doc\RegistrationAttribute.uex" path="docs/doc[@for=&quot;Unregister&quot;]"/>
        /// <devdoc>
        /// Called to unregister this attribute with the given context.  The context
        /// contains the location where the registration information should be removed.
        /// It also contains things such as the type being unregistered, and path information.
        /// </devdoc>
        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(RegistryPath);
        }

        /// <summary>
        /// Gets the name of the reg.
        /// </summary>
        /// <value>The name of the reg.</value>
        public string RegName
        {
            get { return _regname; }
        }

        /// <summary>
        /// Get the package containing the UI name of the provider
        /// </summary>
        /// <value>The package GUID.</value>
        public Guid PackageGuid
        {
            get { return AnkhId.PackageGuid; }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return _displayName; }
        }
    }
}
