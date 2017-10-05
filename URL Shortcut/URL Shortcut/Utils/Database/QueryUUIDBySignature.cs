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

        /// <summary>
        /// Get URL's UUID by its short form.
        /// </summary>
        /// <param name="signature">URL's short form</param>
        /// <param name="uuid">URL's TimeUUID to be returned</param>
        /// <returns>Returns true if operation was successful.</returns>
        public bool GetUUIDBySignature(string signature, out TimeUuid uuid)
        {
            var col = CassandraSchema.TABLE_SIGNATURES.UUID;
            var cql = string.Format("SELECT {0} FROM {1} WHERE {2} = ? ;",
                col,
                CassandraSchema.TABLE_SIGNATURES.TBL_SIGNATURES,
                CassandraSchema.TABLE_SIGNATURES.SIGNATURE);
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(signature);
            var rows = this.session.Execute(stmt);

            var row = CassandraHelper.GetFirstRow(rows);

            if (row == null)
            {
                return false;
            }

            uuid = row.GetValue<TimeUuid>(col);

            return true;
        }
    }
}
