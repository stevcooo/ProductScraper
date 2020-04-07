namespace ProductScraper.Models.EntityModels
{
    public class ScrapeConfig : AzureTableEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string ProductNamePath { get; set; }
        public string ProductPricePath { get; set; }
        public string ProductSecondPricePath { get; set; }
        public string ProductAvailabilityPath { get; set; }
        public string ProductAvailabilityValue { get; set; }
        public bool ProductAvailabilityIsAtributeValue { get; set; }
    }
}
