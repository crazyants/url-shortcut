namespace URL_Shortcut.Models.POCOs
{
    public class APIResult
    {
        public int Status { get; set; }
        public string URL { get; set; }
        public string Signature { get; set; }
        public long Popularity { get; set; }
    }
}
