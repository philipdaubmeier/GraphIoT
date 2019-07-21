using NodaTime;
using RichardSzalay.MockHttp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PhilipDaubmeier.DigitalstromDssMock
{
    public static class DigitalstromDssMockExtensions
    {
        public static MockHttpMessageHandler AddSensorMocks(this MockHttpMessageHandler mockHttp, int resolution = 60 * 5, int valueCount = 1)
        {
            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/apartment/getSensorValues")
                    .WithExactQueryString($"token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""weather"": {
                                          ""WeatherIconId"": ""01n"",
                                          ""WeatherConditionId"": ""800"",
                                          ""WeatherServiceId"": ""7"",
                                          ""WeatherServiceTime"": ""2019-05-01T21:15:07.560Z""
                                      },
                                      ""outdoor"": {},
                                      ""zones"": [
                                          {
                                              ""id"": 65534,
                                              ""name"": """",
                                              ""values"": []
                                          },
                                          {
                                              ""id"": 32027,
                                              ""name"": ""Kitchen"",
                                              ""values"": [" + string.Join(',', Enumerable.Range(0, valueCount)
                                                  .Select(x => new Tuple<int, string>(x, DateTime.UtcNow.AddSeconds(-1 * resolution * (valueCount - x)).ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture)))
                                                  .Select(x => @"
                                                  {
                                                      ""TemperatureValue"": " + (20.05M + x.Item1 / 10M).ToString(CultureInfo.InvariantCulture) + @",
                                                      ""TemperatureValueTime"": """ + x.Item2 + @"""
                                                  },
                                                  {
                                                      ""HumidityValue"": " + (42.475M + x.Item1 / 10M).ToString(CultureInfo.InvariantCulture) + @",
                                                      ""HumidityValueTime"": """ + x.Item2 + @"""
                                                  }")) + @"
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/zones/*(ZoneID)/groups/group0/sensor/*(type,value,time)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""zones"": [
                                          {
                                              ""ZoneID"": 0
                                          },
                                          {
                                              ""ZoneID"": 1
                                          },
                                          {
                                              ""ZoneID"": 32027,
                                              ""sensor"": [" + string.Join(',', Enumerable.Range(0, valueCount)
                                                  .Select(x => new Tuple<int, long>(x, Instant.FromDateTimeUtc(DateTime.UtcNow.AddSeconds(-1 * resolution * (valueCount - x))).ToUnixTimeSeconds()))
                                                  .Select(x => @"
                                                  {
                                                      ""type"": 9,
                                                      ""value"": " + (21.3M + x.Item1 / 10M).ToString(CultureInfo.InvariantCulture) + @",
                                                      ""time"": " + x.Item2 + @"
                                                  },
                                                  {
                                                      ""type"": 13,
                                                      ""value"": " + (45.975M + x.Item1 / 10M).ToString(CultureInfo.InvariantCulture) + @",
                                                      ""time"": " + x.Item2 + @"
                                                  }")) + @"
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            return mockHttp;
        }

        public static MockHttpMessageHandler AddEnergyMeteringMocks(this MockHttpMessageHandler mockHttp, int resolution = 1, int valueCount = 600)
        {
            mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/property/query")
                    .WithExactQueryString($"query=/apartment/dSMeters/*(dSUID,name)/capabilities(metering)&token={MockDigitalstromConnection.AppToken}")
                    .Respond("application/json", @"{
                                  ""result"":
                                  {
                                      ""dSMeters"": [
                                          {
                                              ""dSUID"": ""99999942f800000000000f0000deadbeef"",
                                              ""name"": ""dss meter"",
                                              ""capabilities"": [
                                                  {
                                                      ""metering"": true
                                                  }
                                              ]
                                          },
                                          {
                                              ""dSUID"": ""77777742f800000000000c0000cafecafe"",
                                              ""name"": ""dss meter"",
                                              ""capabilities"": [
                                                  {
                                                      ""metering"": true
                                                  }
                                              ]
                                          },
                                          {
                                              ""dSUID"": ""111111111800000000000f00000000abcd"",
                                              ""name"": ""vdc meter"",
                                              ""capabilities"": [
                                                  {
                                                      ""metering"": false
                                                  }
                                              ]
                                          }
                                      ]
                                  },
                                  ""ok"": true
                              }");

            foreach (var dsuid in new List<string>() { "99999942f800000000000f0000deadbeef", "77777742f800000000000c0000cafecafe" })
            {
                mockHttp.When($"{MockDigitalstromConnection.BaseUri}/json/metering/getValues")
                        .WithExactQueryString($"dsuid={dsuid}&type=consumption&resolution={resolution}&valueCount={valueCount}&token={MockDigitalstromConnection.AppToken}")
                        .Respond("application/json", @"{
                                      ""result"":
                                      {
                                          ""meterID"": """ + dsuid + @""",
                                          ""type"": ""consumption"",
                                          ""unit"": ""W"",
                                          ""resolution"": ""1"",
                                          ""values"": [" + string.Join(',', Enumerable.Range(0, valueCount).Select(x => @"
                                              [
                                                  " + Instant.FromDateTimeUtc(DateTime.UtcNow.AddSeconds(-1 * resolution * (valueCount - x))).ToUnixTimeSeconds() + @",
                                                  " + (dsuid.StartsWith("9") ? 89 + x : 7 + x * 2) + @"
                                              ]")) + @"
                                          ]
                                      },
                                      ""ok"": true
                                  }");
            }

            return mockHttp;
        }
    }
}