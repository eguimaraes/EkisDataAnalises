using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WbData
{
    public class Indicadores
    {
        public static string connectionString = "Data Source=localhost;Initial Catalog=wb;Integrated Security=True;";

        public DataTable dataTable;

        public DataTable GetData(string Country="*")
        {
            DataTable dataTable = new DataTable();
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = $@"SELECT {Country} FROM [wb].[dbo].[GDP_Join]";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        public DataTable obtemDadadosGDP() {
                
        return GetData();
        
        }

        public void setDataTable() { dataTable=GetData();}

        public DataTable obtemDadadosGDP(string CountryCode) {
              
        
        return GetData(CountryCode);
        
        }

        
        public void CriaViewJoin() {
        
        
        
        
        
        }

        public void CriaDB()
        {





        }

        static async Task CriaTabela(string selectedCountry)
        {
           
            var countries = new List<Country>
            {
                new Country("Brazil", "BR"),
                new Country("Argentina", "AR"),
                new Country("United States", "US"),
                new Country("China", "CN"),
                new Country("Japan", "JP"),
                new Country("Germany", "DE"),
                new Country("India", "IN"),
                new Country("United Kingdom", "GB"),
                new Country("France", "FR"),
                new Country("Italy", "IT"),
                new Country("Canada", "CA"),
                new Country("South Korea", "KR"),
                new Country("Australia", "AU"),
                new Country("Spain", "ES"),
                new Country("Mexico", "MX"),
                new Country("Indonesia", "ID"),
                new Country("Netherlands", "NL"),
                new Country("Saudi Arabia", "SA"),
                new Country("Turkey", "TR"),
                new Country("Taiwan", "TW"),
                new Country("Switzerland", "CH")
            };

           // Find the country based on user input
                        
            int startYear = 1989;
            int endYear = 2023;
            
            // Connect to SQL Database and create table for the selected country
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

            string tableName = $"GDP_Data_{selectedCountry}";

                // Drop existing table
                string dropTableQuery = $"IF OBJECT_ID('{tableName}', 'U') IS NOT NULL DROP TABLE {tableName};";
                using (SqlCommand dropCommand = new SqlCommand(dropTableQuery, connection))
                {
                    dropCommand.ExecuteNonQuery();
                   
                }

                // Create new table
                string createTableQuery = $@"
                CREATE TABLE {tableName} (
                    Id INT IDENTITY(1,1),
                    CountryCode NVARCHAR(3),
                    GDP DECIMAL(18, 2),
                    Year INT PRIMARY KEY 
                );";
                using (SqlCommand createCommand = new SqlCommand(createTableQuery, connection))
                {
                    createCommand.ExecuteNonQuery();
                   
                }

                // Get GDP data from World Bank API
                string apiUrl = $"https://api.worldbank.org/v2/country/{selectedCountry}/indicator/NY.GDP.MKTP.CD?date={startYear}:{endYear}&format=json";
              

                //teste

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JArray jsonData = JArray.Parse(jsonResponse);
                       

                        foreach (var item in jsonData[1])
                        {
                            string countryCode = item["countryiso3code"].ToString();
                            string gdp = item["value"]?.ToString() ?? "0";
                            string year = item["date"].ToString();

                            // Insert data into SQL table
                            string insertQuery = $@"
                            INSERT INTO {tableName} (CountryCode, GDP, Year)
                            VALUES (@CountryCode, @GDP, @Year)";
                            using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@CountryCode", countryCode);
                                insertCommand.Parameters.AddWithValue("@GDP", decimal.Parse(gdp));
                                insertCommand.Parameters.AddWithValue("@Year", int.Parse(year));

                                insertCommand.ExecuteNonQuery();
                            }
                        }

                      
                    }
                    else
                    {
                       
                    }
                }
            }
        }


    }
}
