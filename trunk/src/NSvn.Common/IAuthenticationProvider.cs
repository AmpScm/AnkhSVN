// $Id$

namespace NSvn.Common
{
    /// <summary>
    /// Represents an SVN authentication provider
    /// </summary>
    public interface IAuthenticationProvider
    {
        Credential FirstCredentials( );
        Credential NextCredentials( );

        // TODO: implement
        //Credential SaveCredentials();
    }
}
