namespace PhilipDaubmeier.ViessmannClient.Model.Features
{
    public interface ICommand
    {
        string? Uri { get; set; }
        string? Name { get; set; }
        bool? IsExecutable { get; set; }
    }
}