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
        public bool IsValid(SearchWoodDealJson json)
        {
            bool buyerNameValid = !string.IsNullOrWhiteSpace(json.buyerName);
            bool buyerInnLengthValid = (json.buyerInn.Length > 5);
            bool buyerInnIsNumber = IsNumber(json.buyerInn);
            
            bool sellerNameValid = !string.IsNullOrWhiteSpace(json.sellerName);
            bool sellerInnLengthValid = (json.sellerInn.Length > 5);
            bool sellerInnIsNumber = IsNumber(json.sellerInn);
            
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
            if (DateTime.TryParse(jsonDealDate, out Temp) == true &&
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