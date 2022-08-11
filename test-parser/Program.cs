using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace test_parser
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {
            AsyncMain(args).GetAwaiter().GetResult();
        }

        public static async Task AsyncMain(string[] args)
        {
            Console.WriteLine("LESEGAIS DUMPER BY MAXIM BELYY (KITSUNOFF)");
            string configstring = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "config.json");
            Config config = JsonConvert.DeserializeObject<Config>(configstring);
            if (config == null)
                throw new ArgumentNullException("config.json is empty");
            
            SearchDealWoodRequester requester = new SearchDealWoodRequester(config);
            SearchDealRepository repository = new SearchDealRepository(config);
            int dealCount = await requester.GetDealCountsAsync();
            int pagesCount = dealCount / config.RecordCountPerRequest;
            
            while (true)
            {
                for (int page = 0; page < pagesCount; page++)
                {
                    try
                    {
                        var responce = await requester.ParseSearchWoodDealAsync(page);
                        await repository.InsertOrUpdateManyAsync(responce);
                        await Task.Delay(TimeSpan.FromSeconds(config.RequestDelay));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }
                }
                Console.WriteLine("Parsing ended. Waiting 10 minutes");
                await Task.Delay(TimeSpan.FromMinutes(10));
            }
        }
    }
}