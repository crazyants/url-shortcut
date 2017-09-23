using Cassandra;
using URL_Shortcut.Utils;

namespace URL_Shortcut.Utils.Database
{
    public class QuerySignatureByUUID
    {
        ISession session;

        public QuerySignatureByUUID(ISession session)
        {
            this.session = session;
        }

        public bool GetSignatureByUUID(TimeUuid uuid, out string signature)
        {
            var cql = "SELECT signature FROM tbl_urls WHERE uuid = ? ;";
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(uuid);
            var rows = this.session.Execute(stmt);

            var row = CassandraHelper.GetFirstRow(rows);

            if (row == null)
            {
                signature = string.Empty;
                return false;
            }

            signature = row.GetValue<string>("signature");

            return true;
        }
    }
}
