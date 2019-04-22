using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.SmarthomeApi.Database.Model;
using System;
using System.Linq;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/database")]
    public class DatabaseController : Controller
    {
        private readonly TokenStoreDbContext db;
        public DatabaseController(TokenStoreDbContext databaseContext)
        {
            db = databaseContext;
        }

        // GET api/database/authdatasets
        [HttpGet("authdatasets")]
        public ActionResult Get(string password)
        {
            if (password != "9bb04cf87b69e851")
                return StatusCode(403);

            var authdata = db.AuthDataSet.ToList();

            return Json(new
            {
                authdata = authdata.Select(x => new
                {
                    id = x.AuthDataId,
                    value = !x.AuthDataId.EndsWith("_expiry") ? x.DataContent: DateTime.FromBinary(long.Parse(x.DataContent)).ToString("o")
                })
            });
        }
    }
}
