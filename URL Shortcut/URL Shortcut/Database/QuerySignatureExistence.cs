using Cassandra;

namespace URL_Shortcut.Database
{
    public class QuerySignatureExistence
    {
        ISession session;

        public QuerySignatureExistence(ISession session)
        {
            this.session = session;
        }

        public bool SignatureExists(string signature)
        {
            var cql = "SELECT COUNT(*) AS count FROM tbl_signatures WHERE signature = ? ;";
            var prep = this.session.Prepare(cql);
            var stmt = prep.Bind(signature);
            var rows = this.session.Execute(stmt);

            foreach (Row row in rows)
            {
                byte n = (byte)row.GetValue<long>("count");
                if (n>0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
