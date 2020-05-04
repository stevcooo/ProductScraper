using System;
using System.ComponentModel.DataAnnotations;

namespace ProductScraper.Models.EntityModels
{
    public class ProductInfo : AzureTableEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PreviousName { get; set; }
        [Required]
        [Url]
        public string URL { get; set; }
        public string Price { get; set; }
        public string PreviousPrice { get; set; }
        public string Currency { get; set; }        
        public bool? Availability { get; set; }
        public bool? PreviousAvailability { get; set; }
        public DateTime LastCheckedOn { get; set; }
        public bool HasChangesSinceLastTime { get; set; }

        public virtual UserProfile UserProfile { get; set; }
        public virtual long UserProfileId { get; set; }        

        public ProductInfo()
        {
            LastCheckedOn = DateTime.Now;
        }
    }
}
