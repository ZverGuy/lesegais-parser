using System;

namespace test_parser
{
    public class SearchWoodModelValidaror
    {
        public bool IsValid(SearchWoodDealJson json)
        {
            bool sellerNameValid = !string.IsNullOrWhiteSpace(json.sellerName);
            bool buyerNameValid = !string.IsNullOrWhiteSpace(json.buyerName);
            bool sellerInnLengthValid = (json.sellerInn.Length > 5);
            bool dealDateValid = ValidateDate(json.dealDate);
            if(sellerNameValid && buyerNameValid && sellerInnLengthValid && dealDateValid)
                return true;
            return false;
        }

        private bool ValidateDate(string jsonDealDate)
        {
            DateTime Temp;
            if (DateTime.TryParse(jsonDealDate, out Temp) == true &&
                Temp.Year > 1900 &&
                Temp > DateTime.MinValue)
                return (true);
            return (false);
        }
    }
}