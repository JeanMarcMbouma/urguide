using System;

namespace UrGuide.Data.Entities.Regions
{
    public class PaymentMethod
    {
        public string PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public string Secret2 { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Enabled { get; set; }

    }
}
