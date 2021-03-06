﻿using System;
using System.Collections.Generic;

namespace ProductScraper.Models.EntityModels
{
    public class UserProfile : AzureTableEntity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual IList<ProductInfo> ProductInfos { get; set; }
        public DateTime LastNotificationEmailSendOn { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public int DaysBetweenEmailNotifications { get; set; }
        public bool SendEmailWhenNoProductHasBeenChanged { get; set; }
    }
}
