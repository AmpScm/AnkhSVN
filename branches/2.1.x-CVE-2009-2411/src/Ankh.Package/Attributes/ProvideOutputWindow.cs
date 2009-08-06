using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell;
using System.Globalization;

namespace Ankh.VSPackage.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	internal sealed class ProvideOutputWindowAttribute : RegistrationAttribute
	{
		string _outputWindowId;
		string _resourceId;
		string _name;
		bool _initiallyInvisible;
		bool _clearWithSolution;

		public ProvideOutputWindowAttribute(string outputWindowId, string resourceId)
        {
            _outputWindowId = outputWindowId;
			_resourceId = resourceId;
        }

        public override void Register(RegistrationContext context)
        {
            Key childKey = null;

            try
            {
                childKey = context.CreateKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", "OutputWindow", OutputWindowId.ToString("B").ToUpperInvariant()));

                childKey.SetValue("", ResourceId);
				childKey.SetValue("Package", context.ComponentType.GUID.ToString("B").ToUpperInvariant());
				if (!string.IsNullOrEmpty(Name))
					childKey.SetValue("Name", Name);

				childKey.SetValue("InitiallyInvisible", InitiallyInvisible ? 1 : 0);
				childKey.SetValue("ClearWithSolution", ClearWithSolution ? 1 : 0);
            }
            finally
            {
                if (childKey != null) childKey.Close();
            }
        }

        public override void Unregister(RegistrationContext context)
        {
			context.RemoveKey(string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", "OutputWindow", OutputWindowId.ToString("B").ToUpperInvariant()));
        }

		public Guid OutputWindowId
		{
			get { return new Guid(_outputWindowId); }
		}

		public string ResourceId
		{
			get { return _resourceId; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public bool InitiallyInvisible
		{
			get { return _initiallyInvisible; }
			set { _initiallyInvisible = value; }
		}

		public bool ClearWithSolution
		{
			get { return _clearWithSolution; }
			set { _clearWithSolution = value; }
		}
	}
}
