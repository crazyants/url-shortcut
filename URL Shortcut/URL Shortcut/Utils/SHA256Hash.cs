using System.Security.Cryptography;
using System.Text;

namespace URL_Shortcut.Utils
{
    public class SHA256Hash
    {
        public static string GetSHA256Hash(string str)
        {
            // Instantiate SHA256 algorithm
            SHA256 sha256 = SHA256.Create();

            // Calculate raw hash
            byte[] rawHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));

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
