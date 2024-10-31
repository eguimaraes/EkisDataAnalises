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

        public void CreateGDPView(string viewName = "[dbo].[GDP_Join]")
        {
            string viewQuery = $@"
            CREATE VIEW {viewName} AS
            SELECT        dbo.Anos.Year, dbo.GDP_Data_Argentina.GDP AS AR, dbo.GDP_Data_Australia.GDP AS AU, 
                          dbo.GDP_Data_Brazil.GDP AS BR, dbo.GDP_Data_Canada.GDP AS CA, dbo.GDP_Data_China.GDP AS CN, 
                          dbo.GDP_Data_France.GDP AS FR, dbo.GDP_Data_Germany.GDP AS DE, dbo.GDP_Data_India.GDP AS [IN], 
                          dbo.GDP_Data_Indonesia.GDP AS ID, dbo.GDP_Data_Italy.GDP AS IT, dbo.GDP_Data_Japan.GDP AS JP, 
                          dbo.GDP_Data_Mexico.GDP AS MX, dbo.GDP_Data_Netherlands.GDP AS NL, dbo.GDP_Data_Saudi_Arabia.GDP AS SA, 
                          dbo.GDP_Data_South_Korea.GDP AS KR, dbo.GDP_Data_Spain.GDP AS ES, dbo.GDP_Data_Switzerland.GDP AS CH, 
                          dbo.GDP_Data_United_States.GDP AS US, dbo.GDP_Data_United_Kingdom.GDP AS GB, dbo.GDP_Data_Turkey.GDP AS TR
            FROM            dbo.Anos 
            INNER JOIN      dbo.GDP_Data_Argentina ON dbo.Anos.Year = dbo.GDP_Data_Argentina.Year 
            INNER JOIN      dbo.GDP_Data_Australia ON dbo.Anos.Year = dbo.GDP_Data_Australia.Year 
            INNER JOIN      dbo.GDP_Data_Brazil ON dbo.Anos.Year = dbo.GDP_Data_Brazil.Year 
            INNER JOIN      dbo.GDP_Data_Canada ON dbo.Anos.Year = dbo.GDP_Data_Canada.Year 
            INNER JOIN      dbo.GDP_Data_China ON dbo.Anos.Year = dbo.GDP_Data_China.Year 
            INNER JOIN      dbo.GDP_Data_France ON dbo.Anos.Year = dbo.GDP_Data_France.Year 
            INNER JOIN      dbo.GDP_Data_Germany ON dbo.Anos.Year = dbo.GDP_Data_Germany.Year 
            INNER JOIN      dbo.GDP_Data_India ON dbo.Anos.Year = dbo.GDP_Data_India.Year 
            INNER JOIN      dbo.GDP_Data_Indonesia ON dbo.Anos.Year = dbo.GDP_Data_Indonesia.Year 
            INNER JOIN      dbo.GDP_Data_Italy ON dbo.Anos.Year = dbo.GDP_Data_Italy.Year 
            INNER JOIN      dbo.GDP_Data_Japan ON dbo.Anos.Year = dbo.GDP_Data_Japan.Year 
            INNER JOIN      dbo.GDP_Data_Mexico ON dbo.Anos.Year = dbo.GDP_Data_Mexico.Year 
            INNER JOIN      dbo.GDP_Data_Netherlands ON dbo.Anos.Year = dbo.GDP_Data_Netherlands.Year 
            INNER JOIN      dbo.GDP_Data_Saudi_Arabia ON dbo.Anos.Year = dbo.GDP_Data_Saudi_Arabia.Year 
            INNER JOIN      dbo.GDP_Data_South_Korea ON dbo.Anos.Year = dbo.GDP_Data_South_Korea.Year 
            INNER JOIN      dbo.GDP_Data_Spain ON dbo.Anos.Year = dbo.GDP_Data_Spain.Year 
            INNER JOIN      dbo.GDP_Data_Switzerland ON dbo.Anos.Year = dbo.GDP_Data_Switzerland.Year 
            INNER JOIN      dbo.GDP_Data_Turkey ON dbo.Anos.Year = dbo.GDP_Data_Turkey.Year 
            INNER JOIN      dbo.GDP_Data_United_Kingdom ON dbo.Anos.Year = dbo.GDP_Data_United_Kingdom.Year 
            INNER JOIN      dbo.GDP_Data_United_States ON dbo.Anos.Year = dbo.GDP_Data_United_States.Year";

            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=YourDatabase;Integrated Security=True;"))
            {
                SqlCommand command = new SqlCommand(viewQuery, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine($"View '{viewName}' created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating view: {ex.Message}");
                }
            }
        }

        public void CreateGDPView(string viewName, List<string> countries, List<string> tables)
        {
            if (countries.Count != tables.Count)
            {
                throw new ArgumentException("Countries and tables lists must have the same number of elements.");
            }

            // Build the SELECT statement with country-specific GDP columns
            StringBuilder selectClause = new StringBuilder("SELECT dbo.Anos.Year");
            for (int i = 0; i < countries.Count; i++)
            {
                selectClause.Append($", {tables[i]}.GDP AS {countries[i]}");
            }

            // Build the FROM and JOIN statements based on the countries and tables
            StringBuilder joinClause = new StringBuilder("FROM dbo.Anos");
            for (int i = 0; i < countries.Count; i++)
            {
                joinClause.Append($" INNER JOIN {tables[i]} ON dbo.Anos.Year = {tables[i]}.Year");
            }

            // Combine all parts to form the final CREATE VIEW statement
            string viewQuery = $@"
            CREATE VIEW {viewName} AS
            {selectClause}
            {joinClause}";

            // Connection to the SQL Server database
            using (SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog=YourDatabase;Integrated Security=True;"))
            {
                SqlCommand command = new SqlCommand(viewQuery, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Console.WriteLine($"View '{viewName}' created successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error creating view: {ex.Message}");
                }
            }
        }

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


            CreateGDPView();


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
