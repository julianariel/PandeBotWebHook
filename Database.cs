using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PandeBot
{
    public class Database
    {
        ILogger _log;
        public Listas Listas { get; set; }
        public string Path { get; set; }

        public string FileName { get; set; } = "database.json";

        string CompletePath
        {
            get
            {
                return $"{Path}\\{FileName}";
            }
        }
        public Database(string path, string fileName, ILogger log)
        {
            Path = path;
            FileName = fileName;
            _log = log;
        }

        public void SaveDB()
        {
            try
            {
                File.WriteAllText(CompletePath, JsonConvert.SerializeObject(this.Listas), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString());
            }
        }

        public void LoadDB()
        {
            try
            {
                Listas = JsonConvert.DeserializeObject<Listas>(File.ReadAllText(CompletePath), new JsonSerializerSettings { Culture = new CultureInfo("es-AR", false) });
            }
            catch (Exception ex)
            {
                _log.LogError(ex.ToString());
            }
        }
    }

    public class Listas
    {
        public List<string> apodos { get; set; }
        public List<string> frases { get; set; }
        public List<string> audios { get; set; }
        public List<string> photos { get; set; }

    }
}
