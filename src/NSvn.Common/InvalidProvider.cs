using System;

namespace NSvn.Common
{
	/// <summary>
	/// Represents an IAuthenticationProvider that always returns Credential::Invalid
	/// </summary>
	public class InvalidProvider : IAuthenticationProvider
	{
        /// <summary>
        /// Ctor made private to avoid external instantiation
        /// </summary>
		private InvalidProvider()
		{
		}

        /// <summary>
        /// The sole instance of this class
        /// </summary>
        public static readonly InvalidProvider Instance = new InvalidProvider();

        #region Implementation of IAuthenticationProvider
        public NSvn.Common.Credential FirstCredentials()
        {
            return Credential.Invalid;
        }

        public NSvn.Common.Credential NextCredentials()
        {
            return Credential.Invalid;
        }
        #endregion
	}
}
