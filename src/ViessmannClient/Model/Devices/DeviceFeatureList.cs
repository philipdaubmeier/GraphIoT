using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class DeviceFeatureList : FeatureList
    {
        public IEnumerable<Message> GetDeviceMessagesLogbook()
        {
            return GetProperties(FeatureName.Name.DeviceMessagesLogbook)?.Messages?.Value ?? new List<Message>();
        }

        public string GetHeatingBoilerSerial()
        {
            return GetProperties(FeatureName.Name.HeatingBoilerSerial)?.Value?.Value as string ?? string.Empty;
        }

        public bool IsHeatingBoilerSensorsTemperatureCommonSupplyConnected()
        {
            return GetProperties(FeatureName.Name.HeatingBoilerSensorsTemperatureCommonSupply)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingBoilerSensorsTemperatureCommonSupply()
        {
            return GetProperties(FeatureName.Name.HeatingBoilerSensorsTemperatureCommonSupply)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingBoilerSensorsTemperatureMainConnected()
        {
            return GetProperties(FeatureName.Name.HeatingBoilerSensorsTemperatureMain)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingBoilerSensorsTemperatureMain()
        {
            return GetProperties(FeatureName.Name.HeatingBoilerSensorsTemperatureMain)?.ValueAsDouble ?? double.NaN;
        }

        public double GetHeatingBoilerTemperature()
        {
            return GetProperties(FeatureName.Name.HeatingBoilerTemperature)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingBurnerActive()
        {
            return GetProperties(FeatureName.Name.HeatingBurner)?.Active?.Value ?? false;
        }

        public bool IsGetHeatingBurnerAutomaticStatusOk()
        {
            return GetProperties(FeatureName.Name.HeatingBurnerAutomatic)?.Status?.Value?.Equals("ok", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public int GetHeatingBurnerAutomaticErrorCode()
        {
            return GetProperties(FeatureName.Name.HeatingBurnerAutomatic)?.ErrorCode?.Value ?? 0;
        }

        public int GetHeatingBurnerModulation()
        {
            return GetProperties(FeatureName.Name.HeatingBurnerModulation)?.ValueAsInt ?? 0;
        }

        public decimal GetHeatingBurnerStatisticsHours()
        {
            return GetProperties(FeatureName.Name.HeatingBurnerStatistics)?.Hours?.Value ?? 0m;
        }

        public long GetHeatingBurnerStatisticsStarts()
        {
            return GetProperties(FeatureName.Name.HeatingBurnerStatistics)?.Starts?.Value ?? 0;
        }

        public IEnumerable<FeatureName.Circuit> GetHeatingCircuits()
        {
            return GetProperties(FeatureName.Name.HeatingCircuits)?.Enabled?.Value?.Select(c => int.TryParse(c, out int number) ? (FeatureName.Circuit)number : FeatureName.Circuit.Circuit0).Distinct().ToList() ?? new List<FeatureName.Circuit>();
        }

        public bool IsHeatingCircuitsCircuitActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsCircuit, circuit)?.Active?.Value ?? false;
        }

        public string GetHeatingCircuitsCircuitName(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsCircuit, circuit)?.Name?.Value ?? string.Empty;
        }

        public bool IsHeatingCircuitsCirculationPumpOn(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsCirculationPump, circuit)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool IsHeatingCircuitsFrostprotectionOn(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsFrostprotection, circuit)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public decimal GetHeatingCircuitsHeatingCurveShift(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsHeatingCurve, circuit)?.Shift?.Value ?? 0m;
        }

        public decimal GetHeatingCircuitsHeatingCurveSlope(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsHeatingCurve, circuit)?.Slope?.Value ?? 0m;
        }

        public bool IsHeatingCircuitsHeatingScheduleActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsHeatingSchedule, circuit)?.Active?.Value ?? false;
        }

        public Schedule GetHeatingCircuitsHeatingSchedule(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsHeatingSchedule, circuit)?.Entries?.Value ?? new Schedule();
        }

        public string GetHeatingCircuitsOperatingModesActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingModesActive, circuit)?.Value?.Value?.ToString() ?? string.Empty;
        }

        public bool IsHeatingCircuitsOperatingModesDhwActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingModesDhw, circuit)?.Active?.Value ?? false;
        }

        public bool IsHeatingCircuitsOperatingModesDhwAndHeatingActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingModesDhwAndHeating, circuit)?.Active?.Value ?? false;
        }

        public bool IsHeatingCircuitsOperatingModesForcedNormalActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingModesForcedNormal, circuit)?.Active?.Value ?? false;
        }

        public bool IsHeatingCircuitsOperatingModesForcedReducedActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingModesForcedReduced, circuit)?.Active?.Value ?? false;
        }

        public bool IsHeatingCircuitsOperatingModesStandbyActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingModesStandby, circuit)?.Active?.Value ?? false;
        }

        public string GetHeatingCircuitsOperatingProgramsActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsActive, circuit)?.Value?.Value?.ToString() ?? string.Empty;
        }

        public bool IsHeatingCircuitsOperatingProgramsComfortActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsComfort, circuit)?.Active?.Value ?? false;
        }

        public decimal GetHeatingCircuitsOperatingProgramsComfortTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsComfort, circuit)?.Temperature?.Value ?? 0m;
        }

        public bool IsHeatingCircuitsOperatingProgramsEcoActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsEco, circuit)?.Active?.Value ?? false;
        }

        public decimal GetHeatingCircuitsOperatingProgramsEcoTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsEco, circuit)?.Temperature?.Value ?? 0m;
        }

        public bool IsHeatingCircuitsOperatingProgramsExternalActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsExternal, circuit)?.Active?.Value ?? false;
        }

        public decimal GetHeatingCircuitsOperatingProgramsExternalTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsExternal, circuit)?.Temperature?.Value ?? 0m;
        }

        public bool IsHeatingCircuitsOperatingProgramsHolidayActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsHoliday, circuit)?.Active?.Value ?? false;
        }

        public string GetHeatingCircuitsOperatingProgramsHolidayStart(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsHoliday, circuit)?.Start?.Value ?? string.Empty;
        }

        public string GetHeatingCircuitsOperatingProgramsHolidayEnd(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsHoliday, circuit)?.End?.Value ?? string.Empty;
        }

        public bool IsHeatingCircuitsOperatingProgramsHolidayAtHomeActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsHolidayAtHome, circuit)?.Active?.Value ?? false;
        }

        public string GetHeatingCircuitsOperatingProgramsHolidayAtHomeStart(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsHolidayAtHome, circuit)?.Start?.Value ?? string.Empty;
        }

        public string GetHeatingCircuitsOperatingProgramsHolidayAtHomeEnd(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsHolidayAtHome, circuit)?.End?.Value ?? string.Empty;
        }

        public bool IsHeatingCircuitsOperatingProgramsNormalActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsNormal, circuit)?.Active?.Value ?? false;
        }

        public decimal GetHeatingCircuitsOperatingProgramsNormalTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsNormal, circuit)?.Temperature?.Value ?? 0m;
        }

        public bool IsHeatingCircuitsOperatingProgramsReducedActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsReduced, circuit)?.Active?.Value ?? false;
        }

        public decimal GetHeatingCircuitsOperatingProgramsReducedTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsReduced, circuit)?.Temperature?.Value ?? 0m;
        }

        public bool IsHeatingCircuitsOperatingProgramsStandbyActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsOperatingProgramsStandby, circuit)?.Active?.Value ?? false;
        }

        public bool IsHeatingCircuitsSensorsTemperatureConnected(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsSensorsTemperature, circuit)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingCircuitsSensorsTemperature(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsSensorsTemperature, circuit)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingCircuitsSensorsTemperatureRoomConnected(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsSensorsTemperatureRoom, circuit)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingCircuitsSensorsTemperatureRoom(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsSensorsTemperatureRoom, circuit)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingCircuitsSensorsTemperatureSupplyConnected(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsSensorsTemperatureSupply, circuit)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingCircuitsSensorsTemperatureSupply(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsSensorsTemperatureSupply, circuit)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingCircuitsGeofencingActive(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsGeofencing, circuit)?.Active?.Value ?? false;
        }

        public string GetHeatingCircuitsGeofencingStatus(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCircuitsGeofencing, circuit)?.Status?.Value ?? string.Empty;
        }

        public bool IsHeatingConfigurationMultiFamilyHouseActive()
        {
            return GetProperties(FeatureName.Name.HeatingConfigurationMultiFamilyHouse)?.Active?.Value ?? false;
        }

        public string GetHeatingControllerSerial()
        {
            return GetProperties(FeatureName.Name.HeatingControllerSerial)?.Value?.Value?.ToString() ?? string.Empty;
        }

        public IEnumerable<Message> GetHeatingCoolingCircuitMessages(FeatureName.Circuit? circuit = FeatureName.Circuit.Circuit0)
        {
            return GetProperties(FeatureName.Name.HeatingCoolingCircuitsMessages, circuit)?.Messages?.Value ?? new List<Message>();
        }

        public int GetHeatingDeviceTimeOffset()
        {
            return GetProperties(FeatureName.Name.HeatingDeviceTimeOffset)?.ValueAsInt ?? 0;
        }

        public bool IsHeatingDhwActive()
        {
            return GetProperties(FeatureName.Name.HeatingDhw)?.Active?.Value ?? false;
        }

        public bool IsHeatingDhwChargingActive()
        {
            return GetProperties(FeatureName.Name.HeatingDhwCharging)?.Active?.Value ?? false;
        }

        public bool IsHeatingDhwOneTimeChargeActive()
        {
            return GetProperties(FeatureName.Name.HeatingDhwOneTimeCharge)?.Active?.Value ?? false;
        }

        public bool IsHeatingDhwPumpsCirculationOn()
        {
            return GetProperties(FeatureName.Name.HeatingDhwPumpsCirculation)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool IsHeatingDhwPumpsCirculationScheduleActive()
        {
            return GetProperties(FeatureName.Name.HeatingDhwPumpsCirculationSchedule)?.Active?.Value ?? false;
        }

        public Schedule GetHeatingDhwPumpsCirculationSchedule()
        {
            return GetProperties(FeatureName.Name.HeatingDhwPumpsCirculationSchedule)?.Entries?.Value ?? new Schedule();
        }

        public bool IsHeatingDhwPumpsPrimaryOn()
        {
            return GetProperties(FeatureName.Name.HeatingDhwPumpsPrimary)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public bool IsHeatingDhwScheduleActive()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSchedule)?.Active?.Value ?? false;
        }

        public Schedule GetHeatingDhwSchedule()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSchedule)?.Entries?.Value ?? new Schedule();
        }

        public bool IsHeatingDhwSensorsTemperatureHotWaterStorageConnected()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSensorsTemperatureHotWaterStorage)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingDhwSensorsTemperatureHotWaterStorage()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSensorsTemperatureHotWaterStorage)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingDhwSensorsTemperatureHotWaterStorageConnectedTop()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSensorsTemperatureHotWaterStorageTop)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingDhwSensorsTemperatureHotWaterStorageTop()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSensorsTemperatureHotWaterStorageTop)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingDhwSensorsTemperatureHotWaterStorageConnectedBottom()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSensorsTemperatureHotWaterStorageBottom)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingDhwSensorsTemperatureHotWaterStorageBottom()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSensorsTemperatureHotWaterStorageBottom)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingDhwSensorsTemperatureOutletConnected()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSensorsTemperatureOutlet)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingDhwSensorsTemperatureOutlet()
        {
            return GetProperties(FeatureName.Name.HeatingDhwSensorsTemperatureOutlet)?.ValueAsDouble ?? double.NaN;
        }

        public double GetHeatingDhwTemperature()
        {
            return GetProperties(FeatureName.Name.HeatingDhwTemperature)?.ValueAsDouble ?? double.NaN;
        }

        public double GetHeatingDhwTemperatureMain()
        {
            return GetProperties(FeatureName.Name.HeatingDhwTemperatureMain)?.ValueAsDouble ?? double.NaN;
        }

        public string GetHeatingFuelCellOperatingModesActive()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellOperatingModesActive)?.Value?.Value?.ToString() ?? string.Empty;
        }

        public bool IsHeatingFuelCellOperatingModesEcologicalActive()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellOperatingModesEcological)?.Active?.Value ?? false;
        }

        public bool IsHeatingFuelCellOperatingModesEconomicalActive()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellOperatingModesEconomical)?.Active?.Value ?? false;
        }

        public bool IsHeatingFuelCellOperatingModesHeatControlledActive()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellOperatingModesHeatControlled)?.Active?.Value ?? false;
        }

        public bool IsHeatingFuelCellOperatingModesMaintenanceActive()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellOperatingModesMaintenance)?.Active?.Value ?? false;
        }

        public bool IsHeatingFuelCellOperatingModesStandbyActive()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellOperatingModesStandby)?.Active?.Value ?? false;
        }

        public string GetHeatingFuelCellOperatingPhase()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellOperatingPhase)?.Value?.Value?.ToString() ?? string.Empty;
        }

        public IEnumerable<double> GetHeatingFuelCellPowerProduction(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellPowerProduction)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public bool IsHeatingFuelCellSensorsTemperatureReturnConnected()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellSensorsTemperatureReturn)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingFuelCellSensorsTemperatureReturn()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellSensorsTemperatureReturn)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingFuelCellSensorsTemperatureSupplyConnected()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellSensorsTemperatureSupply)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingFuelCellSensorsTemperatureSupply()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellSensorsTemperatureSupply)?.ValueAsDouble ?? double.NaN;
        }

        public decimal GetHeatingFuelCellStatisticsOperationHours()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellStatistics)?.OperationHours?.Value ?? 0m;
        }

        public decimal GetHeatingFuelCellStatisticsProductionHours()
        {
            return GetProperties(FeatureName.Name.HeatingFuelCellStatistics)?.ProductionHours?.Value ?? 0m;
        }

        public IEnumerable<double> GetHeatingGasConsumptionDhw(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingGasConsumptionDhw)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public IEnumerable<double> GetHeatingGasConsumptionFuelCell(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingGasConsumptionFuelCell)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public IEnumerable<double> GetHeatingGasConsumptionHeating(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingGasConsumptionHeating)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public IEnumerable<double> GetHeatingGasConsumptionTotal(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingGasConsumptionTotal)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public IEnumerable<double> GetHeatingHeatProduction(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingHeatProduction)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public double GetHeatingPowerCumulativeProduced()
        {
            return GetProperties(FeatureName.Name.HeatingPowerCumulativeProduced)?.ValueAsDouble ?? 0d;
        }

        public double GetHeatingPowerCumulativePurchased()
        {
            return GetProperties(FeatureName.Name.HeatingPowerCumulativePurchased)?.ValueAsDouble ?? 0d;
        }

        public double GetHeatingPowerCumulativeSold()
        {
            return GetProperties(FeatureName.Name.HeatingPowerCumulativeSold)?.ValueAsDouble ?? 0d;
        }

        public IEnumerable<double> GetHeatingPowerProduction(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingPowerProduction)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public double GetHeatingPowerProductionCumulative()
        {
            return GetProperties(FeatureName.Name.HeatingPowerProductionCumulative)?.ValueAsDouble ?? 0d;
        }

        public double GetHeatingPowerProductionCurrent()
        {
            return GetProperties(FeatureName.Name.HeatingPowerProductionCurrent)?.ValueAsDouble ?? 0d;
        }

        public double GetHeatingPowerProductionDemandCoverageCurrent()
        {
            return GetProperties(FeatureName.Name.HeatingPowerProductionDemandCoverageCurrent)?.ValueAsDouble ?? 0d;
        }

        public IEnumerable<double> GetHeatingPowerProductionDemandCoverageTotal(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingPowerProductionDemandCoverageTotal)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public double GetHeatingPowerProductionProductionCoverageCurrent()
        {
            return GetProperties(FeatureName.Name.HeatingPowerProductionProductionCoverageCurrent)?.ValueAsDouble ?? 0d;
        }

        public IEnumerable<double> GetHeatingPowerProductionProductionCoverageTotal(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingPowerProductionProductionCoverageTotal)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public double GetHeatingPowerPurchaseCumulative()
        {
            return GetProperties(FeatureName.Name.HeatingPowerPurchaseCumulative)?.ValueAsDouble ?? 0d;
        }

        public double GetHeatingPowerPurchaseCurrent()
        {
            return GetProperties(FeatureName.Name.HeatingPowerPurchaseCurrent)?.ValueAsDouble ?? 0d;
        }

        public double GetHeatingPowerSoldCumulative()
        {
            return GetProperties(FeatureName.Name.HeatingPowerSoldCumulative)?.ValueAsDouble ?? 0d;
        }

        public bool IsHeatingSensorsPowerOutputConnected()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsPowerOutput)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingSensorsPowerOutput()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsPowerOutput)?.ValueAsDouble ?? 0d;
        }

        public bool IsHeatingSensorsPressureSupplyConnected()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsPressureSupply)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingSensorsPressureSupply()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsPressureSupply)?.ValueAsDouble ?? 0d;
        }

        public bool IsHeatingSensorsTemperatureHydraulicSeparatorConnected()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsTemperatureHydraulicSeparator)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingSensorsTemperatureHydraulicSeparator()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsTemperatureHydraulicSeparator)?.ValueAsDouble ?? 0d;
        }

        public bool IsHeatingSensorsTemperatureReturnConnected()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsTemperatureReturn)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingSensorsTemperatureReturn()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsTemperatureReturn)?.ValueAsDouble ?? 0d;
        }

        public bool IsHeatingSensorsVolumetricFlowReturnConnected()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsVolumetricFlowReturn)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingSensorsVolumetricFlowReturn()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsVolumetricFlowReturn)?.ValueAsDouble ?? 0d;
        }

        public bool IsHeatingSensorsTemperatureOutsideConnected()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsTemperatureOutside)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingSensorsTemperatureOutside()
        {
            return GetProperties(FeatureName.Name.HeatingSensorsTemperatureOutside)?.ValueAsDouble ?? double.NaN;
        }

        public bool GetHeatingServiceDue()
        {
            return GetProperties(FeatureName.Name.HeatingServiceTimeBased)?.ServiceDue?.Value ?? false;
        }

        public int GetHeatingServiceIntervalMonths()
        {
            return GetProperties(FeatureName.Name.HeatingServiceTimeBased)?.ServiceIntervalMonths?.Value ?? 0;
        }

        public int GetHeatingActiveMonthSinceLastService()
        {
            return GetProperties(FeatureName.Name.HeatingServiceTimeBased)?.ActiveMonthSinceLastService?.Value ?? 0;
        }

        public string GetHeatingLastService()
        {
            return GetProperties(FeatureName.Name.HeatingServiceTimeBased)?.LastService?.Value ?? string.Empty;
        }

        public bool IsHeatingSolarActive()
        {
            return GetProperties(FeatureName.Name.HeatingSolar)?.Active?.Value ?? false;
        }

        public int GetHeatingSolarPowerProductionWhToday()
        {
            return (int)(GetHeatingSolarPowerProduction().FirstOrDefault() * 1000d);
        }

        public IEnumerable<double> GetHeatingSolarPowerProduction(Resolution resolution = Resolution.Day)
        {
            return GetProperties(FeatureName.Name.HeatingSolarPowerProduction)?.SeriesByResolution(resolution)?.Value ?? new List<double>();
        }

        public string GetHeatingSolarPowerProductionUnit()
        {
            return GetProperties(FeatureName.Name.HeatingSolarPowerProduction)?.Unit?.Value ?? string.Empty;
        }

        public bool IsHeatingSolarPumpsCircuitOn()
        {
            return GetProperties(FeatureName.Name.HeatingSolarPumpsCircuit)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public decimal GetHeatingSolarStatisticsHours()
        {
            return GetProperties(FeatureName.Name.HeatingSolarStatistics)?.Hours?.Value ?? 0m;
        }

        public bool IsHeatingSolarSensorsTemperatureDhwConnected()
        {
            return GetProperties(FeatureName.Name.HeatingSolarSensorsTemperatureDhw)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingSolarSensorsTemperatureDhw()
        {
            return GetProperties(FeatureName.Name.HeatingSolarSensorsTemperatureDhw)?.ValueAsDouble ?? double.NaN;
        }

        public bool IsHeatingSolarSensorsTemperatureCollectorConnected()
        {
            return GetProperties(FeatureName.Name.HeatingSolarSensorsTemperatureCollector)?.Status?.Value?.Equals("connected", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }

        public double GetHeatingSolarSensorsTemperatureCollector()
        {
            return GetProperties(FeatureName.Name.HeatingSolarSensorsTemperatureCollector)?.ValueAsDouble ?? double.NaN;
        }

        public double GetHeatingSolarPowerCumulativeProduced()
        {
            return GetProperties(FeatureName.Name.HeatingSolarPowerCumulativeProduced)?.ValueAsDouble ?? 0d;
        }

        public bool IsHeatingSolarRechargeSuppressionOn()
        {
            return GetProperties(FeatureName.Name.HeatingSolarRechargeSuppression)?.Status?.Value?.Equals("on", StringComparison.InvariantCultureIgnoreCase) ?? false;
        }
    }
}