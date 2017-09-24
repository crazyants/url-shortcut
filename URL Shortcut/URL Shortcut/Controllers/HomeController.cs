using Microsoft.AspNetCore.Mvc;
using URL_Shortcut.Utils.Database;
using Cassandra;
using URL_Shortcut.Models;
using URL_Shortcut.Models.POCOs;

namespace URL_Shortcut.Controllers
{
    [Route("")]
    [Route("Home")]
    public class HomeController : Controller
    {
        [Route("")]
        [Route("Index/{k}")]
        public IActionResult Index([FromQuery] string k)
        {
            QueryResult queryResult = new QueryResult()
            {
                URL = null
            };

            if (k == null || k == string.Empty)
            {
                return View(queryResult);
            }

            CassandraConnection cassandraConnection = new CassandraConnection();
            ISession csSession = cassandraConnection.GetSession();

            URLLookup urlLookup = new URLLookup(csSession);

            if (!urlLookup.LookupURL(k, out string url))
            {
                return NotFound();
            }

            queryResult.URL = url;

            return View(queryResult);
        }
    }
}
