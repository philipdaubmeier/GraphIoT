namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    public interface IFunctionExpression : IGraphiteExpression
    {
        IGraphiteExpression InnerExpression { get; }
    }
}