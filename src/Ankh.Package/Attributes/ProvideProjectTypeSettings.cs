using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using Ankh.Scc.ProjectMap;

namespace Ankh.VSPackage.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	sealed class ProvideProjectTypeSettingsAttribute : RegistrationAttribute
	{
		readonly Guid _projectType;
		SccProjectFlags _flags;

		public ProvideProjectTypeSettingsAttribute(string projectType, SccProjectFlags flags)
		{
			_projectType = new Guid(projectType);
			_flags = flags;
		}

		public Guid ProjectType
		{
			get { return _projectType; }
		}

		string KeyName
		{
			get { return string.Format("Extensions\\AnkhSVN\\ProjectHandling\\{0}", ProjectType.ToString("B")); }
		}

		public override void Register(RegistrationAttribute.RegistrationContext context)
		{
			using (Key key = context.CreateKey(KeyName))
			{
				key.SetValue("flags", (int)_flags);
			}
		}

		public override void Unregister(RegistrationAttribute.RegistrationContext context)
		{
			context.RemoveKey(KeyName);
		}
	}
}
