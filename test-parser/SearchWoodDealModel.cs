using System;

namespace test_parser
{
    public class SearchWoodDealModel
    {
        public string DeclarationNumber { get; set; }
        public string TraderName { get; set; }
        public long TraderInn { get; set; }
        public string CustomerName { get; set; }
        public long CustomerInn { get; set; }
        public double WoodVolumeTrader { get; set; }
        public double WoodVolumeCustomer { get; set; }
        public DateTime DealDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || ! this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else {
                SearchWoodDealModel p = (SearchWoodDealModel) obj;
                return (DeclarationNumber == p.DeclarationNumber) &&
                       (TraderName == p.TraderName) &&
                       (TraderInn == p.TraderInn) &&
                       (CustomerName == p.CustomerName) &&
                       (CustomerInn == p.CustomerInn) &&
                       (Math.Abs(WoodVolumeTrader - p.WoodVolumeTrader) < 0.1) &&
                       (Math.Abs(WoodVolumeCustomer - p.WoodVolumeCustomer) < 0.1) &&
                       (DealDate == p.DealDate);
            }
        }
    }
}