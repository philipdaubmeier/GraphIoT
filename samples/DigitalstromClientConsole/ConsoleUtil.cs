using PhilipDaubmeier.DigitalstromClient.Model.Core;
using System;

namespace PhilipDaubmeier.DigitalstromClientConsole
{
    public class ConsoleUtil
    {
        public static Uri ReadUri(string caption)
        {
            while (true)
            {
                Console.WriteLine(caption);
                var uriStr = Console.ReadLine();
                if (uriStr is null)
                    continue;
                try
                {
                    return new Uri(uriStr, UriKind.Absolute);
                }
                catch
                {
                    Console.WriteLine("Not a valid uri format!");
                }
            }
        }

        public static string ReadNonWhitespace(string caption)
        {
            while (true)
            {
                Console.WriteLine(caption);

                var str = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(str))
                    return str;

                Console.WriteLine("Must not be only blank characters!");
            }
        }

        public static string ReadPassword(string caption)
        {
            Console.WriteLine(caption);
            string password = string.Empty;
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                    continue;
                }

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(string.Empty);
                    return password;
                }

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password = password[0..^1];
                    Console.Write("\b \b");
                }
            }
        }

        public static int ReadPositiveInt(string caption)
        {
            while (true)
            {
                Console.WriteLine(caption);
                var str = Console.ReadLine();
                if (int.TryParse(str, out int result))
                    return result;

                Console.WriteLine($"Must be a number between 0 and {int.MaxValue}!");
            }
        }

        public static Zone ReadZone(string caption)
        {
            return ReadPositiveInt(caption);
        }

        public static Group ReadGroup(string caption)
        {
            return ReadPositiveInt(caption);
        }

        public static Scene ReadScene(string caption)
        {
            return ReadPositiveInt(caption);
        }
    }
}