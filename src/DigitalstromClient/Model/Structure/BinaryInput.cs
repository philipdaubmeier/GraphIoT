using PhilipDaubmeier.DigitalstromClient.Model.Core;

namespace PhilipDaubmeier.DigitalstromClient.Model.Structure
{
    public class BinaryInput
    {
        public Group TargetGroup { get; set; } = Color.White;
        public int InputType { get; set; }
        public int InputId { get; set; }
        public int State { get; set; }
    }
}