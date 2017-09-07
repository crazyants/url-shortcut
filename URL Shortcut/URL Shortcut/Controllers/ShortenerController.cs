using Microsoft.AspNetCore.Mvc;
using URL_Shortcut.Utils;
using URL_Shortcut.Database;
using Cassandra;
using URL_Shortcut.Models;

namespace URL_Shortcut.Controllers
{
    [Route("api/shortener")]
    public class ShortenerController : Controller
    {
        [Route("")]
        [HttpPost]
        public string Index([FromHeader]string url)
        {
            // Get the URL's SHA512 & SHA256 hash
            string sha512 = SHA512Hash.GetSHA512Hash(url);
            string sha256 = SHA256Hash.GetSHA256Hash(url);

            // Open up a connection to Cassandra
            CassandraConnection csConnection = new CassandraConnection();

            // Get connection session
            ISession csSession = csConnection.GetSession();

            // Lookup database and return the URL's signature if it exists
            SignatureLookup signatureLookup = new SignatureLookup(csSession);
            if (signatureLookup.LookupSignature(sha512, sha256, out string signature))
            {
                return signature;
            }

            // Insert the new URL into the database

            return url;
        }
    }
}
