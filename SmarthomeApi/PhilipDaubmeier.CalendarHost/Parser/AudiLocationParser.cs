using System.Text.RegularExpressions;

namespace PhilipDaubmeier.CalendarHost.FormatParsers
{
    /// <summary>
    /// Specialized parser for recognizing essential location information
    /// of an Audi appointment by splitting up known patterns or trying known parts.
    /// </summary>
    public class AudiLocationParser
    {
        private readonly Regex buildingRoomPattern = new Regex(@"^(AUDI (IN|[A-Z]{2})( R|) ((BZ|TZ|VKR|[A-Z0-9]{1,3}) |)|)(([A-Za-z0-9_ -]+)\/([A-Za-z0-9_ -]+))($| .*$|\(.*$)");
        private readonly Regex telephonePattern = new Regex(@"^.*?(\+(49|)[\d-\/\(\) p#]{5,40}).*$");
        private readonly Regex conferencePattern = new Regex(@"^.*?(Skype|Lync|Phone|ruf(e|t)|(Tel|Web)(C|c|k)o|Online|Online(-|)(B|b)esprechung).*$");

        /// <summary>
        /// Extracts short location information of an Audi appointment.
        /// 
        /// Examples:
        ///     AUDI IN R BZ A03/Shopfloor (0) (I/PI-FM45)
        ///     AUDI IN R BZ A51/MFC (1) (I/VD-31I4)
        ///     AUDI IN R BZ GVZ Halle O/A239 (12) Beamer, DK (I/BT-62)
        ///     AUDI IN R S40/A122 Gesundheitsraum (10) B (I/PK-14)
        ///     AUDI IN R TZ A56/334 (10) Projektraum (I/PG)
        /// 
        /// Those will be parsed by the regex seen below in the code and extract the
        /// building (e.g. "S40" or "GVZ Halle O") and the room (e.g. "Shopfloor" oder "A239").
        /// If this pattern is recognized, the return will be "<building>/<room>" (e.g. "A51/MFC").
        /// 
        /// If, on the other hand, a telefone number is recognized, it will return the number,
        /// stripped from all surrounding text.
        /// 
        /// Examples:
        ///     Phone Number: +49 841 89 9710 / Code: 637907
        ///     +49841899730p5014088#
        ///     +49-5361-96802663, Konferenzkennung: 49342383#
        /// 
        /// Lastly, it will try to find some hint that points toward a telefone conference call,
        /// in which case it will return a fixed string "Telko" to denote a conf call.
        /// 
        /// Examples:
        ///     WebCo/TelCo
        ///     WebConf
        ///     Skype-Besprechung
        ///     Onlinebesprechung / Skype
        ///     
        /// If no pattern is found whatsoever, it just truncates the end result to the max length.
        /// 
        /// The returned string is guaranteed to be no longer than 32 characters.
        /// </summary>
        public string ToShortLocation(string longLocation)
        {
            if (string.IsNullOrEmpty(longLocation))
                return longLocation;

            GroupCollection groups;

            if ((groups = buildingRoomPattern.Match(longLocation)?.Groups)?.Count > 8)
                return EnsureShortLocation($"{groups[7]}/{groups[8]}");

            if ((groups = telephonePattern.Match(longLocation)?.Groups)?.Count > 1)
                return EnsureShortLocation($"{groups[1]}");

            if (conferencePattern.IsMatch(longLocation))
                return "Telko";

            return EnsureShortLocation(longLocation);
        }

        /// <summary>
        /// Strips control chars from beginning and end and t
        /// runcates the string to the max length required by "short location"
        /// </summary>
        private string EnsureShortLocation(string value)
        {
            return Truncate(value.Trim(' ', '-', '/', '(', ','), 32);
        }

        private string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
    }
}
