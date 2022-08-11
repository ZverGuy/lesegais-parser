using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace test_parser
{
    public class SearchDealRepository
    {
        private readonly string _connectionString;
        private readonly SearchWoodModelValidaror _validator;
        public bool DataBaseIsEmpty;
        private readonly bool _enableUpdate;

        public SearchDealRepository(Config config)
        {
            _connectionString = config.ConnectionString;
            _enableUpdate = config.UpdateRecords;
            _validator = new SearchWoodModelValidaror(config);
            DataBaseIsEmpty = DataBaseIsEmptyCheck();
        }

        private bool DataBaseIsEmptyCheck()
        {
            bool hasRecords = false;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(dec_number) FROM db.db_schema.search_wood";
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        hasRecords = reader.GetInt32(0) == 0;
                    }
                }
                connection.Close();
                
            }
            return hasRecords;
        }

        public bool TryGetDeal(string decNumber, out SearchWoodDealModel model)
        {
            model = null;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM db.db_schema.search_wood WHERE dec_number = @dec_number";
                    command.Parameters.AddWithValue("@dec_number", decNumber);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        model = new SearchWoodDealModel()
                        {
                            DeclarationNumber = decNumber,
                            CustomerInn = reader.GetInt64(4),
                            CustomerName = reader.GetString(3),
                            DealDate = reader.GetDateTime(5),
                            LastUpdated = reader.GetDateTime(8),
                            TraderInn = reader.GetInt64(2),
                            TraderName = reader.GetString(1),
                            WoodVolumeCustomer = reader.GetDouble(7),
                            WoodVolumeTrader = reader.GetDouble(6)
                        };
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
            {
                if (!_validator.ValidateJson(json)) continue;
                if (TryGetDeal(json.dealNumber, out var model))
                {
                    if(!_enableUpdate) continue;
                    
                    Console.WriteLine(
                        $"[Repository][Task-{Task.CurrentId}]Deal {json.dealNumber} exist in db. Equals Checking...");
                    if (Equals(model, json.ToDataBaseModel()))
                    {
                        Console.WriteLine(
                            $"[Repository][Task-{Task.CurrentId}]Deal {json.dealNumber} does not changed, skip.");
                        continue;
                    }

                    await UpdatePrivateAsync(model);
                }
                else
                {
                    Console.WriteLine($"[Repository][Task-{Task.CurrentId}]Adding or Updating deal {json.dealNumber}");
                    await InsertPrivateAsync(json.ToDataBaseModel());
                }
            }
        }

        private async Task UpdatePrivateAsync(SearchWoodDealModel model)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"UPDATE db.db_schema.search_wood SET 
                                            t_name = @t_name,
                                            t_inn = @t_inn,
                                            c_name = @c_name,
                                            c_inn = @c_inn,
                                            w_volume_t = @wood_v_t,
                                            w_volume_c = @wood_v_c,
                                            d_date = @d_date,
                                            last_update = @last_update
                                            WHERE dec_number = @dec_number";
                    command.Parameters.AddWithValue("@dec_number", model.DeclarationNumber);
                    command.Parameters.AddWithValue("@t_name", model.TraderName);
                    command.Parameters.AddWithValue("@t_inn", model.TraderInn);
                    command.Parameters.AddWithValue("@c_name", model.CustomerName);
                    command.Parameters.AddWithValue("@c_inn", model.CustomerInn);
                    command.Parameters.AddWithValue("@wood_v_t", model.WoodVolumeTrader);
                    command.Parameters.AddWithValue("@wood_v_c", model.WoodVolumeCustomer);
                    command.Parameters.AddWithValue("@d_date", model.DealDate);
                    command.Parameters.AddWithValue("@last_update", model.LastUpdated);
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();
                Console.WriteLine($"[Repository][Task-{Task.CurrentId}]Updating deal {model.DeclarationNumber} ended");
            }
        }

        private async Task InsertPrivateAsync(SearchWoodDealModel model)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "INSERT INTO db.db_schema.search_wood " +
                        "(dec_number, t_name, t_inn, c_name, c_inn, w_volume_t, w_volume_c, d_date, last_update) " +
                        "VALUES (@dec_number, @t_name, @t_inn, @c_name, @c_inn, @wood_v_t,@wood_v_c, @deal_date, @last_update)";
                    
                    command.Parameters.AddWithValue("@dec_number", model.DeclarationNumber);
                    command.Parameters.AddWithValue("@t_name", model.TraderName);
                    command.Parameters.AddWithValue("@t_inn", model.TraderInn);
                    command.Parameters.AddWithValue("@c_name", model.CustomerName);
                    command.Parameters.AddWithValue("@c_inn", model.CustomerInn);
                    command.Parameters.AddWithValue("@wood_v_t", model.WoodVolumeTrader);
                    command.Parameters.AddWithValue("@wood_v_c", model.WoodVolumeCustomer);
                    command.Parameters.AddWithValue("@deal_date", model.DealDate);
                    command.Parameters.AddWithValue("@last_update", model.LastUpdated);
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();
                Console.WriteLine($"[Repository][Task-{Task.CurrentId}]Adding deal {model.DeclarationNumber} ended");
            }
        }
    }
}