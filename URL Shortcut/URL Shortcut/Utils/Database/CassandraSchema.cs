namespace URL_Shortcut.Utils.Database
{
    internal static class CassandraSchema
    {
        internal struct TABLE_COUNTERS
        {
            internal const string TBL_COUNTERS = "tbl_counters";
            internal const string KEY = "key";
            internal const string COUNTER = "counter";
            internal const string _KEY_URLS = "urls";
        }

        internal struct TABLE_URLS
        {
            internal const string TBL_URLS = "tbl_urls";
            internal const string UUID = "uuid";
            internal const string URL = "url";
            internal const string SIGNATURE = "signature";
            internal const string CREATED_ON = "created_on";
        }

        internal struct TABLE_HASHES
        {
            internal const string TBL_HASHES = "tbl_hashes";
            internal const string SHA512 = "sha512";
            internal const string SHA256 = "sha256";
            internal const string UUID = "uuid";
        }

        internal struct TABLE_SIGNATURES
        {
            internal const string TBL_SIGNATURES = "tbl_signatures";
            internal const string SIGNATURE = "signature";
            internal const string UUID = "uuid";
        }

        internal struct TABLE_HITS
        {
            internal const string TBL_HITS = "tbl_hits";
            internal const string UUID = "uuid";
            internal const string HIT = "hit";
        }
    }
}
