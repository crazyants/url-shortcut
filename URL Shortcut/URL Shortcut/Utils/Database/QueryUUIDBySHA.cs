using Cassandra;

namespace URL_Shortcut.Utils.Database
{
    public class QueryUUIDBySHA
    {
        ISession session;

        public QueryUUIDBySHA(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Get URL's UUID by its SHA hash.
        /// </summary>
        /// <param name="sha512">URL's SHA512</param>
        /// <param name="sha256">URL's SHA256</param>
        /// <param name="uuid">URL's UUID to be returned</param>
        /// <returns>Returns true if operation was successful.</returns>
        public bool GetUUIDBySHA(string sha512, string sha256, out TimeUuid uuid)
        {
            var cql = "SELECT uuid FROM tbl_hashes WHERE sha512 = ? AND sha256 = ? ;";
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(sha512, sha256);
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
