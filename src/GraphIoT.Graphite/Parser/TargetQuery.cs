using System.Linq;
using System.Text.RegularExpressions;

namespace PhilipDaubmeier.GraphIoT.Graphite.Parser
{
    public class TargetQuery
    {
        private Regex TargetRegex { get; set; }

        public TargetQuery(string wildcardPattern)
        {
            TargetRegex = new Regex("^" + Regex.Escape(wildcardPattern).Replace(@"\*", "(.*?)") + "$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        public bool IsMatch(string target) => TargetRegex.IsMatch(target);

        public (bool expandable, string segment) NextSegment(string target)
        {
            var segment = TargetRegex.Matches(target).First().Groups.Cast<Group>().Last().Value;
            return (segment.Contains('.'), segment.Split('.').FirstOrDefault() ?? string.Empty);
        }
    }
}