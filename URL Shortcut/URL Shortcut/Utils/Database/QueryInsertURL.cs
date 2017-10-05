using Cassandra;

namespace URL_Shortcut.Utils.Database
{
    public class QueryInsertURL
    {
        ISession session;

        public QueryInsertURL(ISession session)
        {
            this.session = session;
        }

        /// <summary>
        /// Insert a new URL into database.
        /// </summary>
        /// <param name="uuid">URL's TimeUUID</param>
        /// <param name="url">Long URL</param>
        /// <param name="signature">Short URL</param>
        /// <param name="sha512">URL's SHA512</param>
        /// <param name="sha256">URL's SHA256</param>
        /// <returns>Returns true if operation was successful.</returns>
        public bool InsertURL(TimeUuid uuid, string url, string signature, string sha512, string sha256)
        {
            // Prepare CQL strings
            var cqlInsertURL = string.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (?, ?, ?, unixtimestampof(now())) ;",
                CassandraSchema.TABLE_URLS.TBL_URLS,
                CassandraSchema.TABLE_URLS.UUID,
                CassandraSchema.TABLE_URLS.URL,
                CassandraSchema.TABLE_URLS.SIGNATURE,
                CassandraSchema.TABLE_URLS.CREATED_ON);
            var cqlInsertHash = string.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES (?, ?, ?) ;",
                CassandraSchema.TABLE_HASHES.TBL_HASHES,
                CassandraSchema.TABLE_HASHES.SHA512,
                CassandraSchema.TABLE_HASHES.SHA256,
                CassandraSchema.TABLE_HASHES.UUID);
            var cqlInsertSignature = string.Format("INSERT INTO {0} ({1}, {2}) VALUES (?, ?) ;",
                CassandraSchema.TABLE_SIGNATURES.TBL_SIGNATURES,
                CassandraSchema.TABLE_SIGNATURES.SIGNATURE,
                CassandraSchema.TABLE_SIGNATURES.UUID);

            // Prepare the statements
            var stmtInsertURL = this.session.Prepare(cqlInsertURL);
            var stmtInsertHash = this.session.Prepare(cqlInsertHash);
            var stmtInsertSignature = this.session.Prepare(cqlInsertSignature);

            // Make a batch of statements
            BatchStatement batchStatement = new BatchStatement()
                .Add(stmtInsertURL.Bind(uuid, url, signature))
                .Add(stmtInsertHash.Bind(sha512, sha256, uuid))
                .Add(stmtInsertSignature.Bind(signature, uuid));

            // Execute the statements
            this.session.Execute(batchStatement);

            // Update the global counter
            var key = CassandraSchema.TABLE_COUNTERS._KEY_URLS;
            var cql = string.Format("UPDATE {0} SET {1} = {1} + 1 WHERE {2} = ? ;",
                CassandraSchema.TABLE_COUNTERS.TBL_COUNTERS,
                CassandraSchema.TABLE_COUNTERS.COUNTER,
                CassandraSchema.TABLE_COUNTERS.KEY);
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(key);
            this.session.Execute(stmt);
            
            return true;
        }
    }
}
