using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Utils
{
    public class HashUtils
    {

        public static byte[] GetMd5HashForFile( string file )
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            using ( FileStream fs = new FileStream( file, FileMode.Open, FileAccess.Read ) )
            {
                return md5.ComputeHash( fs );
            }
        }

        

        public static bool HashesAreEqual( byte[] hash1, byte[] hash2 )
        {
            if ( hash1.Length != hash2.Length )
            {
                return false;
            }

            for ( int i = 0; i < hash1.Length; i++ )
            {
                if ( hash1[ i ] != hash2[ i ] )
                {
                    return false;
                }
            }

            return true;
        }

        public static bool FileMatchesMd5Hash( byte[] hash, string file )
        {
            byte[] fileHash = HashUtils.GetMd5HashForFile( file );
            return HashesAreEqual( hash, fileHash );
        }

        public static byte[] GetSha1HashForFile( string path )
        {
            SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider();

            using ( FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read ))
            {
                return provider.ComputeHash(fs);
            }
        }

        public static bool FileMatchesSha1Hash( byte[] hash, string path )
        {
            byte[] fileHash = HashUtils.GetSha1HashForFile( path );
            return HashesAreEqual( hash, fileHash );
        }
    }
}
