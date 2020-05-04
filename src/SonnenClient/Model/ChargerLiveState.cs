using System;

namespace PhilipDaubmeier.SonnenClient.Model
{
    public class ChargerLiveState
    {
        public DateTime? MeasuredAt { get; set; }
        public double ActivePower { get; set; }
        public double Current { get; set; }
        public double ChargeSpeedKmh { get; set; }
        public string Car { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double? MaxChargeCurrent { get; set; }
        public bool Smartmode { get; set; }
        public DateTime? DepartureAt { get; set; }
        public double TransactionChargedKm { get; set; }
        public double TotalChargedKm { get; set; }
        public double TotalChargedEnergy { get; set; }
        public string ChargingBehaviour { get; set; } = string.Empty;
        public bool ChargingAuthorizationRequired { get; set; }
    }
}