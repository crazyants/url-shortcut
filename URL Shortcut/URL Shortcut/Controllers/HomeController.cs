using Cassandra;
using Microsoft.AspNetCore.Mvc;
using URL_Shortcut.Models;
using URL_Shortcut.Models.POCOs;
using URL_Shortcut.Utils.Database;

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
            // Create the return object
            QueryResult queryResult = new QueryResult()
            {
                URL = null
            };

            // Return empty if no key specified in query string
            if (k == null || k == string.Empty)
            {
                return View(queryResult);
            }

            // Make a connection to Cassandra and get a session
            CassandraConnection cassandraConnection = new CassandraConnection();
            ISession csSession = cassandraConnection.GetSession();

            URLLookup urlLookup = new URLLookup(csSession);

            // Get the actual long URL
            if (!urlLookup.LookupURL(k, out string url))
            {
                // Return NotFound page if key doesn't exist
                return NotFound();
            }

            // Attache the URL to the result object
            queryResult.URL = url;

            // Return result
            return View(queryResult);
        }
    }
}
