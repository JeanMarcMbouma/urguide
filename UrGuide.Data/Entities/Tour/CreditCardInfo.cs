using System;

namespace UrGuide.Data.Entities.Tour
{
    public class CreditCardInfo
    {
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public short ExpiryYear { get; set; }
        public byte ExpiryMonth { get; set; }

        public bool Expired => new DateTime(ExpiryYear, ExpiryMonth, 1) < new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    }

}
