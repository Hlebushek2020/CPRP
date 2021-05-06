using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPRP.Classes
{
    public class RegistrySectionUserChoice
    {
        [JsonIgnore]
        public const string KeyHash = "Hash";
        [JsonIgnore]
        public const string KeyProgId = "ProgId";

        public string Hash { get; set; }
        public string ProgId { get; set; }
    }
}
