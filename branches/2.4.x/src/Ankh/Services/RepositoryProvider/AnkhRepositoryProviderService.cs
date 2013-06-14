using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using Ankh.ExtensionPoints.RepositoryProvider;
using Ankh.UI;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace Ankh.Services.RepositoryProvider
{
    [GlobalService(typeof(IAnkhRepositoryProviderService))]
    sealed class AnkhRepositoryProviderService : AnkhService, IAnkhRepositoryProviderService
    {
        readonly Dictionary<string, ScmRepositoryProvider> _nameProviderMap;

        public AnkhRepositoryProviderService(IAnkhServiceProvider context)
            : base(context)
        {
            _nameProviderMap = new Dictionary<string, ScmRepositoryProvider>();
        }

        #region IAnkhRepositoryProviderService Members

        /// <summary>
        /// Gets all registered SCM repository providers
        /// </summary>
        public ICollection<ScmRepositoryProvider> RepositoryProviders
        {
            get
            {
                if (_nameProviderMap != null)
                {
                    ScmRepositoryProvider[] result = new ScmRepositoryProvider[_nameProviderMap.Count];
                    _nameProviderMap.Values.CopyTo(result, 0);
                    return result;
                }
                return new ScmRepositoryProvider[] { };
            }
        }

        /// <summary>
        /// Gets all the registered SCM repository providers for the given SCM type(svn, git).
        /// </summary>
        /// <param name="type">SCM type</param>
        /// <remarks>This call DOES NOT trigger provider package initialization.</remarks>
        public ICollection<ScmRepositoryProvider> GetRepositoryProviders(RepositoryType type)
        {
            ICollection<ScmRepositoryProvider> allProviders = RepositoryProviders;
            List<ScmRepositoryProvider> result = new List<ScmRepositoryProvider>();
            foreach (ScmRepositoryProvider provider in allProviders)
            {
                if (type == RepositoryType.Any || type ==provider.Type)
                {
                    result.Add(provider);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Tries to find a registered provider with the given name.
        /// </summary>
        /// <param name="id">Repository provider's identifier</param>
        /// <param name="repoProvider">[out] Repository provider instance if found</param>
        /// <remarks>This call DOES NOT trigger provider package initialization.</remarks>
        /// <returns>true if the lookup is successful, false otherwise</returns>
        public bool TryGetRepositoryProvider(string id, out ScmRepositoryProvider repoProvider)
        {
            repoProvider = null;
            if (_nameProviderMap != null && _nameProviderMap.Count > 0)
            {
                return _nameProviderMap.TryGetValue(id, out repoProvider);
            }
            return false;
        }

        // TODO
        public void ShowProvideHelp()
        {
            // Shamelessly copied from the AnkhHelService

            UriBuilder ub = new UriBuilder("http://svc.ankhsvn.net/svc/go/");
            ub.Query = string.Format("t=ctrlHelp&v={0}&l={1}&dt={2}", GetService<IAnkhPackage>().UIVersion, CultureInfo.CurrentUICulture.LCID, Uri.EscapeUriString("Ankh.UI.ScmRepositoryProviders.HowTo"));

            try
            {
                bool showHelpInBrowser = true;
                IVsHelpSystem help = GetService<IVsHelpSystem>(typeof(SVsHelpService));
                if (help != null)
                    showHelpInBrowser = !VSErr.Succeeded(help.DisplayTopicFromURL(ub.Uri.AbsoluteUri, (uint)VHS_COMMAND.VHS_Default));

                if (showHelpInBrowser)
                    Help.ShowHelp(null, ub.Uri.AbsoluteUri);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        #endregion

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ReadProviderRegistry();
        }

        /// <summary>
        /// Reads the SCM repository provider information from the registry
        /// </summary>
        private void ReadProviderRegistry()
        {
            IAnkhPackage ankhPackage = GetService<IAnkhPackage>();
            if (ankhPackage != null)
            {
                using (RegistryKey key = ankhPackage.ApplicationRegistryRoot)
                {
                    using (RegistryKey aKey = key.OpenSubKey("ScmRepositoryProviders"))
                    {
                        if (aKey == null)
                            return;

                        string[] providerKeys = aKey.GetSubKeyNames();
                        foreach (string providerKey in providerKeys)
                        {
                            using (RegistryKey provider = aKey.OpenSubKey(providerKey))
                            {
                                string serviceName = (string)provider.GetValue("");
                                RepositoryType rt = GetRepositoryType(provider.GetValue("ScmType") as string);
                                ScmRepositoryProvider descriptor = new ScmRepositoryProviderProxy(this, providerKey, serviceName, rt);
                                if (!_nameProviderMap.ContainsKey(providerKey))
                                {
                                    _nameProviderMap.Add(providerKey, descriptor);
                                }
                            }
                        }
                    }
                }
            }
        }

        private RepositoryType GetRepositoryType(string typeString)
        {
            if (string.Equals(typeString, "svn", StringComparison.OrdinalIgnoreCase))
                return RepositoryType.Subversion;
            if (string.Equals(typeString, "git", StringComparison.OrdinalIgnoreCase))
                return RepositoryType.Git;

            return RepositoryType.Any;
        }
    }
}
