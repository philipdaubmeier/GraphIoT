namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public class Command<TParams> : ICommand where TParams : class
    {
        public string? Uri { get; set; }
        public string? Name { get; set; }
        public bool? IsExecutable { get; set; }
        public TParams? @Params { get; set; }
    }
}