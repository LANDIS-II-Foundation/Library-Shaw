using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Landis.Extension.ShawDamm
{
    public class SimpleFileParser
    {
        public static double AbsZero = -273.15;

        private static Regex _whiteSpaceSplitter = new Regex(@"\s+");

        private List<string[]> _inputLines;

        public SimpleFileParser(string filePath, out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                _inputLines = File.ReadAllLines(filePath).Where(x => !x.StartsWith("#")).Select(x => _whiteSpaceSplitter.Split(x)).ToList();
            }
            catch (Exception e)
            {
                errorMessage = $"Unable to open file '{filePath}'";
            }
        }

        /// <summary>
        /// Tries to check if a token exists.  Returns false and sets errorMessage if duplicate tokens are found.  Returns true otherwise.  
        /// Sets containsToken if the token is found.  Does NOT check that the token has a value.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="containsToken">if set to <c>true</c> [contains token].</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public bool TryContainsToken(string token, out bool containsToken, out string errorMessage)
        {
            containsToken = false;
            string[] match;
            bool isMissing;
            if (!FindTokenMatch(token, true, out match, out isMissing, out errorMessage))
                return false;

            containsToken = !isMissing;
            return true;               
        }

        /// <summary>
        /// Parses the value for the token, with optional range checking.  Returns false and populates errorMessage if duplicate tokens are found, 
        /// the token or value is missing or commented out, or the value is outside the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The token.</param>
        /// <param name="value">The value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="lowerRange">The lower range.</param>
        /// <param name="upperRange">The upper range.</param>
        /// <param name="lowerInclusive">if set to <c>true</c> [lower inclusive].</param>
        /// <param name="upperInclusive">if set to <c>true</c> [upper inclusive].</param>
        /// <returns></returns>
        public bool TryParse<T>(string token, out T value, out string errorMessage, double lowerRange = double.NegativeInfinity, bool lowerInclusive = false, double upperRange = double.PositiveInfinity, bool upperInclusive = false)
        {
            bool isMissing;
            if (!TryParseOptional(token, out value, out errorMessage, out isMissing))
                return false;

            if (isMissing)
            {
                errorMessage = $"Missing value for token '{token}'";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Tries to parse the value for the token, with optional range checking.  Returns true and sets isMissing to true if the token or value is missing or commented out.
        /// Returns false and populates errorMessage if duplicate tokens are found, or a found value is outside the range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token">The token.</param>
        /// <param name="value">The value.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="isMissing">if set to <c>true</c> [is missing].</param>
        /// <param name="lowerRange">The lower range.</param>
        /// <param name="upperRange">The upper range.</param>
        /// <param name="lowerInclusive">if set to <c>true</c> [lower inclusive].</param>
        /// <param name="upperInclusive">if set to <c>true</c> [upper inclusive].</param>
        /// <returns></returns>
        public bool TryParseOptional<T>(string token, out T value, out string errorMessage, out bool isMissing, double lowerRange = double.NegativeInfinity, bool lowerInclusive = false, double upperRange = double.PositiveInfinity, bool upperInclusive = false)
        {
            value = default(T);
            isMissing = false;

            string[] match;
            if (!FindTokenMatch(token, true, out match, out isMissing, out errorMessage))
                return false;

            if (isMissing)
                return true;

            if (match.Length < 2 || string.IsNullOrEmpty(match[1]) || match[1].StartsWith("#"))
            {
                isMissing = true;
                return true;
            }

            if (typeof(T) == typeof(string))
            {
                value = (T)Convert.ChangeType(match[1], typeof(T));
                return true;
            }

            if (typeof(T) == typeof(double))
            {
                double t;
                if (!double.TryParse(match[1], out t))
                {
                    errorMessage = $"Cannot parse '{match[1]}' as double for token '{token}'";
                    return false;
                }

                if (!CheckRange(token, t, lowerRange, upperRange, lowerInclusive, upperInclusive, out errorMessage))
                    return false;

                value = (T)Convert.ChangeType(t, typeof(T));
                return true;
            }

            if (typeof(T) == typeof(int))
            {
                int t;
                if (!int.TryParse(match[1], out t))
                {
                    errorMessage = $"Cannot parse '{match[1]}' as int for token '{token}'";
                    return false;
                }

                if (!CheckRange(token, t, lowerRange, upperRange, lowerInclusive, upperInclusive, out errorMessage))
                    return false;

                value = (T)Convert.ChangeType(t, typeof(T));
                return true;
            }

            if (typeof(T) == typeof(bool))
            {
                bool t;
                if (!bool.TryParse(match[1], out t))
                {
                    errorMessage = $"Cannot parse '{match[1]}' as bool for token '{token}'";
                    return false;
                }

                value = (T)Convert.ChangeType(t, typeof(T));
                return true;
            }

            errorMessage = $"Unrecognized Type '{typeof(T)}' requested for token '{token}'";

            return false;
        }

        private bool CheckRange(string token, double t, double lowerRange, double upperRange, bool lowerInclusive, bool upperInclusive, out string errorMessage)
        {
            errorMessage = string.Empty;

            var inRange = t > lowerRange && t < upperRange;

            inRange |= lowerInclusive && t == lowerRange;
            inRange |= upperInclusive && t == upperRange;

            if (!inRange)
            {
                errorMessage = $"Value '{t}' for token '{token}' is out of the range: {(lowerInclusive ? "[" : "(")}{lowerRange}, {upperRange}{(upperInclusive ? "]" : ")")}";
                return false;
            }

            return true;
        }

        private bool FindTokenMatch(string token, bool allowMissing, out string[] match, out bool isMissing, out string errorMessage)
        {
            errorMessage = string.Empty;
            isMissing = false;
            match = null;

            var matches = this._inputLines.Where(x => x[0].Equals(token, StringComparison.OrdinalIgnoreCase)).ToList();

            if (matches.Count > 1)
            {
                errorMessage = $"Multiple lines match for token '{token}'";
                return false;
            }

            if (!matches.Any())
            {
                isMissing = true;
                if (allowMissing)
                    return true;

                errorMessage = $"Cannot find token '{token}'";
                return false;
            }

            match = matches[0];
            return true;
        }
    }
}
