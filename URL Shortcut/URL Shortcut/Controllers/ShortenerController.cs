using Microsoft.AspNetCore.Mvc;

namespace URL_Shortcut.Controllers
{
    [Route("api/shortener")]
    public class ShortenerController : Controller
    {
        [Route("")]
        [HttpPost]
        public string Index([FromHeader]string url)
        {
            Utils.CassandraConnect cassandraConnect = new Utils.CassandraConnect();
            
            return url;
        }
    }
}
