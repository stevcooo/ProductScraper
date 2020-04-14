using System;

namespace ProductScraper.Models.ViewModels
{
    public class AzureTableSettings
    {
        public AzureTableSettings(string storageConnectionString, string tableName)
        {
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                throw new ArgumentNullException("StorageConnectionString");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("TableName");
            }

            StorageConnectionString = storageConnectionString;
            TableName = tableName;
        }

        public string TableName { get; }
        public string StorageConnectionString { get; set; }
    }
}
