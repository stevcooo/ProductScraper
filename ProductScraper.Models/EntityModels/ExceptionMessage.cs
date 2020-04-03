using System;

namespace ProductScraper.Models.EntityModels
{
    public class ExceptionMessage
    {
        public long Id { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public string AditionalInfo { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime DateTime { get; set; }
    }
}
