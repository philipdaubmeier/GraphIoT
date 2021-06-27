using PhilipDaubmeier.ViessmannClient.Network;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace PhilipDaubmeier.ViessmannClient.Tests
{
    public static class MockDeviceFeatures
    {
        public static MockHttpMessageHandler AddAllDeviceFeatures(this MockHttpMessageHandler mockHttp)
        {
            mockHttp.When($"{MockViessmannConnection.BaseUri}iot/v1/equipment/installations/{MockViessmannConnection.InstallationId}/gateways/{MockViessmannConnection.GatewayId}/devices/{MockViessmannConnection.DeviceId}/features")
                    .Respond("application/json",
                    @"{
                          ""data"": [
                              {
                                  ""feature"": ""heating""
                              },
                              {
                                  ""feature"":""device.messages.logbook"",
                                  ""properties"": {
                                      ""entries"": {
                                          ""value"": [
                                              {
                                                  ""timestamp"": ""2020-01-01T00:00:00.000Z"",
                                                  ""actor"": ""SECONDARY_PUMP1"",
                                                  ""status"": 0,
                                                  ""event"": ""Inverter_CPU_error"",
                                                  ""stateMachine"": ""TPM_SC1"",
                                                  ""additionalInfo"": 120
                                              }
                                          ],
                                          ""type"": ""array""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.boiler""
                              },
                              {
                                  ""feature"": ""heating.boiler.sensors""
                              },
                              {
                                  ""feature"": ""heating.boiler.serial"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""string"",
                                          ""value"": """ + MockViessmannConnection.GatewayId + @"""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.boiler.sensors.temperature.commonSupply"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""error""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.boiler.sensors.temperature.main"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""connected""
                                      },
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 47
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.boiler.temperature"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 47.4
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.burner"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.burner.automatic"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""ok""
                                      },
                                      ""errorCode"": {
                                          ""type"": ""number"",
                                          ""value"": 0
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.burner.modulation"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 0
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.burner.statistics"",
                                  ""properties"": {
                                      ""hours"": {
                                          ""type"": ""number"",
                                          ""value"": 9876.12
                                      },
                                      ""starts"": {
                                          ""type"": ""number"",
                                          ""value"": 44321
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits"",
                                  ""properties"": {
                                      ""enabled"": {
                                          ""type"": ""array"",
                                          ""value"": [
                                              ""0"",
                                              ""1""
                                          ]
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.0"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""name"": {
                                          ""type"": ""string"",
                                          ""value"": """"
                                      }
                                  },
                                  ""commands"": {
                                      ""setName"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0/setName"",
                                          ""name"": ""setName"",
                                          ""isExecutable"": false,
                                          ""params"": {
                                              ""name"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {
                                                      ""minLength"": 1,
                                                      ""maxLength"": 20
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""name"": {
                                          ""type"": ""string"",
                                          ""value"": """"
                                      }
                                  },
                                  ""commands"": {
                                      ""setName"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1/setName"",
                                          ""name"": ""setName"",
                                          ""isExecutable"": false,
                                          ""params"": {
                                              ""name"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {
                                                      ""minLength"": 1,
                                                      ""maxLength"": 20
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.circulation""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.circulation""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.circulation""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.circulation.pump"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""on""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.circulation.pump"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""on""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.circulation.pump""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.circulation.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.circulation.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.circulation.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.dhw""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.dhw""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.dhw""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.dhw.pumps""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.dhw.pumps""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.dhw.pumps""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.dhw.pumps.circulation""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.dhw.pumps.circulation""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.dhw.pumps.circulation""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.dhw.pumps.circulation.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.dhw.pumps.circulation.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.dhw.pumps.circulation.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.dhw.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.dhw.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.dhw.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.frostprotection"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""off""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.frostprotection"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""on""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.frostprotection""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.heating""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.heating""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.heating""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.heating.curve"",
                                  ""properties"": {
                                      ""shift"": {
                                          ""type"": ""number"",
                                          ""value"": 5
                                      },
                                      ""slope"": {
                                          ""type"": ""number"",
                                          ""value"": 0.9
                                      }
                                  },
                                  ""commands"": {
                                      ""setCurve"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.heating.curve/setCurve"",
                                          ""name"": ""setCurve"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""slope"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 0.2,
                                                      ""max"": 3.5,
                                                      ""stepping"": 0.1
                                                  }
                                              },
                                              ""shift"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": -13,
                                                      ""max"": 40,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.heating.curve"",
                                  ""properties"": {
                                      ""shift"": {
                                          ""type"": ""number"",
                                          ""value"": 0
                                      },
                                      ""slope"": {
                                          ""type"": ""number"",
                                          ""value"": 0.5
                                      }
                                  },
                                  ""commands"": {
                                      ""setCurve"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.heating.curve/setCurve"",
                                          ""name"": ""setCurve"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""slope"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 0.2,
                                                      ""max"": 3.5,
                                                      ""stepping"": 0.1
                                                  }
                                              },
                                              ""shift"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": -13,
                                                      ""max"": 40,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.heating.curve""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.heating.schedule"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""entries"": {
                                          ""type"": ""Schedule"",
                                          ""value"": {
                                              ""mon"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""tue"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""wed"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""thu"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""fri"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""sat"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""sun"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ]
                                          }
                                      },
                                      ""overlapAllowed"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      }
                                  },
                                  ""commands"": {
                                      ""setSchedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.heating.schedule/setSchedule"",
                                          ""name"": ""setSchedule"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""newSchedule"": {
                                                  ""required"": true,
                                                  ""type"": ""Schedule"",
                                                  ""constraints"": {
                                                      ""maxEntries"": 4,
                                                      ""resolution"": 10,
                                                      ""modes"": [
                                                          ""normal""
                                                      ],
                                                      ""defaultMode"": ""reduced""
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.heating.schedule"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""entries"": {
                                          ""type"": ""Schedule"",
                                          ""value"": {
                                              ""mon"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""tue"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""wed"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""thu"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""fri"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""sat"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""sun"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ]
                                          }
                                      },
                                      ""overlapAllowed"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      }
                                  },
                                  ""commands"": {
                                      ""setSchedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.heating.schedule/setSchedule"",
                                          ""name"": ""setSchedule"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""newSchedule"": {
                                                  ""required"": true,
                                                  ""type"": ""Schedule"",
                                                  ""constraints"": {
                                                      ""maxEntries"": 4,
                                                      ""resolution"": 10,
                                                      ""modes"": [
                                                          ""normal""
                                                      ],
                                                      ""defaultMode"": ""reduced""
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""properties"":{
                                      ""entries"":{
                                          ""value"":[
                                              {
                                                  ""timestamp"": ""2020-09-15T06:38:01.000Z"",
                                                  ""errorCode"": ""49"",
                                                  ""status"": ""inactive"",
                                                  ""count"": 39,
                                                  ""priority"": ""info""
                                              }
                                          ],
                                          ""type"": ""array""
                                      }
                                  },
                                  ""feature"": ""heating.coolingCircuits.0.messages"",
                                  ""timestamp"": ""2020-09-16T06:15:50.236Z"",
                                  ""isEnabled"": true,
                                  ""isReady"": true,
                                  ""deviceId"": ""0""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.heating.schedule""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.modes""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.modes""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.modes""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.modes.active"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""string"",
                                          ""value"": ""dhwAndHeating""
                                      }
                                  },
                                  ""commands"": {
                                      ""setMode"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.modes.active/setMode"",
                                          ""name"": ""setMode"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""mode"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {
                                                      ""enum"": [
                                                          ""dhw"",
                                                          ""dhwAndHeating"",
                                                          ""forcedNormal"",
                                                          ""forcedReduced"",
                                                          ""standby""
                                                      ]
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.modes.active"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""string"",
                                          ""value"": ""dhwAndHeating""
                                      }
                                  },
                                  ""commands"": {
                                      ""setMode"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.modes.active/setMode"",
                                          ""name"": ""setMode"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""mode"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {
                                                      ""enum"": [
                                                          ""dhw"",
                                                          ""dhwAndHeating"",
                                                          ""forcedNormal"",
                                                          ""forcedReduced"",
                                                          ""standby""
                                                      ]
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.modes.active""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.modes.dhw"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.modes.dhw"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.modes.dhw""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.modes.heating""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.modes.heating""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.modes.heating""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.modes.dhwAndHeating"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.modes.dhwAndHeating"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.modes.dhwAndHeating""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.modes.forcedNormal"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.modes.forcedNormal"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.modes.forcedNormal""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.modes.forcedReduced"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.modes.forcedReduced"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.modes.forcedReduced""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.modes.standby"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.modes.standby"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.modes.standby""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs.active"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""string"",
                                          ""value"": ""normal""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs.active"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""string"",
                                          ""value"": ""normal""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs.active""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs.comfort"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 20
                                      }
                                  },
                                  ""commands"": {
                                      ""setTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.comfort/setTemperature"",
                                          ""name"": ""setTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""targetTemperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 4,
                                                      ""max"": 37,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      },
                                      ""activate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.comfort/activate"",
                                          ""name"": ""activate"",
                                          ""isExecutable"": true,
                                          ""params"": {}
                                      },
                                      ""deactivate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.comfort/deactivate"",
                                          ""name"": ""deactivate"",
                                          ""isExecutable"": false,
                                          ""params"": {}
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs.comfort"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 24
                                      }
                                  },
                                  ""commands"": {
                                      ""setTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.comfort/setTemperature"",
                                          ""name"": ""setTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""targetTemperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 4,
                                                      ""max"": 37,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      },
                                      ""activate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.comfort/activate"",
                                          ""name"": ""activate"",
                                          ""isExecutable"": true,
                                          ""params"": {}
                                      },
                                      ""deactivate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.comfort/deactivate"",
                                          ""name"": ""deactivate"",
                                          ""isExecutable"": false,
                                          ""params"": {}
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs.comfort""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs.eco"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 23
                                      }
                                  },
                                  ""commands"": {
                                      ""activate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.eco/activate"",
                                          ""name"": ""activate"",
                                          ""isExecutable"": true,
                                          ""params"": {}
                                      },
                                      ""deactivate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.eco/deactivate"",
                                          ""name"": ""deactivate"",
                                          ""isExecutable"": false,
                                          ""params"": {}
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs.eco"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 21
                                      }
                                  },
                                  ""commands"": {
                                      ""activate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.eco/activate"",
                                          ""name"": ""activate"",
                                          ""isExecutable"": true,
                                          ""params"": {}
                                      },
                                      ""deactivate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.eco/deactivate"",
                                          ""name"": ""deactivate"",
                                          ""isExecutable"": false,
                                          ""params"": {}
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs.eco""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs.external"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 0
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs.external"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 0
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs.external""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs.holiday"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""start"": {
                                          ""type"": ""string"",
                                          ""value"": """"
                                      },
                                      ""end"": {
                                          ""type"": ""string"",
                                          ""value"": """"
                                      }
                                  },
                                  ""commands"": {
                                      ""changeEndDate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.holiday/changeEndDate"",
                                          ""name"": ""changeEndDate"",
                                          ""isExecutable"": false,
                                          ""params"": {
                                              ""end"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {}
                                              }
                                          }
                                      },
                                      ""schedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.holiday/schedule"",
                                          ""name"": ""schedule"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""start"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {}
                                              },
                                              ""end"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {}
                                              }
                                          }
                                      },
                                      ""unschedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.holiday/unschedule"",
                                          ""name"": ""unschedule"",
                                          ""isExecutable"": true,
                                          ""params"": {}
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs.holiday"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""start"": {
                                          ""type"": ""string"",
                                          ""value"": """"
                                      },
                                      ""end"": {
                                          ""type"": ""string"",
                                          ""value"": """"
                                      }
                                  },
                                  ""commands"": {
                                      ""changeEndDate"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.holiday/changeEndDate"",
                                          ""name"": ""changeEndDate"",
                                          ""isExecutable"": false,
                                          ""params"": {
                                              ""end"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {}
                                              }
                                          }
                                      },
                                      ""schedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.holiday/schedule"",
                                          ""name"": ""schedule"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""start"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {}
                                              },
                                              ""end"": {
                                                  ""required"": true,
                                                  ""type"": ""string"",
                                                  ""constraints"": {}
                                              }
                                          }
                                      },
                                      ""unschedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.holiday/unschedule"",
                                          ""name"": ""unschedule"",
                                          ""isExecutable"": true,
                                          ""params"": {}
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs.holiday""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs.normal"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 23
                                      }
                                  },
                                  ""commands"": {
                                      ""setTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.normal/setTemperature"",
                                          ""name"": ""setTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""targetTemperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 3,
                                                      ""max"": 37,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs.normal"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 21
                                      }
                                  },
                                  ""commands"": {
                                      ""setTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.normal/setTemperature"",
                                          ""name"": ""setTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""targetTemperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 3,
                                                      ""max"": 37,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs.normal""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs.reduced"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 10
                                      }
                                  },
                                  ""commands"": {
                                      ""setTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.0.operating.programs.reduced/setTemperature"",
                                          ""name"": ""setTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""targetTemperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 3,
                                                      ""max"": 37,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs.reduced"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""temperature"": {
                                          ""type"": ""number"",
                                          ""value"": 12
                                      }
                                  },
                                  ""commands"": {
                                      ""setTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.circuits.1.operating.programs.reduced/setTemperature"",
                                          ""name"": ""setTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""targetTemperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 3,
                                                      ""max"": 37,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs.reduced""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.operating.programs.standby"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.operating.programs.standby"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.operating.programs.standby""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.sensors""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.sensors""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.sensors""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.sensors.temperature""
                              },
                              {
                                  ""feature"": ""heating.circuits.1.sensors.temperature""
                              },
                              {
                                  ""feature"": ""heating.circuits.2.sensors.temperature""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.sensors.temperature.room"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""notConnected""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.sensors.temperature.room"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""notConnected""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.sensors.temperature.room""
                              },
                              {
                                  ""feature"": ""heating.circuits.0.sensors.temperature.supply"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""notConnected""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.sensors.temperature.supply"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""notConnected""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.sensors.temperature.supply""
                              },
                              {
                                  ""feature"": ""heating.configuration.multiFamilyHouse"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.controller.serial"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""string"",
                                          ""value"": ""777555888999""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.device""
                              },
                              {
                                  ""feature"": ""heating.device.time""
                              },
                              {
                                  ""feature"": ""heating.device.time.offset"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 52
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.charging"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.pumps.circulation"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""off""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.pumps.circulation.schedule"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""entries"": {
                                          ""type"": ""Schedule"",
                                          ""value"": {
                                              ""mon"": [ { ""start"": ""08:00"", ""end"": ""08:20"", ""mode"": ""on"", ""position"": 0 }, { ""start"": ""16:00"", ""end"": ""16:30"", ""mode"": ""on"", ""position"": 1 } ],
                                              ""tue"": [ { ""start"": ""08:00"", ""end"": ""08:20"", ""mode"": ""on"", ""position"": 0 }, { ""start"": ""16:00"", ""end"": ""16:30"", ""mode"": ""on"", ""position"": 1 } ],
                                              ""wed"": [ { ""start"": ""08:00"", ""end"": ""08:20"", ""mode"": ""on"", ""position"": 0 }, { ""start"": ""16:00"", ""end"": ""16:30"", ""mode"": ""on"", ""position"": 1 } ],
                                              ""thu"": [ { ""start"": ""08:00"", ""end"": ""08:20"", ""mode"": ""on"", ""position"": 0 }, { ""start"": ""16:00"", ""end"": ""16:30"", ""mode"": ""on"", ""position"": 1 } ],
                                              ""fri"": [ { ""start"": ""08:00"", ""end"": ""08:20"", ""mode"": ""on"", ""position"": 0 }, { ""start"": ""16:00"", ""end"": ""16:30"", ""mode"": ""on"", ""position"": 1 } ],
                                              ""sat"": [ { ""start"": ""08:00"", ""end"": ""08:20"", ""mode"": ""on"", ""position"": 0 }, { ""start"": ""16:00"", ""end"": ""16:30"", ""mode"": ""on"", ""position"": 1 } ],
                                              ""sun"": [ { ""start"": ""08:00"", ""end"": ""08:20"", ""mode"": ""on"", ""position"": 0 }, { ""start"": ""16:00"", ""end"": ""16:30"", ""mode"": ""on"", ""position"": 1 } ]
                                          }
                                      }
                                  },
                                  ""commands"": {
                                      ""setSchedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.dhw.pumps.circulation.schedule/setSchedule"",
                                          ""name"": ""setSchedule"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""newSchedule"": {
                                                  ""required"": true,
                                                  ""type"": ""Schedule"",
                                                  ""constraints"": {
                                                      ""maxEntries"": 4,
                                                      ""resolution"": 10,
                                                      ""modes"": [
                                                          ""on""
                                                      ],
                                                      ""defaultMode"": ""off""
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.pumps.primary"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""off""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.schedule"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      },
                                      ""entries"": {
                                          ""type"": ""Schedule"",
                                          ""value"": {
                                              ""mon"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""tue"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""wed"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""thu"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""fri"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""sat"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ],
                                              ""sun"": [ { ""start"": ""00:00"", ""end"": ""24:00"", ""mode"": ""on"", ""position"": 0 } ]
                                          }
                                      }
                                  },
                                  ""commands"": {
                                      ""setSchedule"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.dhw.schedule/setSchedule"",
                                          ""name"": ""setSchedule"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""newSchedule"": {
                                                  ""required"": true,
                                                  ""type"": ""Schedule"",
                                                  ""constraints"": {
                                                      ""maxEntries"": 4,
                                                      ""resolution"": 10,
                                                      ""modes"": [
                                                          ""on""
                                                      ],
                                                      ""defaultMode"": ""off""
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.sensors""
                              },
                              {
                                  ""feature"": ""heating.dhw.sensors.temperature.hotWaterStorage"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""connected""
                                      },
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 55.4
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.sensors.temperature.outlet"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""error""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.temperature"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 50
                                      }
                                  },
                                  ""commands"": {
                                      ""setTargetTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.dhw.temperature/setTargetTemperature"",
                                          ""name"": ""setTargetTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""temperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 10,
                                                      ""max"": 60,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.dhw.temperature.main"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 50
                                      }
                                  },
                                  ""commands"": {
                                      ""setTargetTemperature"": {
                                          ""uri"": """ + MockViessmannConnection.BaseUri + @"operational-data/v2/installations/" + MockViessmannConnection.InstallationId + @"/gateways/" + MockViessmannConnection.GatewayId + @"/devices/" + MockViessmannConnection.DeviceId + @"/features/heating.dhw.temperature.main/setTargetTemperature"",
                                          ""name"": ""setTargetTemperature"",
                                          ""isExecutable"": true,
                                          ""params"": {
                                              ""temperature"": {
                                                  ""required"": true,
                                                  ""type"": ""number"",
                                                  ""constraints"": {
                                                      ""min"": 10,
                                                      ""max"": 60,
                                                      ""stepping"": 1
                                                  }
                                              }
                                          }
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.errors""
                              },
                              {
                                  ""feature"": ""heating.errors.active"",
                                  ""properties"": {
                                      ""entries"": {
                                          ""type"": ""ErrorListChanges"",
                                          ""value"": {}
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.errors.history"",
                                  ""properties"": {
                                      ""entries"": {
                                          ""type"": ""ErrorListChanges"",
                                          ""value"": {}
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.sensors""
                              },
                              {
                                  ""feature"": ""heating.sensors.temperature""
                              },
                              {
                                  ""feature"": ""heating.sensors.temperature.outside"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""connected""
                                      },
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 7.3
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.service.timeBased"",
                                  ""properties"": {
                                      ""serviceDue"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""serviceIntervalMonths"": {
                                          ""type"": ""number"",
                                          ""value"": 0
                                      },
                                      ""activeMonthSinceLastService"": {
                                          ""type"": ""number"",
                                          ""value"": 0
                                      },
                                      ""lastService"": {
                                          ""type"": ""string"",
                                          ""value"": """"
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.service.burnerBased""
                              },
                              {
                                  ""feature"": ""heating.service""
                              },
                              {
                                  ""feature"": ""heating.solar"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": true
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.solar.power.production"",
                                  ""properties"": {
                                      ""day"": {
                                          ""type"": ""array"",
                                          ""value"": [
                                              10.698,
                                              7.193,
                                              6.543,
                                              0,
                                              10.172,
                                              21.322,
                                              17.562,
                                              6.99
                                          ]
                                      },
                                      ""week"": {
                                          ""type"": ""array"",
                                          ""value"": []
                                      },
                                      ""month"": {
                                          ""type"": ""array"",
                                          ""value"": []
                                      },
                                      ""year"": {
                                          ""type"": ""array"",
                                          ""value"": []
                                      },
                                      ""unit"": {
                                          ""type"": ""string"",
                                          ""value"": ""kilowattHour""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.solar.pumps.circuit"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""on""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.solar.statistics"",
                                  ""properties"": {
                                      ""hours"": {
                                          ""type"": ""number"",
                                          ""value"": 11085
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.solar.sensors""
                              },
                              {
                                  ""feature"": ""heating.solar.sensors.temperature""
                              },
                              {
                                  ""feature"": ""heating.solar.sensors.temperature.dhw"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 41.4
                                      },
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""connected""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.solar.sensors.temperature.collector"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 50.7
                                      },
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""connected""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.solar.power.cumulativeProduced"",
                                  ""properties"": {
                                      ""value"": {
                                          ""type"": ""number"",
                                          ""value"": 23456
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.solar.rechargeSuppression"",
                                  ""properties"": {
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""on""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.0.geofencing"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""home""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.1.geofencing"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""home""
                                      }
                                  }
                              },
                              {
                                  ""feature"": ""heating.circuits.2.geofencing"",
                                  ""properties"": {
                                      ""active"": {
                                          ""type"": ""boolean"",
                                          ""value"": false
                                      },
                                      ""status"": {
                                          ""type"": ""string"",
                                          ""value"": ""home""
                                      }
                                  }
                              }
                          ]
                      }");

            return mockHttp;
        }
    }
}