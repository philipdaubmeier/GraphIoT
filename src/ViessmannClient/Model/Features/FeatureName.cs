using System.Collections.Generic;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class FeatureName
    {
        public enum Circuit
        {
            Circuit0,
            Circuit1,
            Circuit2
        }

        public enum Name
        {
            Heating,
            HeatingBoiler,
            HeatingBoilerSensors,
            HeatingBoilerSerial,
            HeatingBoilerSensorsTemperatureCommonSupply,
            HeatingBoilerSensorsTemperatureMain,
            HeatingBoilerTemperature,
            HeatingBurner,
            HeatingBurnerAutomatic,
            HeatingBurnerModulation,
            HeatingBurnerStatistics,
            HeatingCircuits,
            HeatingCircuitsCircuit,
            HeatingCircuitsCirculation,
            HeatingCircuitsCirculationPump,
            HeatingCircuitsCirculationSchedule,
            HeatingCircuitsDhw,
            HeatingCircuitsDhwPumps,
            HeatingCircuitsDhwPumpsCirculation,
            HeatingCircuitsDhwPumpsCirculationSchedule,
            HeatingCircuitsDhwSchedule,
            HeatingCircuitsFrostprotection,
            HeatingCircuitsHeating,
            HeatingCircuitsHeatingCurve,
            HeatingCircuitsHeatingSchedule,
            HeatingCircuitsOperating,
            HeatingCircuitsOperatingModes,
            HeatingCircuitsOperatingModesActive,
            HeatingCircuitsOperatingModesDhw,
            HeatingCircuitsOperatingModesHeating,
            HeatingCircuitsOperatingModesDhwAndHeating,
            HeatingCircuitsOperatingModesForcedNormal,
            HeatingCircuitsOperatingModesForcedReduced,
            HeatingCircuitsOperatingModesStandby,
            HeatingCircuitsOperatingPrograms,
            HeatingCircuitsOperatingProgramsActive,
            HeatingCircuitsOperatingProgramsComfort,
            HeatingCircuitsOperatingProgramsEco,
            HeatingCircuitsOperatingProgramsExternal,
            HeatingCircuitsOperatingProgramsHoliday,
            HeatingCircuitsOperatingProgramsNormal,
            HeatingCircuitsOperatingProgramsReduced,
            HeatingCircuitsOperatingProgramsStandby,
            HeatingCircuitsSensors,
            HeatingCircuitsSensorsTemperature,
            HeatingCircuitsSensorsTemperatureRoom,
            HeatingCircuitsSensorsTemperatureSupply,
            HeatingCircuitsGeofencing,
            HeatingConfigurationMultiFamilyHouse,
            HeatingControllerSerial,
            HeatingDevice,
            HeatingDeviceTime,
            HeatingDeviceTimeOffset,
            HeatingDhw,
            HeatingDhwCharging,
            HeatingDhwPumpsCirculation,
            HeatingDhwPumpsCirculationSchedule,
            HeatingDhwPumpsPrimary,
            HeatingDhwSchedule,
            HeatingDhwSensors,
            HeatingDhwSensorsTemperatureHotWaterStorage,
            HeatingDhwSensorsTemperatureOutlet,
            HeatingDhwTemperature,
            HeatingDhwTemperatureMain,
            HeatingErrors,
            HeatingErrorsActive,
            HeatingErrorsHistory,
            HeatingSensors,
            HeatingSensorsTemperature,
            HeatingSensorsTemperatureOutside,
            HeatingServiceTimeBased,
            HeatingServiceBurnerBased,
            HeatingService,
            HeatingSolar,
            HeatingSolarPowerProduction,
            HeatingSolarPumpsCircuit,
            HeatingSolarStatistics,
            HeatingSolarSensors,
            HeatingSolarSensorsTemperature,
            HeatingSolarSensorsTemperatureDhw,
            HeatingSolarSensorsTemperatureCollector,
            HeatingSolarPowerCumulativeProduced,
            HeatingSolarRechargeSuppression
        }

        private static readonly Dictionary<string, Name> _mapping = new Dictionary<string, Name>()
        {
            {"heating", Name.Heating},
            {"heating.boiler", Name.HeatingBoiler},
            {"heating.boiler.sensors", Name.HeatingBoilerSensors},
            {"heating.boiler.serial", Name.HeatingBoilerSerial},
            {"heating.boiler.sensors.temperature.commonSupply", Name.HeatingBoilerSensorsTemperatureCommonSupply},
            {"heating.boiler.sensors.temperature.main", Name.HeatingBoilerSensorsTemperatureMain},
            {"heating.boiler.temperature", Name.HeatingBoilerTemperature},
            {"heating.burner", Name.HeatingBurner},
            {"heating.burner.automatic", Name.HeatingBurnerAutomatic},
            {"heating.burner.modulation", Name.HeatingBurnerModulation},
            {"heating.burner.statistics", Name.HeatingBurnerStatistics},
            {"heating.circuits", Name.HeatingCircuits},
            {"heating.circuits.{circuit}", Name.HeatingCircuitsCircuit},
            {"heating.circuits.{circuit}.circulation", Name.HeatingCircuitsCirculation},
            {"heating.circuits.{circuit}.circulation.pump", Name.HeatingCircuitsCirculationPump},
            {"heating.circuits.{circuit}.circulation.schedule", Name.HeatingCircuitsCirculationSchedule},
            {"heating.circuits.{circuit}.dhw", Name.HeatingCircuitsDhw},
            {"heating.circuits.{circuit}.dhw.pumps", Name.HeatingCircuitsDhwPumps},
            {"heating.circuits.{circuit}.dhw.pumps.circulation", Name.HeatingCircuitsDhwPumpsCirculation},
            {"heating.circuits.{circuit}.dhw.pumps.circulation.schedule", Name.HeatingCircuitsDhwPumpsCirculationSchedule},
            {"heating.circuits.{circuit}.dhw.schedule", Name.HeatingCircuitsDhwSchedule},
            {"heating.circuits.{circuit}.frostprotection", Name.HeatingCircuitsFrostprotection},
            {"heating.circuits.{circuit}.heating", Name.HeatingCircuitsHeating},
            {"heating.circuits.{circuit}.heating.curve", Name.HeatingCircuitsHeatingCurve},
            {"heating.circuits.{circuit}.heating.schedule", Name.HeatingCircuitsHeatingSchedule},
            {"heating.circuits.{circuit}.operating", Name.HeatingCircuitsOperating},
            {"heating.circuits.{circuit}.operating.modes", Name.HeatingCircuitsOperatingModes},
            {"heating.circuits.{circuit}.operating.modes.active", Name.HeatingCircuitsOperatingModesActive},
            {"heating.circuits.{circuit}.operating.modes.dhw", Name.HeatingCircuitsOperatingModesDhw},
            {"heating.circuits.{circuit}.operating.modes.heating", Name.HeatingCircuitsOperatingModesHeating},
            {"heating.circuits.{circuit}.operating.modes.dhwAndHeating", Name.HeatingCircuitsOperatingModesDhwAndHeating},
            {"heating.circuits.{circuit}.operating.modes.forcedNormal", Name.HeatingCircuitsOperatingModesForcedNormal},
            {"heating.circuits.{circuit}.operating.modes.forcedReduced", Name.HeatingCircuitsOperatingModesForcedReduced},
            {"heating.circuits.{circuit}.operating.modes.standby", Name.HeatingCircuitsOperatingModesStandby},
            {"heating.circuits.{circuit}.operating.programs", Name.HeatingCircuitsOperatingPrograms},
            {"heating.circuits.{circuit}.operating.programs.active", Name.HeatingCircuitsOperatingProgramsActive},
            {"heating.circuits.{circuit}.operating.programs.comfort", Name.HeatingCircuitsOperatingProgramsComfort},
            {"heating.circuits.{circuit}.operating.programs.eco", Name.HeatingCircuitsOperatingProgramsEco},
            {"heating.circuits.{circuit}.operating.programs.external", Name.HeatingCircuitsOperatingProgramsExternal},
            {"heating.circuits.{circuit}.operating.programs.holiday", Name.HeatingCircuitsOperatingProgramsHoliday},
            {"heating.circuits.{circuit}.operating.programs.normal", Name.HeatingCircuitsOperatingProgramsNormal},
            {"heating.circuits.{circuit}.operating.programs.reduced", Name.HeatingCircuitsOperatingProgramsReduced},
            {"heating.circuits.{circuit}.operating.programs.standby", Name.HeatingCircuitsOperatingProgramsStandby},
            {"heating.circuits.{circuit}.sensors", Name.HeatingCircuitsSensors},
            {"heating.circuits.{circuit}.sensors.temperature", Name.HeatingCircuitsSensorsTemperature},
            {"heating.circuits.{circuit}.sensors.temperature.room", Name.HeatingCircuitsSensorsTemperatureRoom},
            {"heating.circuits.{circuit}.sensors.temperature.supply", Name.HeatingCircuitsSensorsTemperatureSupply},
            {"heating.circuits.{circuit}.geofencing", Name.HeatingCircuitsGeofencing},
            {"heating.configuration.multiFamilyHouse", Name.HeatingConfigurationMultiFamilyHouse},
            {"heating.controller.serial", Name.HeatingControllerSerial},
            {"heating.device", Name.HeatingDevice},
            {"heating.device.time", Name.HeatingDeviceTime},
            {"heating.device.time.offset", Name.HeatingDeviceTimeOffset},
            {"heating.dhw", Name.HeatingDhw},
            {"heating.dhw.charging", Name.HeatingDhwCharging},
            {"heating.dhw.pumps.circulation", Name.HeatingDhwPumpsCirculation},
            {"heating.dhw.pumps.circulation.schedule", Name.HeatingDhwPumpsCirculationSchedule},
            {"heating.dhw.pumps.primary", Name.HeatingDhwPumpsPrimary},
            {"heating.dhw.schedule", Name.HeatingDhwSchedule},
            {"heating.dhw.sensors", Name.HeatingDhwSensors},
            {"heating.dhw.sensors.temperature.hotWaterStorage", Name.HeatingDhwSensorsTemperatureHotWaterStorage},
            {"heating.dhw.sensors.temperature.outlet", Name.HeatingDhwSensorsTemperatureOutlet},
            {"heating.dhw.temperature", Name.HeatingDhwTemperature},
            {"heating.dhw.temperature.main", Name.HeatingDhwTemperatureMain},
            {"heating.errors", Name.HeatingErrors},
            {"heating.errors.active", Name.HeatingErrorsActive},
            {"heating.errors.history", Name.HeatingErrorsHistory},
            {"heating.sensors", Name.HeatingSensors},
            {"heating.sensors.temperature", Name.HeatingSensorsTemperature},
            {"heating.sensors.temperature.outside", Name.HeatingSensorsTemperatureOutside},
            {"heating.service.timeBased", Name.HeatingServiceTimeBased},
            {"heating.service.burnerBased", Name.HeatingServiceBurnerBased},
            {"heating.service", Name.HeatingService},
            {"heating.solar", Name.HeatingSolar},
            {"heating.solar.power.production", Name.HeatingSolarPowerProduction},
            {"heating.solar.pumps.circuit", Name.HeatingSolarPumpsCircuit},
            {"heating.solar.statistics", Name.HeatingSolarStatistics},
            {"heating.solar.sensors", Name.HeatingSolarSensors},
            {"heating.solar.sensors.temperature", Name.HeatingSolarSensorsTemperature},
            {"heating.solar.sensors.temperature.dhw", Name.HeatingSolarSensorsTemperatureDhw},
            {"heating.solar.sensors.temperature.collector", Name.HeatingSolarSensorsTemperatureCollector},
            {"heating.solar.power.cumulativeProduced", Name.HeatingSolarPowerCumulativeProduced},
            {"heating.solar.rechargeSuppression", Name.HeatingSolarRechargeSuppression}
        };

        private readonly Name _name = Name.Heating;

        public Circuit CircuitNum { get; } = Circuit.Circuit0;

        public FeatureName(Name name, Circuit? circuit = null)
        {
            _name = name;
            CircuitNum = circuit ?? Circuit.Circuit0;
        }

        public static implicit operator FeatureName(string name)
        {
            var circuit = Circuit.Circuit0;
            if (name.Length >= 18 && int.TryParse(name.Replace("heating.circuits.", "").Substring(0, 1), out int circuitNum))
            {
                circuit = circuitNum switch
                {
                    1 => Circuit.Circuit1,
                    2 => Circuit.Circuit2,
                    _ => Circuit.Circuit0
                };
            }

            var placeholder = ".{circuit}";
            name = name.Replace(".0", placeholder).Replace(".1", placeholder).Replace(".2", placeholder);

            if (!_mapping.TryGetValue(name, out Name enumName))
                return new FeatureName(Name.Heating);

            return new FeatureName(enumName, circuit);
        }

        public static implicit operator Name(FeatureName name)
        {
            return name._name;
        }

        public static implicit operator FeatureName(Name name)
        {
            return new FeatureName(name);
        }
    }
}