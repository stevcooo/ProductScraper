namespace ProductScraper.Models.EntityModels
{
    public class Ad: AzureTableEntity
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
    }
}