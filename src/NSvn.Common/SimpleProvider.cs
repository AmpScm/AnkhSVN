// $Id$
using System;
using System.Collections;

namespace NSvn.Common
{
	/// <summary>
	/// Represents a simple provider that only has one set of credentials
	/// </summary>
	public class SimpleProvider : IAuthenticationProvider
	{
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="credential">The single credential provided by this provider</param>
        /// <param name="kind">The kind of credential this is</param>
		public SimpleProvider( ICredential credential )
		{
            this.credential = credential;
		}

        #region Implementation of IAuthenticationProvider
        public NSvn.Common.ICredential FirstCredentials( ICollection parameters )
        {
            return this.credential;
        }

        public NSvn.Common.ICredential NextCredentials( ICollection parameters )
        {
            return null;
        }

        public bool SaveCredentials( ICollection parameters)
        {
            return false;
        }

        public string Kind
        {
            get{ return credential.Kind;  }
        }
        #endregion

        private ICredential credential;
	}
}
