// $Id$
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
            this.providers = new AuthenticationProviderCollection();
		}

        /// <summary>
        /// The authentication providers associated with this authentication 
        /// baton.
        /// </summary>
        public AuthenticationProviderCollection Providers
        {
            get{ return this.providers; }
            set{ this.providers = value; }
        }

        /// <summary>
        /// A set of parameters accessible by all providers.
        /// </summary>
        public StringDictionary Parameters
        {
            get{ return this.parameters; }
        }



        private AuthenticationProviderCollection providers;
        private StringDictionary parameters;
	}
}

