using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.CompactTimeSeries;
using PhilipDaubmeier.SmarthomeApi.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Database;
using PhilipDaubmeier.TimeseriesHostCommon.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PhilipDaubmeier.SmarthomeApi.Controllers
{
    [Route("api/backup")]
    public class BackupController : Controller
    {
        private readonly DatabaseBackupService<PersistenceContext> _backupService;
        public BackupController(DatabaseBackupService<PersistenceContext> backupService)
        {
            _backupService = backupService;
        }

        // GET api/backup/load
        [HttpGet("load")]
        public ActionResult LoadBackup([FromQuery] string begin, [FromQuery] string end, [FromQuery] string tablefilter)
        {
            if (!TimeSeriesSpanParser.TryParse(begin, end, 1.ToString(), out TimeSeriesSpan span))
                return StatusCode((int)HttpStatusCode.NotFound);

            var tableFilterList = ((string.IsNullOrWhiteSpace(tablefilter) ? null : tablefilter)?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(name => name.Trim()).ToList()) ?? new List<string>();

            return Json(new { tables = _backupService.CreateBackup(span.Begin.Date, span.End.Date, tableFilterList) });
        }

        // PUT api/backup/restore
        [HttpPut("restore")]
        public async Task<ActionResult> RestoreBackup(CancellationToken cancellationToken)
        {
            return await RestoreBackupFromStream(Request.Body, cancellationToken);
        }

        // POST api/backup/poll
        [HttpPost("poll")]
        public async Task<ActionResult> PollBackupAndApplyToLocal([FromQuery] string uri, [FromQuery] string begin, [FromQuery] string end, [FromQuery] string tablefilter, CancellationToken cancellationToken)
        {
            var polled = await new HttpClient().GetStreamAsync($"{uri}/api/backup/load?begin={begin}&end={end}&tablefilter={tablefilter}");
            return await RestoreBackupFromStream(polled, cancellationToken);
        }

        private async Task<ActionResult> RestoreBackupFromStream(Stream stream, CancellationToken cancellationToken)
        {
            try
            {
                await _backupService.RestoreBackup(stream, cancellationToken);
            }
            catch (IOException ex)
            {
                var result = Json(new { ok = false, error = ex.Message });
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
                return result;
            }
            catch (Exception)
            {
                var result = Json(new { ok = false, error = "unknown reason" });
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
                return result;
            }

            return StatusCode((int)HttpStatusCode.Created);
        }
    }
}