// $Id$

namespace NSvn.Common
{
    /// <summary>
    /// Represents an SVN authentication provider
    /// </summary>
    public interface IAuthenticationProvider
    {
        ICredential FirstCredentials( );
        ICredential NextCredentials( );

        /// <summary>
        /// The kind of credential provided by this provider
        /// </summary>
        string Kind
        {
            get;
        }

        // TODO: implement
        //Credential SaveCredentials();
    }
}
