using Cassandra;
using URL_Shortcut.Utils.Database;

namespace URL_Shortcut.Models
{
    public class URLInsertion
    {
        ISession session;

        public URLInsertion(ISession session)
        {
            this.session = session;
        }

        public bool InsertURL(string url, string signature, string sha512, string sha256)
        {
            // Assign a UUID to the URL
            TimeUuid uuid = TimeUuid.NewId();

            // Insert the URL
            QueryInsertURL queryOne = new QueryInsertURL(this.session);
            queryOne.InsertURL(uuid, url, signature, sha512, sha256);

            // Hit the URL
            QueryHitURL queryTwo = new QueryHitURL(this.session);
            if (!queryTwo.HitURL(uuid, false))
            {
                return false;
            }

            return true;
        }
    }
}
