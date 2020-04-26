namespace PhilipDaubmeier.ViessmannClient.Model
{
    public class Wiremessage<T> : IWiremessage<T> where T : class
    {
        public T? Data { get; set; }
        public PagingCursor? Cursor { get; set; }
    }
}