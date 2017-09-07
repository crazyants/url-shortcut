using Cassandra;

namespace URL_Shortcut.Database
{
    public class QueryUUIDBySHA
    {
        ISession session;

        public QueryUUIDBySHA(ISession session)
        {
            this.session = session;
        }

        public bool GetUUIDBySHA(string sha512, string sha256, out TimeUuid uuid)
        {
            var cql = "SELECT uuid FROM tbl_hashes WHERE sha512 = ? AND sha256 = ? ;";
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(sha512, sha256);
            var rows = this.session.Execute(stmt);

            foreach (Row row in rows)
            {
                uuid = row.GetValue<TimeUuid>("uuid");
                return true;
            }

            return false;
        }
    }
}
