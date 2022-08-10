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
        private bool _dataBaseIsEmpty;

        public SearchDealRepository(Config config)
        {
            _connectionString = config.ConnectionString;
            _validator = new SearchWoodModelValidaror(config);
            _dataBaseIsEmpty = DataBaseIsEmpty();
        }

        private bool DataBaseIsEmpty()
        {
            bool hasRecords = false;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(dec_number) FROM search_wood";
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        hasRecords = reader.GetInt64("COUNT(dec_number)") == 0;
                    }
                }
                connection.Close();
                
            }
            return hasRecords;
        }

        public bool DealExistInDb(string declarationNumber)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM search_wood WHERE dec_number = @id";
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
        
        public async Task InsertOrUpdateManyAsync(IEnumerable<SearchWoodDealJson> jsons, bool ignoreDealIfExist = true)
        {
            foreach (var json in jsons)
            {
                if (_validator.IsValid(json))
                {
                    if (!_dataBaseIsEmpty)
                    {
                        if (DealExistInDb(json.dealNumber) && ignoreDealIfExist)
                        {
                            Console.WriteLine($"[Repository][Task-{Task.CurrentId}]Deal {json.dealNumber} exist in db and ignoring enabled");
                            continue;
                        }
                    }
                    Console.WriteLine($"[Repository][Task-{Task.CurrentId}]Adding new deal {json.dealNumber}");
                    await InsertOrUpdatePrivateAsync(json.ToDataBaseModel());
                }
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
                        "INSERT INTO search_wood " +
                        "(dec_number, t_name, t_inn, c_name, c_inn, w_volume_t, w_volume_c, d_date) " +
                        "VALUE (@dec_number, @t_name, @t_inn, @c_name, @c_inn, @wood_v_t,@wood_v_c, @deal_date)" +
                        "ON DUPLICATE KEY UPDATE " +
                        "dec_number = @dec_number," +
                        "t_name = @t_name," +
                        "t_inn = @t_inn," +
                        "c_name = @c_name," +
                        "c_inn = @c_inn," +
                        "w_volume_t = @wood_v_t," +
                        "w_volume_c = @wood_v_c," +
                        "d_date=@deal_date";
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