using System.Collections.Generic;

namespace UrGuide.Model.Analytics
{
    public class ConversionFunnel
    {
        public int TourRequests { get; set; }
        public int BidsReceived { get; set; }
        public int BidsAccepted { get; set; }
        public int BookingsCreated { get; set; }
        public int BookingsCompleted { get; set; }
        public List<ConversionStage> Stages { get; set; } = new List<ConversionStage>();
    }

    public class ConversionStage
    {
        public string StageName { get; set; }
        public int Count { get; set; }
        public decimal ConversionRate { get; set; }
    }
}
