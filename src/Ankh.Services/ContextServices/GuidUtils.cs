using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.ContextServices
{
    public static class GuidUtils
    {
        /// <summary>
        /// Generates a unique uuid from the specified hash value with the algorithm specified by RFC 4122
        /// </summary>
        /// <param name="baseUuid">The base UUID.</param>
        /// <param name="hashData">The hash data.</param>
        /// <returns>The guid</returns>
        public static Guid CreateGuid(Guid baseUuid, byte[] hashData)
        {
            if (baseUuid == Guid.Empty)
                throw new ArgumentNullException("baseUuid");
            else if (hashData == null)
                throw new ArgumentNullException("hashData");

            // See RFC 4122 for C implementation examples

            byte[] hash;
            using (SHA1 sha1 = SHA1.Create())
            {
                sha1.TransformBlock(GuidToNetworkOrder(baseUuid.ToByteArray()), 0, 16, null, 0);
                sha1.TransformFinalBlock(hashData, 0, hashData.Length);

                hash = sha1.Hash;
            }

            hash = ToHostOrder(hash); // Treat as guid

            Int32 timeLow = BitConverter.ToInt32(hash, 0);
            Int16 timeMid = BitConverter.ToInt16(hash, 4);
            Int16 timeHiAndVersion = BitConverter.ToInt16(hash, 6);
            Byte clockSeqHi = hash[8];
            Byte clockSeqLow = hash[9];

            return new Guid(
                timeLow,
                timeMid,
                (short)((timeHiAndVersion & 0xFFF) | (5 << 12)),
                (byte)((clockSeqHi & 0x3F) | 0x80),
                clockSeqLow,
                hash[10],
                hash[11],
                hash[12],
                hash[13],
                hash[14],
                hash[15]);
        }

        /// <summary>
        /// Converts the guid bytes to network order
        /// </summary>
        /// <param name="a">A.</param>
        /// <returns></returns>
        static byte[] GuidToNetworkOrder(byte[] a)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(a, 0, 4);
                Array.Reverse(a, 4, 2);
                Array.Reverse(a, 6, 2);
            }

            return a;
        }

        static byte[] ToHostOrder(byte[] a)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(a, 0, 4);
                Array.Reverse(a, 4, 2);
                Array.Reverse(a, 6, 2);
            }
            return a;
        }
    }
}
