
namespace NSvn.Common
{
    /// <summary>
    /// Represents an SVN authentication provider
    /// </summary>
    public interface IAuthenticationProvider
    {
        Credential FirstCredentials( AuthenticationBaton baton );
        Credential NextCredentials( AuthenticationBaton baton );
        Credential SaveCredentials();
    }
}