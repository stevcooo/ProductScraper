using System;

namespace ProductScraper.Services.Exceptions
{
    public class ScrapeServiceException : Exception
    {
        public ScrapeServiceException() : base()
        {

        }

        public ScrapeServiceException(string message) : base(message)
        {

        }

        public ScrapeServiceException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
