using System;

namespace test_parser
{
    public class SearchWoodDealModel
    {
        public string DeclarationNumber { get; set; }
        public string TraderName { get; set; }
        public string TraderInn { get; set; }
        public string CustomerName { get; set; }
        public string CustomerInn { get; set; }
        public double WoodVolumeTrader { get; set; }
        public double WoodVolumeCustomer { get; set; }
        public DateTime DealDate { get; set; }
    }
}