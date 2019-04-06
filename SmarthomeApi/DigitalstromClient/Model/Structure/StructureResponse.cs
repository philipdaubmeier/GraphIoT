namespace DigitalstromClient.Model.Structure
{
    public class StructureResponse : IWiremessagePayload<StructureResponse>
    {
        public Apartment apartment { get; set; }
    }
}
