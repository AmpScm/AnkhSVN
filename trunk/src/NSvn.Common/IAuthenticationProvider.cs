
namespace NSvn.Common
{
    /// <summary>
    /// Represents an SVN authentication provider
    /// </summary>
    public interface IAuthenticationProvider
    {
        Credential FirstCredentials( );
        Credential NextCredentials( );
        Credential SaveCredentials();
    }
}