using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPRP_Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Process runApp = new Process();
            // set app
            runApp.StartInfo.FileName = args[0];
            // construct args for app
            for (int i = 1; i < args.Length - 1; i++)
            {
                if (File.Exists(args[i]))
                    runApp.StartInfo.Arguments += $"\"{args[i]}\" ";
                else
                    runApp.StartInfo.Arguments += args[i] + " ";
            }
            runApp.StartInfo.Arguments = runApp.StartInfo.Arguments.TrimEnd(' ');
            // start
            runApp.Start();
            // set priority
            string priorityArg = args[args.Length - 1];
            switch (priorityArg)
            {
                case "-n": //Normal = 32
                    runApp.PriorityClass = ProcessPriorityClass.Normal;
                    break;
                case "-i": //Idle = 64
                    runApp.PriorityClass = ProcessPriorityClass.Idle;
                    break;
                case "-h": //High = 128
                    runApp.PriorityClass = ProcessPriorityClass.High;
                    break;
                case "-rt": //RealTime = 256
                    runApp.PriorityClass = ProcessPriorityClass.RealTime;
                    break;
                case "-bn": //BelowNormal = 16384
                    runApp.PriorityClass = ProcessPriorityClass.BelowNormal;
                    break;
                case "-an": //AboveNormal = 32768
                    runApp.PriorityClass = ProcessPriorityClass.AboveNormal;
                    break;
            }
        }
    }
}
