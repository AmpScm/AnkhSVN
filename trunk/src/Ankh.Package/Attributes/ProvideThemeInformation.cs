using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.VSPackage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    internal sealed class ProvideThemeInformationAttribute : RegistrationAttribute
    {
        readonly string _theme;
        readonly int flags;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProvideTextMarkerAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="regName">Name of the reg.</param>
        public ProvideThemeInformationAttribute(string theme, bool light)
        {
            _theme = theme;
            flags = light ? 0x01 : 0x02;
        }

        /// <summary>
        /// Gets the registry path.
        /// </summary>
        /// <value>The registry path.</value>
        public string RegistryPath
        {
            get { return "Extensions\\AnkhSVN\\Themes"; }
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
                key.SetValue(Theme, flags);
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
            context.RemoveValue(RegistryPath, new Guid(_theme).ToString("B"));
            context.RemoveKeyIfEmpty(RegistryPath);
        }

        /// <summary>
        /// Gets the name of the reg.
        /// </summary>
        /// <value>The name of the reg.</value>
        public string Theme
        {
            get { return new Guid(_theme).ToString("B"); }
        }
    }
}
