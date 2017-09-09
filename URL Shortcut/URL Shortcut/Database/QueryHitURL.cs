using Cassandra;

namespace URL_Shortcut.Database
{
    public class QueryHitURL
    {
        ISession session;

        public QueryHitURL(ISession session)
        {
            this.session = session;
        }

        public bool HitURL(TimeUuid uuid, bool check = true)
        {
            if (check)
            {
                QueryURLHitCount query = new QueryURLHitCount(this.session);
                if (!query.GetURLHitCount(uuid, out long hits))
                {
                    return false;
                }
            }

            var cql = "UPDATE tbl_hits SET hit = hit + 1 WHERE uuid = ? ;";
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
