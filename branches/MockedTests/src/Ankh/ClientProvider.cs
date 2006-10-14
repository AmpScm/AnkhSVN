using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh
{
    [Service(typeof(IClientProvider))]
    sealed class ClientProvider : IClientProvider
    {
        public ClientProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        IConfigurationProvider ConfigProvider
        {
            get { return (IConfigurationProvider)this.serviceProvider.GetService(typeof(IConfigurationProvider)); }
        }

        public NSvn.Core.Client Client
        {
            get 
            {
                if (this.client == null)
                {
                    // should we use a custom configuration directory?
                    string configDir = this.ConfigProvider.Configuration.Subversion.ConfigDir;
                    if (configDir != null)
                        this.client = new SvnClient(this.serviceProvider,
                            Environment.ExpandEnvironmentVariables(configDir));
                    else
                        this.client = new SvnClient(this.serviceProvider);
                }
                return this.client;
            }
        }

        private NSvn.Core.Client client;
        private IServiceProvider serviceProvider;
    }
}
