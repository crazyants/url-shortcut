using Cassandra;

namespace URL_Shortcut.Utils.Database
{
    public class QueryUUIDBySignature
    {
        ISession session;

        public QueryUUIDBySignature(ISession session)
        {
            this.session = session;
        }

        public bool GetUUIDBySignature(string signature, out TimeUuid uuid)
        {
            var cql = "SELECT uuid FROM tbl_signatures WHERE signature = ? ;";
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(signature);
            var rows = this.session.Execute(stmt);

            var row = CassandraHelper.GetFirstRow(rows);

            if (row == null)
            {
                return false;
            }

            uuid = row.GetValue<TimeUuid>("uuid");

            return true;
        }
    }
}
