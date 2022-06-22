using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ToolsWatchService;

var lpath = "tws_logs/log.log";
if (args.Length > 1)
    lpath = args[1];

Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .WriteTo.File(lpath, rollingInterval: RollingInterval.Day)
               .CreateLogger();

var fn = "tws.json";
if (args.Length > 0)
    fn = args[0];
var j = File.ReadAllText(fn);
ToolTask.List = JsonSerializer.Deserialize<ToolTask[]>(j);
Log.Information("Run - {0}", ToolTask.List.Length);


using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Tools Watch Service";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<WatchService>();
    })
    .Build();

await host.RunAsync();
