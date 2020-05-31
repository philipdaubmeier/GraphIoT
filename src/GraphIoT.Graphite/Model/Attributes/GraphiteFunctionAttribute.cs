using System;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GraphiteFunctionAttribute : Attribute
    {
        public string Name { get; }
        public string Group { get; }

        public GraphiteFunctionAttribute(string name, string group) => (Name, Group) = (name, group);
    }
}