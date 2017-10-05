using Cassandra;

namespace URL_Shortcut.Utils.Database
{
    public class QueryHitURL
    {
        ISession session;

        public QueryHitURL(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Increase the URL hit counter by one. URL is identified by a UUID.
        /// </summary>
        /// <param name="uuid">URL's TimeUUID</param>
        /// <param name="check">Check if UUID exists before incrementing the counter.</param>
        /// <returns>Returns true if operation was successful.</returns>
        public bool HitURL(TimeUuid uuid, bool check = true)
        {
            // Whether to check if entity exists or not
            if (check)
            {
                QueryURLHitCount query = new QueryURLHitCount(this.session);
                if (!query.GetURLHitCount(uuid, out long hits))
                {
                    // Return false if entity does not exists
                    return false;
                }
            }

            // Increase the URL popularity by one
            var cql = string.Format("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = ? ;",
                CassandraSchema.TABLE_HITS.TBL_HITS,
                CassandraSchema.TABLE_HITS.HIT,
                CassandraSchema.TABLE_HITS.UUID);

            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(uuid);
            var rows = this.session.Execute(stmt);

            if (rows == null)
            {
                return false;
            }

            return true;
        }
    }
}
