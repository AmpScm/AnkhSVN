using System;

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
    }
}
