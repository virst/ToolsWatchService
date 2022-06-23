// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

string e = Environment.GetEnvironmentVariable("ENV_1") ?? "null";

string fn = @"c:\tmp\123.txt";
if (args.Length > 0)
    fn = args[0];

do
{
    Console.WriteLine(new FileInfo(fn).FullName + " - " + DateTime.Now.ToLongTimeString());
    File.WriteAllText(fn, DateTime.Now.ToLongTimeString() + $"[{e}]");
    Thread.Sleep(3000);
}
while (args.Length < 2);