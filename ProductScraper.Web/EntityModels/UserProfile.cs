using System.Collections.Generic;

namespace ProductScraper.Web.EntityModels
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual IList<ProductInfo> ProductInfos { get; set; }
    }
}
