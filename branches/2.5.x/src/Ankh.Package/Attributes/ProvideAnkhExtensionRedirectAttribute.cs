using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VSPackage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    sealed class ProvideAnkhExtensionRedirectAttribute : RegistrationAttribute
    {
        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            AssemblyName name = typeof(Ankh.ExtensionPoints.IssueTracker.IssueRepositorySettings).Assembly.GetName();
            using (Key key = context.CreateKey(GetKey()))
            {
                key.SetValue("name", name.Name);
                key.SetValue("culture", "neutral");
                key.SetValue("publicKeyToken", TokenToString(name.GetPublicKeyToken()));
                key.SetValue("oldVersion", "2.1.7172.0-" + name.Version);
                key.SetValue("newVersion", name.Version);
                if (context.GetType().Name.ToUpperInvariant().Contains("PKGDEF"))
                    key.SetValue("codeBase", Path.Combine("$PackageFolder$", name.Name + ".dll"));
                else
                    key.SetValue("codeBase", "[#CF_" + name.Name + ".dll" + "]");
            }
        }

        private string TokenToString(byte[] bytes)
        {
            StringBuilder sb = new StringBuilder(16);

            foreach (byte b in bytes)
                sb.AppendFormat("{0:x2}", b);

            return sb.ToString();
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(GetKey());
        }

        private string GetKey()
        {
            return @"RuntimeConfiguration\dependentAssembly\bindingRedirection\" + new Guid(AnkhId.ExtensionRedirectId).ToString("B");
        }

    }
}
