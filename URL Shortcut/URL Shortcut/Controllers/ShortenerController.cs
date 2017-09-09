using Microsoft.AspNetCore.Mvc;
using URL_Shortcut.Models.POCOs;
using URL_Shortcut.Utils;
using URL_Shortcut.Database;
using Cassandra;
using URL_Shortcut.Models;
using System;

namespace URL_Shortcut.Controllers
{
    [Route("api/shortener")]
    public class ShortenerController : Controller
    {
        [Route("")]
        [HttpPost]
        public object Index([FromHeader]string url)
        {
            // Create object to return
            APIResult result = new APIResult()
            {
                Status = -1,
                URL = url,
                Signature = string.Empty
            };

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
                result.Signature = signature;
                result.Status = 0;
                return Json(result);
            }

            // Get time->now in millisecond
            long time = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            // Get the time when keyspace initiated
            QueryKeySpaceTime query1 = new QueryKeySpaceTime(csSession);
            if (!query1.GetKeySpaceTime(out long initTime))
            {
                return Json(result);
            }

            // Prepare dictionary
            char[] dictionary =
            {
                'A', 'B', 'C', 'D', 'E', 'F',
                'G', 'H', 'I', 'J', 'K', 'L',
                'M', 'N', 'O', 'P', 'Q', 'R',
                'S', 'T', 'U', 'V', 'W', 'X',
                'Y', 'Z',

                'a', 'b', 'c', 'd', 'e', 'f',
                'g', 'h', 'i', 'j', 'k', 'l',
                'm', 'n', 'o', 'p', 'q', 'r',
                's', 't', 'u', 'v', 'w', 'x',
                'y', 'z',

                '0', '1', '2', '3', '4', '5',
                '6', '7', '8', '9'
            };

            // Get a unique ID to sign
            Random rnd = new Random();
            string sign = string.Empty;
            ulong id = UniqueID.FAILURE;
            while (id == UniqueID.FAILURE)
            {
                // Get ID
                id = UniqueID.GetUniqueID(initTime, time, rnd);

                // Get signature
                sign = BaseN.ChangeBase(id, dictionary);

                // Check for signature uniqueness
                QuerySignatureExistence query2 = new QuerySignatureExistence(csSession);
                if (query2.SignatureExists(sign))
                {
                    id = UniqueID.FAILURE;
                }
            }

            // Unique signature is now set
            result.Signature = sign;

            // Insert the new URL into the database
            URLInsertion urlInsertion = new URLInsertion(csSession);
            if (urlInsertion.InsertURL(url, sign, sha512, sha256))
            {
                result.Status = 0;
            }

            return Json(result);
        }
    }
}
