using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace SignalRChatAuthJWT.Auth
{
    internal static class AuthExtensions
    {
        internal static string GetSha1(this string content)
        {
            
            byte[] hash;
            
            using var sha1 = new SHA1Managed();

            hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(content));

            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        internal static string GetMd5Hash(this string content)
        {
            using MD5 md5Hash = MD5.Create();

            byte[] hash = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(content));
            string result = string.Concat(hash.Select(b => b.ToString("x2")));
            Debug.WriteLine($"{content} : {result}");
            return result;
        }

        public static bool VerifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(input);
            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
