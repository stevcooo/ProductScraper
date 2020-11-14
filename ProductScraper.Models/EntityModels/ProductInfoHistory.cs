using System;

namespace ProductScraper.Models.EntityModels
{
    public class ProductInfoHistory : AzureTableEntity
    {
        public long Id { get; set; }
        public string Price { get; set; }
        public DateTime Date { get; set; }

        public ProductInfoHistory()
        {
            Date = DateTime.Now;
        }

        public virtual ProductInfo ProductInfo { get; set; }
        public virtual long ProductInfoId { get; set; }        
    }
}