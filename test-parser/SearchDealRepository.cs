using MySqlConnector;

namespace test_parser
{
    public class SearchDealRepository
    {
        private readonly string _connectionString;

        public SearchDealRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool TryGetDeal(string declarationNumber, out SearchWoodDealModel model)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM searchreportwooddeal WHERE declaration_number = @id";
                    command.Parameters.AddWithValue("id", declarationNumber);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        model = new SearchWoodDealModel()
                        {
                            DeclarationNumber = dataReader.GetString("declaration_number"),
                            TraderName = dataReader.GetString("trader_name"),
                            CustomerName = dataReader.GetString("customer_name"),
                            TraderInn = dataReader.GetString("trader_inn"),
                            CustomerInn = dataReader.GetString("customer_inn"),
                            WoodVolumeTrader = dataReader.GetFloat("wood_volume_trader"),
                            WoodVolumeCustomer = dataReader.GetFloat("wood_volume_customer"),
                            DealDate = dataReader.GetDateTime("deal_date"),
                        };
                        return true;
                    }
                    model = null;
                    return false;
                }
            }
        }


        public void InsertOrUpdate(SearchWoodDealJson json)
        {
            if(!TryGetDeal(json.dealNumber, out var model))
            {
                
            }
        }
    }
}