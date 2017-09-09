using Cassandra;

namespace URL_Shortcut.Database
{
    public class QueryKeySpaceTime
    {
        ISession session;

        public QueryKeySpaceTime(ISession session)
        {
            this.session = session;
        }

        public bool GetKeySpaceTime(out long time)
        {
            var cql = "SELECT blobAsBigInt(created_time) AS created_time FROM tbl_times WHERE key = ? ;";
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind("url_shortcut");
            var rows = this.session.Execute(stmt);

            foreach (Row row in rows)
            {
                time = row.GetValue<long>("created_time");
                return true;
            }

            time = 0;

            return false;
        }
    }
}
