namespace ProductScraper.Models.Extensions
{
    public static class StringExtensions
    {
        public static string ToCoreUrl(this string input)
        {
            string url = input;
            url = url.Replace("https://", "").Replace("http://", "").Replace("www.", "");
            int index = url.IndexOf("/");
            if (index > 0)
            {
                url = url.Substring(0, index);
            }

            return url;
        }
    }
}
