using Cassandra;

namespace URL_Shortcut.Utils.Database
{
    public class QueryURLByUUID
    {
        ISession session;

        public QueryURLByUUID(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Get long URL by its UUID.
        /// </summary>
        /// <param name="uuid">URL's TimeUUID</param>
        /// <param name="url">URL's long form to be returned</param>
        /// <returns>Returns true if operation was successful.</returns>
        public bool GetURLByUUID(TimeUuid uuid, out string url)
        {
            var cql = "SELECT url FROM tbl_urls WHERE uuid = ? ;";
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(uuid);
            var rows = this.session.Execute(stmt);

            var row = CassandraHelper.GetFirstRow(rows);

            if (row == null)
            {
                url = string.Empty;
                return false;
            }

            url = row.GetValue<string>("url");

            return true;
        }
    }
}
