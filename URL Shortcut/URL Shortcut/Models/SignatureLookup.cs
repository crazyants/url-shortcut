using Cassandra;
using URL_Shortcut.Database;

namespace URL_Shortcut.Models
{
    public class SignatureLookup
    {
        private ISession session;

        public SignatureLookup(ISession session)
        {
            this.session = session;
        }

        public bool LookupSignature(string sha512, string sha256, out string signature)
        {
            // See if the URL exists in database
            QueryUUIDBySHA queryOne = new QueryUUIDBySHA(session);
            if (!queryOne.GetUUIDBySHA(sha512, sha256, out TimeUuid uuid))
            {
                signature = string.Empty;
                return false;
            }

            // Return the signature of the existing URL
            QuerySignatureByUUID queryTwo = new QuerySignatureByUUID(session);
            if (!queryTwo.GetSignatureByUUID(uuid, out signature))
            {
                return false;
            }

            return true;
        }
    }
}
