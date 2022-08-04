using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace test_parser
{
    public class SearchDealRepository
    {
        private readonly string _connectionString;
        private readonly SearchWoodModelValidaror _validator;

        public SearchDealRepository(string connectionString)
        {
            _connectionString = connectionString;
            _validator = new SearchWoodModelValidaror();
        }

        public bool TryGetDeal(MySqlConnection connection,string declarationNumber, out SearchWoodDealModel model)
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
                        CustomerInn = dataReader.GetString("customer_inn") ?? string.Empty,
                        WoodVolumeTrader = dataReader.GetFloat("wood_volume_trader"),
                        WoodVolumeCustomer = dataReader.GetFloat("wood_volume_customer"),
                        DealDate = dataReader.GetDateTime("deal_date"),
                    };
                    connection.Close();
                    return true;
                }
                model = null;
                return false;
            }
        }

        public async Task InsertNewDealAsync(SearchWoodDealModel model, MySqlConnection connection)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "INSERT INTO searchreportwooddeal " +
                    "VALUES (@dec_number, @t_name, @t_inn, @c_name, @c_inn,@wood_v_t,@wood_v_c,@deal_date)";
                command.Parameters.AddWithValue("@dec_number", model.DeclarationNumber);
                command.Parameters.AddWithValue("@t_name", model.TraderName);
                command.Parameters.AddWithValue("@t_inn", model.TraderInn);
                command.Parameters.AddWithValue("@c_name", model.CustomerName);
                command.Parameters.AddWithValue("@c_inn", model.CustomerInn);
                command.Parameters.AddWithValue("@wood_v_t", model.WoodVolumeTrader);
                command.Parameters.AddWithValue("@wood_v_c", model.WoodVolumeCustomer);
                command.Parameters.AddWithValue("@deal_date", model.DealDate);
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateDealAsync(SearchWoodDealModel model, MySqlConnection connection)
        {
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE searchreportwooddeal " +
                                      "SET trader_name = @t_name," +
                                      "trader_inn = @t_inn," +
                                      "customer_name = @c_name," +
                                      "customer_inn = @c_inn," +
                                      "wood_volume_trader = @wood_v_t," +
                                      "wood_volume_customer = @wood_v_c," +
                                      "deal_date = @deal_date " +
                                      "WHERE declaration_number = @dec_number";
                command.Parameters.AddWithValue("@dec_number", model.DeclarationNumber);
                command.Parameters.AddWithValue("@t_name", model.TraderName);
                command.Parameters.AddWithValue("@t_inn", model.TraderInn);
                command.Parameters.AddWithValue("@c_name", model.CustomerName);
                command.Parameters.AddWithValue("@c_inn", model.CustomerInn);
                command.Parameters.AddWithValue("@wood_v_t", model.WoodVolumeTrader);
                command.Parameters.AddWithValue("@wood_v_c", model.WoodVolumeCustomer);
                command.Parameters.AddWithValue("@deal_date", model.DealDate);
                await command.ExecuteNonQueryAsync();
            }
        }
        


        public async Task InsertOrUpdateManyAsync(IEnumerable<SearchWoodDealJson> jsons)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                foreach (var json in jsons)
                {
                    if (_validator.isValid(json))
                    {
                        if (!TryGetDeal(connection, json.dealNumber, out var model))
                        {
                            await InsertNewDealAsync(json.ToDataBaseModel(), connection);
                        }
                        else
                        {
                            await UpdateDealAsync(json.ToDataBaseModel(), connection);
                        }
                    }
                }

                await connection.CloseAsync();
            }
        }
    }
}