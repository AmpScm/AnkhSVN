using System;
using System.Text.RegularExpressions;

namespace Utils
{
    /// <summary>
    /// A collection of utility methods for working on URIs
    /// </summary>
    public class UriUtils
    {
        private UriUtils()
        {
            // nothing here
        }        
        
        /// <summary>
        /// Concatenates two URI segments, placing a / in between as appropriate.
        /// </summary>
        /// <param name="uri1"></param>
        /// <param name="uri2"></param>
        /// <returns></returns>
        public static string Combine( string uri1, string uri2 )
        {            
            bool uri1HasSlash = uri1.EndsWith("/");
            bool uri2HasSlash = uri2.StartsWith( "/" );

            if ( uri1HasSlash && uri2HasSlash )
                return uri1.Substring(0, uri1.Length-1) + uri2;
            else if ( uri1HasSlash ^ uri2HasSlash )
                return uri1 + uri2;
            else
                return uri1 + "/" + uri2;
        }

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

        public static bool IsValidUrl(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            Uri uri;

            return Uri.TryCreate(value, UriKind.Absolute, out uri);
        }

        private static readonly Regex URLPARSE = 
            new Regex(@"(?'host'[^:]+://[^/]*)/(?'rest'.*)", 
            RegexOptions.IgnoreCase);
    }
}
