using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPRP.Classes
{
    public class ProcessArgs
    {
        public string App { get; }
        public string PriorityArg { get; }
        public string OutputFolder { get; }
        public bool CreateShortcutOnDesktop { get; }
        public bool IsReplaceDesktopShortcut { get; }
        public IReadOnlyCollection<string> ExtensionList { get; }

        public ProcessArgs(string app, PriorityItem priorityItem, string outputFolder, bool createShortcutOnDesktop, bool isReplaceDesktopShortcut, IReadOnlyCollection<string> extensionList)
        {
            App = app;
            OutputFolder = outputFolder;
            CreateShortcutOnDesktop = createShortcutOnDesktop;
            IsReplaceDesktopShortcut = isReplaceDesktopShortcut;
            ExtensionList = extensionList;
            PriorityArg = priorityItem.PriorityArg;
        }
    }
}
