using Cassandra;

namespace URL_Shortcut.Database
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

            foreach (Row row in rows)
            {
                signature = row.GetValue<string>("signature");
                return true;
            }

            signature = string.Empty;

            return false;
        }
    }
}
