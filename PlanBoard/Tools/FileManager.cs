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
        string connectionString = "Host=ep-young-fire-a81ppksr-pooler.eastus2.azure.neon.tech;Port=5432;Username=<neondb_owner>;Password=<npg_gmcuiq1ov0Qr>;Database=neondb?sslmode=require;SSL Mode=Require;Trust Server Certificate=true";
        //postgresql://neondb_owner:npg_gmcuiq1ov0Qr@ep-young-fire-a81ppksr-pooler.eastus2.azure.neon.tech/neondb?sslmode=require

        public FileManager()
        {
        }

        public static void Save<T>(string path, T obj)
        {
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
