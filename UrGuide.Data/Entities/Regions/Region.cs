namespace UrGuide.Data.Entities.Regions
{
    public class Region
    {
        public string RegionId { get; set; }
        public string Name { get; set; }

        public virtual RegionFlags Flags { get; set; }
        public virtual RegionStats Stats { get; set; }
        public string CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Country Country { get; set; }
        public string TimelineId { get; set; }
        public virtual Timeline Timeline { get; set; }
        public virtual PaymentMethod PaymentMethod { get; set; }
        public string CountryId { get; set; }
        public string PaymentMethodId { get; set; }
    }
}
