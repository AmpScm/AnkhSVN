using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.Shell;

namespace Ankh.VSPackage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    sealed class ProvideAnkhVersionThunkRedirectAttribute : RegistrationAttribute
    {
        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            AssemblyName name = typeof(Ankh.VS.VSVersionThunk).Assembly.GetName();
            using (Key key = context.CreateKey(GetKey()))
            {
                key.SetValue("name", name.Name);
                key.SetValue("culture", "neutral");
                key.SetValue("publicKeyToken", TokenToString(name.GetPublicKeyToken()));
                key.SetValue("version", name.Version);
                if (context.GetType().Name.ToUpperInvariant().Contains("PKGDEF"))
                {
                    string dllName = name.Name + "-V4.dll";

                    if (!context.GetType().Name.Contains("Ankh"))
                        dllName = name.Name + ".dll"; // Doesn't work in debug mode at this time

                    key.SetValue("codeBase", Path.Combine("$PackageFolder$", dllName));
                }
                else
                    key.SetValue("codeBase", "[#CF_" + name.Name + ".V4.dll" + "]");
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
            return @"RuntimeConfiguration\dependentAssembly\codeBase\" + new Guid(AnkhId.VersionThunkId).ToString("B");
        }

    }
}
