﻿namespace PhilipDaubmeier.WeConnectClient.Model.VehicleInfo
{
    internal class VehicleDetailsResponse : VehicleDetails, IWiremessage<VehicleDetails>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        public virtual VehicleDetails Body => this;
    }
}