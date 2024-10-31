using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WbData
{
    public class Indicadores
    {
        public static string connectionString = "Data Source=localhost;Initial Catalog=wb;Integrated Security=True;";

        public DataTable obtemDadadosGDP() { }

        public DataTable obtemDadadosGDP(string ContryCode) { }

        public DataTable obtemDadadosGDP(string[] Countries) { }

        public bool CriaTabela(string crountryCode) { }

      static async Task CriaBancoDedados(string selectedCountry)
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

            string tableName = $"GDP_Data_{selectedCountry.Name.Replace(" ", "_")}";

                // Drop existing table
                string dropTableQuery = $"IF OBJECT_ID('{tableName}', 'U') IS NOT NULL DROP TABLE {tableName};";
                using (SqlCommand dropCommand = new SqlCommand(dropTableQuery, connection))
                {
                    dropCommand.ExecuteNonQuery();
                    Console.WriteLine($"Tabela '{tableName}' removida com sucesso.");
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
                    Console.WriteLine($"Tabela '{tableName}' criada com sucesso.");
                }

                // Get GDP data from World Bank API
                string apiUrl = $"https://api.worldbank.org/v2/country/{selectedCountry}/indicator/NY.GDP.MKTP.CD?date={startYear}:{endYear}&format=json";
                Console.WriteLine("Conectando no webService");

                //teste

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();
                        JArray jsonData = JArray.Parse(jsonResponse);
                        Console.WriteLine("Carregando os Dados");

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
