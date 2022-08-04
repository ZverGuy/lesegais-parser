using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace test_parser
{
    public class RequestQuery
    {
        [JsonProperty("query")]
        public string Query { get; set; }
        [JsonProperty("variables")]
        public JObject Variables { get; set; }
        [JsonProperty("operationName", NullValueHandling=NullValueHandling.Ignore)]
        public string OperationName { get; set; }

        public RequestQuery(string query, (string, object)[] variables, string operationName = null)
        {
            Query = query;
            Variables = new JObject();
            foreach (var variable in variables)
            {
                Variables.Add(variable.Item1, new JValue(variable.Item2));
            }

            OperationName = operationName;
        }
    }
}