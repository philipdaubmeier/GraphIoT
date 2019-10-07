﻿using Microsoft.AspNetCore.Mvc;
using PhilipDaubmeier.DigitalstromClient;
using PhilipDaubmeier.DigitalstromClient.Network;
using PhilipDaubmeier.GraphIoT.App.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipDaubmeier.GraphIoT.App.Controllers
{
    [Route("powerconsumption")]
    public class PowerConsumptionController : Controller
    {
        private static DigitalstromClient.DigitalstromDssClient dsClient;

        private readonly PersistenceContext db;
        public PowerConsumptionController(PersistenceContext databaseContext, IDigitalstromConnectionProvider connectionProvider)
        {
            db = databaseContext;
            dsClient = new DigitalstromClient.DigitalstromDssClient(connectionProvider);
        }

        // GET powerconsumption
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            var powerValues = (await dsClient.GetTotalEnergy(60, 37)).TimeSeries
                .TakeLast(37).Select(x => (int)Math.Round(x.Value, 0)).ToList();

            return Json(new
            { frames = new List<object>() {
                    new
                    {
                        text = $"{powerValues.LastOrDefault()} W",
                        icon = "a21256"
                    },
                    new
                    {
                        index = 1,
                        chartData = powerValues
                    }
                }
            });
        }
    }
}