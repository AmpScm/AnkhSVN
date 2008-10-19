using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.Ids;

namespace Ankh.VSPackage.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    class ProvideLanguageSettingsAttribute : RegistrationAttribute
    {
        readonly string _key;
        readonly string _name;
        readonly int _n, _desc;
        public ProvideLanguageSettingsAttribute(Type type, string key, string name, int nameId, int descriptionId)
        {
            _key = key;
            _name = name;
            _n = nameId;
            _desc = descriptionId;
        }

        /// <summary>
        /// Get the package containing the UI name of the provider
        /// </summary>
        public Guid UINamePkg
        {
            get { return AnkhId.PackageGuid; }
        }

        string GetKey(string key)
        {
            return "AutomationProperties\\TextEditor\\" + key;
        }

        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            using(Key key = context.CreateKey(GetKey(_key)))
            {
                key.SetValue("", string.Format("#{0}", _n));
                key.SetValue("Description", string.Format("#{0}", _desc));
                key.SetValue("Name", _name);
                key.SetValue("Package", UINamePkg.ToString("B").ToUpperInvariant());
                key.SetValue("ProfileSave", 1);
                key.SetValue("ResourcePackage", UINamePkg.ToString("B").ToUpperInvariant());
            }            
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(GetKey(_key));
        }
    }
}
