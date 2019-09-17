namespace PhilipDaubmeier.DigitalstromClient.Model
{
    public interface IDeepCloneable<T>
    {
        T DeepClone();
    }
}