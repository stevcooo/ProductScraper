﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ProductScraper.Models.EntityModels
{
    public class ProductInfo : AzureTableEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }        
        [Required]
        [Url]
        public string URL { get; set; }
        public string Price { get; set; }
        public string SecondPrice { get; set; }
        public bool? Availability { get; set; }
        public DateTime LastCheckedOn { get; set; }
        public bool HasChangesSinceLastTime { get; set; }

        public virtual UserProfile UserProfile { get; set; }
        public virtual int UserProfileId { get; set; }

        public ProductInfo()
        {
            Id = DateTime.Now.Ticks;
            RowKey = Id.ToString();
            LastCheckedOn = DateTime.Now;
        }
    }
}
