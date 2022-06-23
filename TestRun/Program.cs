// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Text.Json;
using ToolsWatchService;

//Console.WriteLine("Hello, World!");

var fn = "tws.json";
if (args.Length > 0)
    fn = args[0];
var j = File.ReadAllText(fn);
ToolTask.List = JsonSerializer.Deserialize<ToolTask[]>(j);
Console.WriteLine(fn);
Console.WriteLine("Run - {0}", ToolTask.List?.Length);
for(int i = 0; i < ToolTask.List?.Length; i++)
{
    Console.WriteLine($"{i}-{ToolTask.List[i]}");
}
Console.WriteLine("Select num:");

var n = Convert.ToInt32(Console.ReadLine());
var t = ToolTask.List[n];

var Psi = new ProcessStartInfo();
Psi.FileName = t.FileName;
if (t.Arguments != null)
    Psi.Arguments = t.Arguments;
if (t.WorkingDirectory != null)
    Psi.WorkingDirectory = t.WorkingDirectory;
if (t.EnvironmentVariables != null)
    foreach (var v in t.EnvironmentVariables) Psi.EnvironmentVariables[v.Key] = v.Value;

var p = Process.Start(Psi); 
p.WaitForExit();


