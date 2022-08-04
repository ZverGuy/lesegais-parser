using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace test_parser
{
    public class SearchDealWoodRequester
    {
        private readonly HttpClient _httpClient;
        private readonly int _recordsPerPage;
        private readonly int _maxRetries;
        private readonly float _requestDelay;
        private readonly int _dealCount;

        public SearchDealWoodRequester(Config config)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "insomnia/2022.4.2");
            _recordsPerPage = config.RecordCountPerRequest;
            _maxRetries = config.MaxRetries;
            _requestDelay = config.RequestDelay;
            _dealCount = config.DealCount;
        }


        public async Task<int> GetDealCountsAsync()
        {
            string query =
                "query SearchReportWoodDealCount($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {\n  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {\n    total\n    number\n    size\n    overallBuyerVolume\n    overallSellerVolume\n    __typename\n  }\n}\n";
            RequestQuery requestQuery = new RequestQuery(query, new (string, object)[]
            {
                new ValueTuple<string, object>("size", 20),
                new ValueTuple<string, object>("number", 0),
                new ValueTuple<string, object>("filter", null)
            }, "SearchReportWoodDealCount");
            var serializedQuery = JsonConvert.SerializeObject(requestQuery);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://www.lesegais.ru/open-area/graphql"),
                Content = new StringContent(serializedQuery)
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json"),
                    }
                },
            };
            var responce = await _httpClient.SendAsync(request);
            var datastring = await responce.Content.ReadAsStringAsync();
            JObject data = JObject.Parse(datastring);
            if (data["data"]["searchReportWoodDeal"]["total"] == null)
                return _dealCount;
            return data["data"]["searchReportWoodDeal"]["total"].Value<int>();
        }

        public async Task<SearchWoodDealJson[]> ParseSearchWoodDealAsync(int page, int trynumber = 0)
        {
            SearchWoodDealJson[] result;
            string query =
                "query SearchReportWoodDeal($size: Int!, $number: Int!, $filter: Filter, $orders: [Order!]) {  searchReportWoodDeal(filter: $filter, pageable: {number: $number, size: $size}, orders: $orders) {    content {      sellerName      sellerInn      buyerName      buyerInn      woodVolumeBuyer      woodVolumeSeller      dealDate      dealNumber      __typename    }    __typename  }}";
            RequestQuery requestQuery = new RequestQuery(query, new (string, object)[]
            {
                new ValueTuple<string, object>("size", _recordsPerPage),
                new ValueTuple<string, object>("number", page),
                new ValueTuple<string, object>("filter", null),
                new ValueTuple<string, object>("orders", null),
            });
            var serializedQuery = JsonConvert.SerializeObject(requestQuery);
            try
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://www.lesegais.ru/open-area/graphql"),
                    Content = new StringContent(serializedQuery)
                    {
                        Headers =
                        {
                            ContentType = new MediaTypeHeaderValue("application/json"),
                        }
                    },
                };
                var responce = await _httpClient.SendAsync(request);
                var datastring = await responce.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(datastring);
                var str = data["data"]["searchReportWoodDeal"]["content"].ToString();
                result = JsonConvert.DeserializeObject<SearchWoodDealJson[]>(str);
                Console.WriteLine($"[Parser][Task]Parsing Completed Successfully.");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                if (trynumber < _maxRetries)
                {
                    Console.WriteLine("Retring");
                    await Task.Delay(TimeSpan.FromSeconds(_requestDelay));
                    result = await ParseSearchWoodDealAsync(page, trynumber + 1);
                    return result;
                }
                throw;
            }
           
        }
    }
}