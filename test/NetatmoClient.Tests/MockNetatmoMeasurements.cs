using PhilipDaubmeier.NetatmoClient.Model.Core;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhilipDaubmeier.NetatmoClient.Tests
{
    public static class MockNetatmoMeasurements
    {
        public static MockHttpMessageHandler AddMeasurements(this MockHttpMessageHandler mockHttp, ModuleId moduleId, IEnumerable<Measure> types, int num = 10)
        {
            var beginTime = 1585752321;
            var time = beginTime;
            var valueCount = 0;
            var pseudoRandom5MinSteps = new[] { 308, 307, 308, 295, 320, 304, 257, 305, 310, 302, 298, 306 };
            var pseudoRandomClumpSizes = new[] { 2, 3, 2, 2, 4 };

            static string GetExampleValueForType(Measure type) => ((MeasureType)type switch
                {
                    MeasureType.Temperature => 21.3,
                    MeasureType.Humidity => 58,
                    MeasureType.CO2 => 524,
                    MeasureType.Rain => 0,
                    _ => 0
                }).ToString(CultureInfo.InvariantCulture);

            mockHttp.When($"{MockNetatmoConnection.BaseUri}/api/getmeasure")
                    .WithFormData(new[]
                    {
                        ("module_id", (string)moduleId),
                        ("type", string.Join(',', types.Select(t => t.ToString()))),
                    }.Select(x => new KeyValuePair<string, string>(x.Item1, x.Item2)))
                    .Respond("application/json",
                    @"{
                        ""body"": [" + string.Join(',', Enumerable.Range(0, num).Select(i =>
                        {
                            var begTime = time;
                            var stepTime = pseudoRandom5MinSteps[i % pseudoRandom5MinSteps.Length];
                            var clumps = Math.Min(num - valueCount, pseudoRandomClumpSizes[i % pseudoRandomClumpSizes.Length]);
                            valueCount += clumps;
                            time += stepTime * clumps;

                            if (clumps <= 0)
                                return null;

                            return @"
                            {
                                ""beg_time"": " + begTime + @",
                                ""step_time"": " + stepTime + @",
                                ""value"": [" + string.Join(',', Enumerable.Range(0, clumps).Select(c => @"
                                    [
                                        " + string.Join(',', types.Select(t => GetExampleValueForType(t))) + @"
                                    ]")) + @"
                                ]
                            }";
                        }).Where(s => s != null)) + @"
                        ],
                        ""status"": ""ok"",
                        ""time_exec"": 0.03,
                        ""time_server"": 1585755824
                    }");

            return mockHttp;
        }
    }
}