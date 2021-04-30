using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPRP.Classes
{
    public class PriorityItem
    {
        public string Title { get; }
        public ProcessPriorityClass Priority { get; }
        public string PriorityArg { get; }

        public PriorityItem(string title, ProcessPriorityClass priority, string priorityArg)
        {
            Title = title;
            Priority = priority;
            PriorityArg = priorityArg;
        }
    }
}
