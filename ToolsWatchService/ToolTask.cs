using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolsWatchService
{
    public class ToolTask
    {
        public static ToolTask[] List;
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public Dictionary<string, string> EnvironmentVariables { get; set; }
        public int? CoolDown { get; set; }
        public string WorkingDirectory { get; set; }
    }
}
