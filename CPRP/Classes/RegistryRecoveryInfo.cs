using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPRP.Classes
{
    public class RegistryRecoveryInfo
    {
        [JsonIgnore]
        public const string KeyFullFileExts = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";
        [JsonIgnore]
        public const string KeyUserChoice = "UserChoice";
        [JsonIgnore]
        public const string KeyOpenWithProgids = "OpenWithProgids";

        public string DefaultMachine { get; set; }
        public string DefaultUser { get; set; }
        public RegistrySectionUserChoice UserChoice { get; set; } = new RegistrySectionUserChoice();
    }
}
