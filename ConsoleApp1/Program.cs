// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var fn = @"c:\tmp\123.txt";
if (args.Length > 0)
    fn = args[0];

while (true)
{
    Console.WriteLine(new FileInfo(fn).FullName + " - " + DateTime.Now.ToLongTimeString());
    File.WriteAllText(fn, DateTime.Now.ToLongTimeString());
    Thread.Sleep(3000);
}