namespace ProductScraper.Models.ViewModels
{
    public class EmailMessage
    {
        public string UserId { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public EmailMessage()
        {

        }

        public EmailMessage(string to, string subject, string content)
        {
            To = to;
            Subject = subject;
            Content = content;
        }
    }
}
