using System;
using System.Text.RegularExpressions;

namespace test_parser
{
    public static class Extensions
    {
        public static SearchWoodDealModel ToDataBaseModel(this SearchWoodDealJson json)
        {
            var normalizedCustomerInn = string.IsNullOrEmpty(json.buyerInn) ? "0" : json.buyerInn.RemoveNotNumbers();
            var normalizedTraderInn = string.IsNullOrEmpty(json.sellerInn) ? "0" : json.sellerInn.RemoveNotNumbers();
            return new SearchWoodDealModel()
            {
                CustomerInn = Convert.ToInt64(normalizedCustomerInn),
                CustomerName = json.buyerName,
                TraderInn = Convert.ToInt64(normalizedTraderInn),
                TraderName = json.sellerName,
                DeclarationNumber = json.dealNumber,
                WoodVolumeCustomer = Math.Abs(json.woodVolumeBuyer - default(double)) > 0.1 ? json.woodVolumeBuyer : default,
                WoodVolumeTrader = Math.Abs(json.woodVolumeSeller - default(double)) > 0.1 ? json.woodVolumeSeller : default,
                DealDate = DateTime.Parse(json.dealDate),
                LastUpdated = DateTime.Now
            };
        }

        public static string RemoveNotNumbers(this string str)
        {
            return  Regex.Replace(str, "[^0-9]", "");
        }
    }
}