using System;
using System.Collections.Specialized;

namespace NSvn.Common
{
	/// <summary>
	/// Represents a set of authentication providers and their parameters
	/// </summary>
	public class AuthenticationBaton
	{
		public AuthenticationBaton()
		{
            this.parameters = new StringDictionary();
            this.providers = new IAuthenticationProviderCollection();
		}

        /// <summary>
        /// The authentication providers associated with this authentication 
        /// baton
        /// </summary>
        public IAuthenticationProviderCollection Providers
        {
            get{ return this.providers; }
        }

        /// <summary>
        /// A set of parameters accessible by all providers
        /// </summary>
        public StringDictionary Parameters
        {
            get{ return this.parameters; }
        }



        private IAuthenticationProviderCollection providers;
        private StringDictionary parameters;
	}
}
