using System;

namespace Transit.Framework
{
    public static class StringExtensions
    {
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
