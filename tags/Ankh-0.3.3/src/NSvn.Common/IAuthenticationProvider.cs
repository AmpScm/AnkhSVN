// $Id$

namespace NSvn.Common
{
    /// <summary>
    /// Represents an SVN authentication provider
    /// </summary>
    public interface IAuthenticationProvider
    {
        /// <summary>
        /// Retrieve the first set of credentials.
        /// </summary>
        /// <param name="realm">The realm for which to authenticate.</param>
        /// <param name="parameters">Contains parameters that are passed among the 
        /// providers.</param>
        ICredential FirstCredentials( string realm, System.Collections.ICollection parameters );

        /// <summary>
        /// Retrieve the next set of credentials.
        /// </summary>
        /// <param name="parameters">Contains parameters that are passed among the 
        /// providers.</param>
        ICredential NextCredentials( System.Collections.ICollection parameters );

        /// <summary>
        /// The kind of credential provided by this provider
        /// </summary>
        string Kind
        {
            get;
        }

        /// <summary>
        /// Save the last used credentials.
        /// </summary>
        bool SaveCredentials( System.Collections.ICollection parameters );
    }
}
