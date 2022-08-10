using System;

namespace test_parser
{
    public static class Extensions
    {
        public static SearchWoodDealModel ToDataBaseModel(this SearchWoodDealJson json)
        {
            return new SearchWoodDealModel()
            {
                CustomerInn = Convert.ToInt64(string.IsNullOrEmpty(json.buyerInn) ? "0" : json.buyerInn),
                CustomerName = json.buyerName,
                TraderInn = Convert.ToInt64(string.IsNullOrEmpty(json.sellerInn) ? "0" : json.sellerInn),
                TraderName = json.sellerName,
                DeclarationNumber = json.dealNumber,
                WoodVolumeCustomer = Math.Abs(json.woodVolumeBuyer - default(double)) > 0.1 ? json.woodVolumeBuyer : default,
                WoodVolumeTrader = Math.Abs(json.woodVolumeSeller - default(double)) > 0.1 ? json.woodVolumeSeller : default,
                DealDate = DateTime.Parse(json.dealDate),
                LastUpdated = DateTime.Now
            };
        }
    }
}