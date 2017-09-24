namespace URL_Shortcut.Models.POCOs
{
    public class APIResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public string URL { get; set; }
        public string Shortcut { get; set; }
        public long Popularity { get; set; }
    }
}
