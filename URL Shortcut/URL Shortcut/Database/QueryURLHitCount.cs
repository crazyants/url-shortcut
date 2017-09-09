using Cassandra;

namespace URL_Shortcut.Database
{
    public class QueryURLHitCount
    {
        public const long NOT_FOUND = -1;
        ISession session;

        public QueryURLHitCount(ISession session)
        {
            this.session = session;
        }

        public bool GetURLHitCount(TimeUuid uuid, out long hits)
        {
            var cql = "SELECT hit FROM tbl_hits WHERE uuid = ? ;";
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(uuid);
            var rows = this.session.Execute(stmt);

            foreach (Row row in rows)
            {
                hits = row.GetValue<long>("hit");
                return true;
            }

            hits = NOT_FOUND;

            return false;
        }
    }
}
