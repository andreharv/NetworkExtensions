using System;
using System.Linq;

namespace Transit.Framework
{
    public static class StringExtensions
    {
        public static string TrimStart(this string inputText, string toTrim)
        {
            if (inputText.StartsWith(toTrim))
            {
                return inputText.Substring(toTrim.Length, inputText.Length - toTrim.Length);
            }
            else
            {
                return inputText;
            }
        }

        public static string TrimEnd(this string inputText, string toTrim)
        {
            if (inputText.EndsWith(toTrim))
            {
                return inputText.Substring(0, inputText.LastIndexOf(toTrim, StringComparison.Ordinal));
            }
            else
            {
                return inputText;
            }
        }
    }
}
