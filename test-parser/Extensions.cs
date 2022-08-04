using System;

namespace test_parser
{
    public static class Extensions
    {
        public static SearchWoodDealModel ToDataBaseModel(this SearchWoodDealJson json)
        {
            return new SearchWoodDealModel()
            {
                CustomerInn = json.buyerInn,
                CustomerName = json.buyerName,
                TraderInn = json.sellerInn,
                TraderName = json.sellerName,
                DeclarationNumber = json.dealNumber,
                WoodVolumeCustomer = json.woodVolumeBuyer != default ? json.woodVolumeBuyer : default,
                WoodVolumeTrader = json.woodVolumeSeller != default ? json.woodVolumeSeller : default,
                DealDate = DateTime.Parse(json.dealDate)
            };
        }
    }
}