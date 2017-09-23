using Cassandra;
using URL_Shortcut.Utils.Database;

namespace URL_Shortcut.Models
{
    public class SignatureLookup
    {
        private ISession session;

        public SignatureLookup(ISession session)
        {
            this.session = session;
        }

        public bool LookupSignature(string sha512, string sha256, out string signature, out long hits)
        {
            // See if the URL exists in database
            QueryUUIDBySHA queryOne = new QueryUUIDBySHA(session);
            if (!queryOne.GetUUIDBySHA(sha512, sha256, out TimeUuid uuid))
            {
                signature = string.Empty;
                hits = -1;
                return false;
            }

            // Get the signature of the existing URL
            QuerySignatureByUUID queryTwo = new QuerySignatureByUUID(session);
            if (!queryTwo.GetSignatureByUUID(uuid, out signature))
            {
                hits = -1;
                return false;
            }

            // Hit the URL
            QueryHitURL queryThree = new QueryHitURL(this.session);
            if (!queryThree.HitURL(uuid))
            {
                hits = -1;
                return false;
            }

            // Get URL's popularity
            QueryURLHitCount queryFour = new QueryURLHitCount(this.session);
            if (!queryFour.GetURLHitCount(uuid, out hits))
            {
                return false;
            }

            return true;
        }
    }
}
