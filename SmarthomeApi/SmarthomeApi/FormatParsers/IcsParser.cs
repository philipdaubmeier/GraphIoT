using SmarthomeApi.Database.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmarthomeApi.FormatParsers
{
    public class IcsParser
    {
        private static class TextReaderEnumerator
        {
            public static IEnumerable<string> EnumerateLines(TextReader reader)
            {
                while (reader.Peek() != -1)
                    yield return reader.ReadLine();
            }
        }

        private PersistenceContext _dbContext;
        private string _owner;

        public IcsParser(PersistenceContext databaseContext, string ownerEmail)
        {
            _dbContext = databaseContext;
            _owner = ownerEmail;
        }

        public async Task Parse(string file)
        {
            await Parse(file.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public async Task Parse(TextReader stream)
        {
            await Parse(TextReaderEnumerator.EnumerateLines(stream));
        }

        public async Task Parse(IEnumerable<string> lines)
        {
            await Task.Factory.StartNew(() =>
            {
                StoreCalendarEntries(_dbContext, GroupTokens(Tokenize(PreprocessMultilines(lines))));
            });
        }

        private IEnumerable<string> PreprocessMultilines(IEnumerable<string> lines)
        {
            StringBuilder lastLine = new StringBuilder();
            bool first = true;
            foreach (var line in lines)
            {
                if (line.Length == 0 || line.Trim().Length == 0)
                    continue;

                if (line[0] == '\t')
                {
                    lastLine.Append(line, 1, line.Length - 1);
                    continue;
                }

                if (!first)
                    yield return lastLine.ToString();
                first = false;
                lastLine.Clear();

                lastLine.Append(line);
            }

            var last = lastLine.ToString();
            if (last.Trim().Length > 0)
                yield return last;
        }

        private IEnumerable<KeyValuePair<Token, string>> Tokenize(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                var splitted = line.Split(new char[] { ':', ';' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (splitted.Count() < 2)
                    continue;

                var token = FindToken(splitted[0]);
                if (token == Token.Unknown)
                    continue;

                yield return new KeyValuePair<Token, string>(token, splitted[1].Trim());
            }
        }

        private enum Token
        {
            Unknown,
            BEGIN,
            CLASS,
            DTEND,
            DTSTAMP,
            DTSTART,
            END,
            EXDATE,
            METHOD,
            PRODID,
            RDATE,
            RECURRENCE_ID,
            RRULE,
            SEQUENCE,
            SUMMARY,
            TRANSP,
            TZID,
            TZOFFSETFROM,
            TZOFFSETTO,
            UID,
            VERSION,
            X_CALEND,
            X_CALSTART,
            X_CLIPEND,
            X_CLIPSTART,
            X_MICROSOFT_CDO_BUSYSTATUS,
            X_MS_OLK_WKHRDAYS,
            X_MS_OLK_WKHREND,
            X_MS_OLK_WKHRSTART,
            X_OWNER,
            X_PRIMARY_CALENDAR,
            X_WR_CALNAME,
            X_WR_RELCALID
        }

        private Token FindToken(string token)
        {
            switch (token.Trim().ToUpperInvariant())
            {
                case "BEGIN": return Token.BEGIN;
                case "CLASS": return Token.CLASS;
                case "DTEND": return Token.DTEND;
                case "DTSTAMP": return Token.DTSTAMP;
                case "DTSTART": return Token.DTSTART;
                case "END": return Token.END;
                case "EXDATE": return Token.EXDATE;
                case "METHOD": return Token.METHOD;
                case "PRODID": return Token.PRODID;
                case "RDATE": return Token.RDATE;
                case "RECURRENCE-ID": return Token.RECURRENCE_ID;
                case "RRULE": return Token.RRULE;
                case "SEQUENCE": return Token.SEQUENCE;
                case "SUMMARY": return Token.SUMMARY;
                case "TRANSP": return Token.TRANSP;
                case "TZID": return Token.TZID;
                case "TZOFFSETFROM": return Token.TZOFFSETFROM;
                case "TZOFFSETTO": return Token.TZOFFSETTO;
                case "UID": return Token.UID;
                case "VERSION": return Token.VERSION;
                case "X-CALEND": return Token.X_CALEND;
                case "X-CALSTART": return Token.X_CALSTART;
                case "X-CLIPEND": return Token.X_CLIPEND;
                case "X-CLIPSTART": return Token.X_CLIPSTART;
                case "X-MICROSOFT-CDO-BUSYSTATUS": return Token.X_MICROSOFT_CDO_BUSYSTATUS;
                case "X-MS-OLK-WKHRDAYS": return Token.X_MS_OLK_WKHRDAYS;
                case "X-MS-OLK-WKHREND": return Token.X_MS_OLK_WKHREND;
                case "X-MS-OLK-WKHRSTART": return Token.X_MS_OLK_WKHRSTART;
                case "X-OWNER": return Token.X_OWNER;
                case "X-PRIMARY-CALENDAR": return Token.X_PRIMARY_CALENDAR;
                case "X-WR-CALNAME": return Token.X_WR_CALNAME;
                case "X-WR-RELCALID": return Token.X_WR_RELCALID;
                default: return Token.Unknown;
            }
        }

        private enum TokenGroup
        {
            Metadata,
            CalendarEntry
        }

        private IEnumerable<Tuple<TokenGroup, IDictionary<Token, string>>> GroupTokens(IEnumerable<KeyValuePair<Token, string>> tokens)
        {
            var grouped = new Dictionary<Token, string>();
            var grouptype = TokenGroup.Metadata;
            var stack = new Stack<string>();
            var calendar = tokens.SkipWhile(x => x.Key != Token.BEGIN || !x.Value.Equals("VCALENDAR", StringComparison.InvariantCultureIgnoreCase)).Skip(1);
            foreach (var item in calendar)
            {
                if (item.Key == Token.BEGIN)
                {
                    stack.Push(item.Value);

                    if (!item.Value.Equals("VEVENT", StringComparison.InvariantCultureIgnoreCase))
                        continue;

                    yield return new Tuple<TokenGroup, IDictionary<Token, string>>(grouptype, grouped);
                    grouptype = TokenGroup.CalendarEntry;
                    grouped = new Dictionary<Token, string>();
                }
                else if (item.Key == Token.END && stack.Count > 0)
                {
                    stack.Pop();
                    if (item.Value.Equals("VCALENDAR", StringComparison.InvariantCultureIgnoreCase))
                        yield break;
                }
                else if ((stack.Count == 0 && grouptype == TokenGroup.Metadata) ||
                    (stack.Count > 0 && stack.Peek().Equals("VEVENT", StringComparison.InvariantCultureIgnoreCase)))
                {
                    grouped.Add(item.Key, item.Value);
                }
            }
            if (grouped.Count > 0)
                yield return new Tuple<TokenGroup, IDictionary<Token, string>>(grouptype, grouped);
        }

        private DateTime ToDateTime(string icsTimeString)
        {
            throw new NotImplementedException();
        }

        private int ToBusyState(string icsBusyString)
        {
            switch (icsBusyString.Trim().ToUpperInvariant())
            {
                case "TENTATIVE": return 1;
                case "OOF": return 2;
                default: return 0;
            }
        }

        private void StoreCalendarEntries(PersistenceContext db, IEnumerable<Tuple<TokenGroup, IDictionary<Token, string>>> groupedTokens)
        {
            string calendarUuid, owner;
            Calendar dbCalendar = null;
            foreach (var entry in groupedTokens)
            {
                if (entry.Item1 == TokenGroup.Metadata && dbCalendar == null)
                {
                    var metadata = entry;
                    if (metadata.Item2.Count == 0)
                        return;

                    if (!metadata.Item2.TryGetValue(Token.X_WR_RELCALID, out calendarUuid) || string.IsNullOrWhiteSpace(calendarUuid))
                        return;

                    if (!metadata.Item2.TryGetValue(Token.X_OWNER, out owner) || owner == null || !owner.ToLowerInvariant().EndsWith($"mailto:{_owner.ToLowerInvariant()}"))
                        return;

                    dbCalendar = db.Calendars.Where(x => x.UUID == calendarUuid).FirstOrDefault();
                    if (dbCalendar == null)
                        db.Calendars.Add(dbCalendar = new Calendar() { UUID = calendarUuid, Owner = owner });

                    continue;
                }

                ParseCalendarEntry(dbCalendar, entry);
            }

            db.SaveChanges();
        }

        private void ParseCalendarEntry(Calendar dbCalendar, Tuple<TokenGroup, IDictionary<Token, string>> entry)
        {
            if (entry.Item1 != TokenGroup.CalendarEntry)
                return;

            var uid = entry.Item2[Token.UID];
            var isPrivate = !entry.Item2[Token.CLASS].Equals("PUBLIC", StringComparison.InvariantCultureIgnoreCase);
            var isFullDay = entry.Item2[Token.DTSTART].ToUpperInvariant().StartsWith("VALUE=DATE:");
            var startTime = ToDateTime(entry.Item2[Token.DTSTART]);
            var endTime = ToDateTime(entry.Item2[Token.DTEND]);
            var modified = ToDateTime(entry.Item2[Token.DTSTAMP]);
            var recurranceRule = entry.Item2[Token.RRULE];
            var summary = entry.Item2[Token.SUMMARY];
            var busyState = ToBusyState(entry.Item2[Token.X_MICROSOFT_CDO_BUSYSTATUS]);

            var dbEntry = dbCalendar.Entries.Where(x => x.UID == uid).FirstOrDefault();
            if (dbEntry == null)
                dbCalendar.Entries.Add(dbEntry = new CalendarEntry());

            dbEntry.UID = uid;
            dbEntry.IsPrivate = isPrivate;
            dbEntry.IsFullDay = isFullDay;
            dbEntry.StartTime = startTime;
            dbEntry.EndTime = endTime;
            dbEntry.Modified = modified;
            dbEntry.RecurranceRule = recurranceRule;
            dbEntry.Summary = summary;
            dbEntry.BusyState = busyState;
        }
    }
}
