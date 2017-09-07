using System.Security.Cryptography;
using System.Text;

namespace URL_Shortcut.Utils
{
    public class SHA512Hash
    {
        public static string GetSHA512Hash(string str)
        {
            // Instantiate SHA512 algorithm
            SHA512 sha512 = SHA512.Create();

            // Calculate raw hash
            byte[] rawHash = sha512.ComputeHash(Encoding.UTF8.GetBytes(str));

            // Convert the raw byte hash into string
            StringBuilder hash = new StringBuilder();
            for (int i = 0; i < rawHash.Length; ++i)
            {
                hash.Append(rawHash[i].ToString("X2"));
            }

            // Return hash string
            return hash.ToString();
        }
    }
}
