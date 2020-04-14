using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace ProductScraper.Services.Helpers
{
    public static class SerializeHelper
    {
        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using StreamWriter sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true);
            using JsonTextWriter jtw = new JsonTextWriter(sw) { Formatting = Formatting.None };
            JsonSerializer js = new JsonSerializer();
            js.Serialize(jtw, value);
            jtw.Flush();
        }
    }
}
