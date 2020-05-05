using System;

namespace PhilipDaubmeier.GraphIoT.Core.Database
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TimeSeriesAttribute : Attribute
    {
        public Type Type { get; }

        public TimeSeriesAttribute(Type type) => Type = type;
    }
}