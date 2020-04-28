using System;

namespace ProductScraper.Helpers
{
    public static class StringHelper
    {
        public static string RemoveSpecialCharacters(this string input)
        {
            if (String.IsNullOrEmpty(input))
                return String.Empty;
            
            input = input.Replace("\"", "\'");

            return input;
        }
    }
}
