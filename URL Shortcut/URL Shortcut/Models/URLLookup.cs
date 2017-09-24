using Cassandra;
using URL_Shortcut.Utils.Database;

namespace URL_Shortcut.Models
{
    public class URLLookup
    {
        private ISession session;

        public URLLookup(ISession session)
        {
            this.session = session;
        }

        public bool LookupURL(string signature, out string url)
        {
            // Get UUID by signature
            QueryUUIDBySignature queryOne = new QueryUUIDBySignature(session);
            if (!queryOne.GetUUIDBySignature(signature, out TimeUuid uuid))
            {
                url = string.Empty;
                return false;
            }

            // Get URL by UUID
            QueryURLByUUID queryTwo = new QueryURLByUUID(session);
            if (!queryTwo.GetURLByUUID(uuid, out url))
            {
                url = string.Empty;
                return false;
            }
            
            return true;
        }
    }
}
