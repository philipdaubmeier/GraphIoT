namespace PhilipDaubmeier.ViessmannClient.Model
{
    public interface IWiremessage<T> where T : class
    {
        T? Data { get; }
        PagingCursor? Cursor { get; }
    }
}