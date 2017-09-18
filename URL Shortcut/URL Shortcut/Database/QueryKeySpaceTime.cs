using Cassandra;
using URL_Shortcut.Utils;

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

            var row = Helper.GetFirstRow(rows);

            if (row == null)
            {
                time = 0;
                return false;
            }

            time = row.GetValue<long>("created_time");

            return true;
        }
    }
}
