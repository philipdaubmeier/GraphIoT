namespace PhilipDaubmeier.WeConnectClient.Model.Capabilities
{
    internal class CapabilitiesResponse : CapabilityList, IWiremessage<CapabilityList>
    {
        public string ErrorCode => string.Empty;

        public bool HasError => false;

        public virtual CapabilityList Body => this;
    }
}