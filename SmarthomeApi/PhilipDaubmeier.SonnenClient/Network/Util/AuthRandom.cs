using System;
using System.Linq;

namespace PhilipDaubmeier.SonnenClient.Network
{
    /// <summary>
    /// Utility class that derives from Random and allows for generating
    /// random strings for nonces and code verifiers, following
    /// RFC 7636 - "Proof Key for Code Exchange by OAuth Public Clients"
    /// </summary>
    public class AuthRandom : Random
    {
        private const string charsVerifier = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
        private const string charsNonce = "abcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// Initializes a new instance of the AuthRandom class, using a time-dependent
        /// default seed value.
        /// </summary>
        public AuthRandom() : base() { }

        /// <summary>
        /// Initializes a new instance of the AuthRandom class, using the specified seed value.
        /// </summary>
        /// <param name="Seed">A number used to calculate a starting value for the pseudo-random number sequence.
        /// If a negative number is specified, the absolute value of the number is used.</param>
        public AuthRandom(int Seed) : base(Seed) { }

        /// <summary>
        /// Creates a 128 characters code verifier containing the chars [A-Za-z0-9-_]
        /// </summary>
        public string GenerateCodeVerifier()
        {
            return GenerateString(charsVerifier, 128);
        }

        /// <summary>
        /// Creates an 11 characters nonce containing chars [a-z0-9]
        /// </summary>
        public string GenerateNonce()
        {
            return GenerateString(charsNonce, 11);
        }

        private string GenerateString(string chars, int length)
        {
            return new string(Enumerable.Repeat(chars, length).Select(s => s[Next(s.Length)]).ToArray());
        }
    }
}