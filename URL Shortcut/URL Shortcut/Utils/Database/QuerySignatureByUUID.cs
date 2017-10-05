using Cassandra;

namespace URL_Shortcut.Utils.Database
{
    public class QuerySignatureByUUID
    {
        ISession session;

        public QuerySignatureByUUID(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Get URL's UUID by its short form.
        /// </summary>
        /// <param name="uuid">URL's TimeUUID</param>
        /// <param name="signature">URL's short form to be returned</param>
        /// <returns>Returns true if operation was successful.</returns>
        public bool GetSignatureByUUID(TimeUuid uuid, out string signature)
        {
            var col = CassandraSchema.TABLE_URLS.SIGNATURE;
            var cql = string.Format("SELECT {0} FROM {1} WHERE {2} = ? ;",
                col,
                CassandraSchema.TABLE_URLS.TBL_URLS,
                CassandraSchema.TABLE_URLS.UUID);
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(uuid);
            var rows = this.session.Execute(stmt);

            var row = CassandraHelper.GetFirstRow(rows);

            if (row == null)
            {
                signature = string.Empty;
                return false;
            }

            signature = row.GetValue<string>(col);

            return true;
        }
    }
}
