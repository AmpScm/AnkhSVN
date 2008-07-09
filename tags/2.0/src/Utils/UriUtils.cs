using System;
using System.Text.RegularExpressions;

namespace Utils
{
    /// <summary>
    /// A collection of utility methods for working on URIs
    /// </summary>
    public static class UriUtils
    {
         /// <summary>
        /// Splits an URL. The first component in the returned array will be the
        /// hostname, the remaining ones will be the path components split by '/'
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string[] Split( string url )
        {
            Match match = URLPARSE.Match(url);
            if ( !match.Success )
                throw new ApplicationException( "Not an URL: " + url );
            
            string host = match.Groups["host"].ToString();
            string rest = match.Groups["rest"].ToString();

            string[] restComponents = rest.Split( '/' );
            string[] components = new string[restComponents.Length+1];
            components[0] = host;
            restComponents.CopyTo( components, 1 );
            return components;
        }

        private static readonly Regex URLPARSE = 
            new Regex(@"(?'host'[^:]+://[^/]*)/(?'rest'.*)", 
            RegexOptions.IgnoreCase);
    }
}
