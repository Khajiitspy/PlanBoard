using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PlanBoard.Tools
{
    public class FileManager
    {
        private static string connectionString = "Host=ep-young-fire-a81ppksr-pooler.eastus2.azure.neon.tech;Port=5432;Username=neondb_owner;Password=npg_gmcuiq1ov0Qr;Database=neondb?sslmode=require;SSL Mode=Require;Trust Server Certificate=true";
        //postgresql://neondb_owner:npg_gmcuiq1ov0Qr@ep-young-fire-a81ppksr-pooler.eastus2.azure.neon.tech/neondb?sslmode=require

        public FileManager(string ConnectionString)
        {
            connectionString = ConnectionString;
        }

        public static void Save<T>(string path, T obj)
        {
            try
            {
                using var connection = new NpgsqlConnection(connectionString);

                connection.Open();
                Console.WriteLine("Connected to the Neon.tech PostgreSQL server successfully!");

                // Example query
                string query = "SELECT version();";

                // Create a command
                using var command = new NpgsqlCommand(query, connection);

                // Execute the query
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine($"PostgreSQL Version: {reader.GetString(0)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            string mystrXAML = XamlWriter.Save(obj);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, mystrXAML);
        }

        public static T Load<T>(string path)
        {
            string xamlString = File.ReadAllText(path);

            using (var stringReader = new StringReader(xamlString))
            {
                using (var xmlReader = System.Xml.XmlReader.Create(stringReader))
                {
                    var obj = XamlReader.Load(xmlReader);
                    return (T)obj;
                }
            }
        }
    }
}
