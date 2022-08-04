using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        public bool DealExistInDb(string declarationNumber)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM searchreportwooddeal WHERE declaration_number = @id";
                    command.Parameters.AddWithValue("@id", declarationNumber);
                    MySqlDataReader dataReader = command.ExecuteReader();
                    if (dataReader.HasRows)
                    {
                        connection.Close();
                        return true;
                    }
                    connection.Close();
                    return false;
                }
            }
            
        }
        
        public async Task InsertOrUpdateManyAsync(IEnumerable<SearchWoodDealJson> jsons)
        {
            foreach (var json in jsons)
                if (_validator.IsValid(json))
                {
                    Console.WriteLine($"[Repository][Task-{Task.CurrentId}]Adding new deal {json.dealNumber}");
                        await InsertOrUpdatePrivateAsync(json.ToDataBaseModel());
                }
        }
        
         private async Task InsertOrUpdatePrivateAsync(SearchWoodDealModel model)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "INSERT INTO searchreportwooddeal " +
                        "(declaration_number, trader_name, trader_inn, customer_name, customer_inn, wood_volume_trader, wood_volume_customer, deal_date) " +
                        "VALUE (@dec_number, @t_name, @t_inn, @c_name, @c_inn, @wood_v_t,@wood_v_c, @deal_date)" +
                        "ON DUPLICATE KEY UPDATE " +
                        "declaration_number = @dec_number," +
                        "trader_name = @t_name," +
                        "trader_inn = @t_inn," +
                        "customer_name = @c_name," +
                        "customer_inn = @c_inn," +
                        "wood_volume_trader = @wood_v_t," +
                        "wood_volume_customer = @wood_v_c," +
                        "deal_date=@deal_date";
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
                await connection.CloseAsync();
                Console.WriteLine($"[Repository][Task-{Task.CurrentId}]Adding deal {model.DeclarationNumber} ended");
            }
        }
    }
}