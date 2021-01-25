using Microsoft.EntityFrameworkCore;

namespace UrGuide.Data.Entities.Regions
{
    [Owned]
    public class RegionFlags
    {
        public bool Active { get; set; }
        public bool CanRaiseTourRequests { get; set; }
        public bool CanMakePayments { get; set; }
        public bool CanMakeReservations { get; set; }
        public bool CanRegisterUsers { get; set; }
    }
}
