using System.Collections.Generic;

namespace ProductScraper.Models.ViewModels
{
    public class EmailMessage
    {
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public EmailMessage(IEnumerable<string> to, string subject, string content)
        {
            To = new List<string>();
            To.AddRange(to);
            Subject = subject;
            Content = content;
        }

        public EmailMessage(string to, string subject, string content)
        {
            To = new List<string>();
            To.Add(to);
            Subject = subject;
            Content = content;
        }
    }
}
