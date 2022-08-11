using System;
using System.Globalization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace test_parser
{
    public class SearchWoodModelValidaror
    {
        private readonly bool _strictValidation;

        public SearchWoodModelValidaror(Config config)
        {
            _strictValidation = config.EnableStrictValidation;
        }
        public bool ValidateJson(SearchWoodDealJson json)
        {
            bool buyerNameValid = !string.IsNullOrWhiteSpace(json.buyerName);
            bool sellerNameValid = !string.IsNullOrWhiteSpace(json.sellerName);
            
            bool buyerInnLengthValid = true;
            bool buyerInnIsNumber = true;
            bool sellerInnLengthValid = true;
            bool sellerInnIsNumber = true;
            if (_strictValidation)
            {
                if (!(json.sellerName.Contains("Физическое лицо") && string.IsNullOrWhiteSpace(json.sellerInn)))
                {
                    sellerInnIsNumber = IsNumber(json.buyerInn);
                    sellerInnLengthValid = (json.buyerInn.Length > 5);
                }
                
                if (!(json.buyerName.Contains("Физическое лицо") && string.IsNullOrWhiteSpace(json.buyerInn)))
                {
                    buyerInnIsNumber = IsNumber(json.buyerInn);
                    buyerInnLengthValid = (json.buyerInn.Length > 5);
                }
            }
            bool dealDateValid = ValidateDate(json.dealDate);

            if (buyerNameValid &&
                buyerInnLengthValid &&
                buyerInnIsNumber &&
                sellerNameValid &&
                sellerInnLengthValid &&
                sellerInnIsNumber &&
                dealDateValid)
            {
                return true;
            }
            Console.WriteLine($"[Validator][Task-{Task.CurrentId}] Deal {json.dealNumber} in not valid " +
                              $"\n Value:{JsonConvert.SerializeObject(json)}");
            return false;
        }

        private bool ValidateDate(string jsonDealDate)
        {
            DateTime Temp;
            if (DateTime.TryParse(jsonDealDate, out Temp) &&
                Temp.Year > 1900 &&
                Temp > DateTime.MinValue &&
                Temp < DateTime.Now)
                return (true);
            return (false);
        }

        private bool IsNumber(string number)
        {
            try
            {
                Convert.ToDouble(number);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}