using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPRP.Classes
{
    public class RegistryRecovery
    {
        public DateTime DateCreation { get; } = DateTime.Now.Date;
        public Dictionary<string, string> DefaultValues { get; } = new Dictionary<string, string>();
        public void Save(string outputFolder)
        {
            string jsonFile = Path.Combine(outputFolder, "registryRecovery.json");
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (StreamWriter streamWriter = new StreamWriter(jsonFile, false, Encoding.UTF8))
            {
                streamWriter.Write(json);
            }
        }
    }
}
