using System;
using System.Collections.Generic;
using System.Text;
using MsVsShell = Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VSPackage.Attributes
{
    public sealed class ProvideUIVersionAttribute : MsVsShell.RegistrationAttribute
    {
        public ProvideUIVersionAttribute()
        {
        }

        internal const string RemapName = "AnkhSVN-UI-Version";
        string GetPath(RegistrationAttribute.RegistrationContext context)
        {
            return "Packages\\" + context.ComponentType.GUID.ToString("B").ToUpperInvariant();
        }

        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            // Create the visibility key.
            using (Key childKey = context.CreateKey(GetPath(context)))
            {
                // Set the value for the command UI guid.
                childKey.SetValue(RemapName, "[ProductVersion]");
            }
        }

        public override void Unregister(Microsoft.VisualStudio.Shell.RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveValue(GetPath(context), RemapName);
        }
    }
}
