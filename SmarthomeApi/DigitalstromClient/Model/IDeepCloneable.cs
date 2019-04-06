namespace DigitalstromClient.Model
{
    public interface IDeepCloneable<T>
    {
        T DeepClone();
    }
}