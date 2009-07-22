using System;
using Microsoft.VisualStudio.Shell;
using System.Globalization;

namespace Ankh.Interop.IssueTracker
{
    /// <summary>
    /// This attribute registers the package as Issue Repository Connector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [System.Runtime.InteropServices.Guid("072C0B48-7BCC-49ce-927C-1EC92279E8CC")]
    public sealed class ProvideIssueRepositoryConnector : RegistrationAttribute
    {
        private const string REG_KEY_CONNECTORS = "IssueRepositoryConnectors";
        private const string REG_KEY_NAME = "Name";
        private const string REG_VALUE_SERVICE = "Service";
        private const string REG_VALUE_PACKAGE = "Package";

        private Type _connectorService = null;
        private string _regName = null;
        private string _uiName = null;
        private Type _uiNamePkg = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectorServiceType">Type of the connector service</param>
        /// <param name="regName">Connector name in the registry</param>
        /// <param name="uiNamePkg">Unique identifier (Guid) of the package that proffers connector service</param>
        /// <param name="uiName">String resource id that represents ui name of the connector</param>
        public ProvideIssueRepositoryConnector(Type connectorServiceType, string regName, Type uiNamePkg, string uiName)
        {
            System.Diagnostics.Debug.Assert(typeof(IIssueRepositoryConnector).IsAssignableFrom(connectorServiceType), "Issue Repository Connector must implement IIssueRepositoryConnector interface");
            _connectorService = connectorServiceType;
            _regName = regName;
            _uiNamePkg = uiNamePkg;
            _uiName = uiName;
        }

        /// <summary>
        /// Gets Issue repository connector service's global identifier.
        /// </summary>
        public Guid IssueRepositoryConnectorService
        {
            get
            {
                return _connectorService.GUID;
            }
        }

        /// <summary>
        /// Gets the name of the issue repository connector (used in registry)
        /// </summary>
        public string RegName
        {
            get
            {
                return _regName;
            }
        }

        /// <summary>
        /// Gets the global identifier used to register the issue repository connector
        /// </summary>
        public Guid RegGuid
        {
            get
            {
                return IssueRepositoryConnectorService;
            }
        }

        /// <summary>
        /// Gets the package identifier the proffers the connector service.
        /// </summary>
        public Guid UINamePkg
        {
            get
            {
                return _uiNamePkg.GUID;
            }
        }

        /// <summary>
        /// Gets the string resource identifier that represents the UI name of the issue tracker repository connector.
        /// </summary>
        public string UIName
        {
            get
            {
                return _uiName;
            }
        }

        public override void Register(RegistrationAttribute.RegistrationContext context)
        {
            // Write to the context's log what we are about to do
            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture,
                "Issue Repository Connector:\t\t{0}\n", RegName));

            // Declare the issue repository connector, its name, the provider's service 
            using (Key connectors = context.CreateKey(REG_KEY_CONNECTORS))
            {
                using (Key connectorKey = connectors.CreateSubkey(RegGuid.ToString("B").ToUpperInvariant()))
                {
                    connectorKey.SetValue("", RegName);
                    connectorKey.SetValue(REG_VALUE_SERVICE, IssueRepositoryConnectorService.ToString("B").ToUpperInvariant());

                    using (Key connectorNameKey = connectorKey.CreateSubkey(REG_KEY_NAME))
                    {
                        connectorNameKey.SetValue("", UIName);
                        connectorNameKey.SetValue(REG_VALUE_PACKAGE, UINamePkg.ToString("B").ToUpperInvariant());

                        connectorNameKey.Close();
                    }
                    connectorKey.Close();
                }
                connectors.Close();
            }
        }

        public override void Unregister(RegistrationAttribute.RegistrationContext context)
        {
            context.RemoveKey(REG_KEY_CONNECTORS +"\\" + RegGuid.ToString("B"));
        }
    }
}
