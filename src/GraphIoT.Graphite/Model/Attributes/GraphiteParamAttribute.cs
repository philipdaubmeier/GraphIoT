using System;
using System.Collections.Generic;
using System.Linq;

namespace PhilipDaubmeier.GraphIoT.Graphite.Model
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class GraphiteParamAttribute : Attribute
    {
        public string Name { get; }
        public string Type { get; }
        public bool Required { get; }
        public IList<string> Suggestions { get; }

        public GraphiteParamAttribute(string name, string type, bool required, string? suggestions = null)
            => (Name, Type, Required, Suggestions) = (name, type, required, ToSuggestionList(suggestions));

        private static IList<string> ToSuggestionList(string? suggestions)
            => suggestions?.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Distinct().ToList() ?? new List<string>();
    }
}