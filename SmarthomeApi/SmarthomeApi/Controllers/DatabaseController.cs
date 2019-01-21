using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmarthomeApi.Database.Model;

namespace SmarthomeApi.Controllers
{
    [Route("api/database")]
    public class DatabaseController : Controller
    {
        private readonly PersistenceContext db;
        public DatabaseController(PersistenceContext databaseContext)
        {
            db = databaseContext;
        }

        // GET api/database/authdatasets
        [HttpGet("authdatasets")]
        public async Task<ActionResult> Get(string password)
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
