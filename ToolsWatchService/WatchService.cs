using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToolsWatchService
{
    internal class WatchService : BackgroundService
    {
        class TaskInfo
        {
            public ToolTask Task;
            public ProcessStartInfo Psi;
            public Process Proc;
            public Thread Thread;
            public bool Active = false;
            public Exception Ex = null;

            public TaskInfo(ToolTask t)
            {
                Task = t;
                Psi = new ProcessStartInfo();
                Psi.FileName = t.FileName;
                if (t.Arguments != null)
                    Psi.Arguments = t.Arguments;
                if (t.WorkingDirectory != null)
                    Psi.WorkingDirectory = t.WorkingDirectory;
                if (t.EnvironmentVariables != null)
                    foreach (var v in t.EnvironmentVariables) Psi.EnvironmentVariables[v.Key] = v.Value;
            }

        }

        private readonly List<TaskInfo> taskList = new();
        private bool active = true;
        private Task task;

        public WatchService()
        {
            Log.Information("WatchService ctor");
            foreach (var t in ToolTask.List)
            {
                taskList.Add(new(t));
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("StartAsync");

            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("StopAsync");
            active = false;
            task.Wait();
            Log.Information("StopAsyncKill");
            KillTasks();
            Log.Information("StopAsyncDone");
            return base.StopAsync(cancellationToken);
        }


        private async Task ServiceTasksLoop()
        {
            while (active)
            {
                try
                {
                    ServiceTasks();
                    await Task.Delay(TimeSpan.FromMinutes(0.25));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "ExecuteAsyncError {Message}", ex.ToString());
                }
            }
        }
        private void ServiceTasks()
        {
            Log.Information("ServiceTasks");
            foreach (var t in taskList)
            {
                if (t.Active) continue;
                if (t.Thread == null || t.Proc.ExitCode == 0)
                {
                    t.Thread = new Thread(() => ThreadWork(t));
                    t.Thread.Start();
                    Log.Information("{0} Start", t.Task.Name);
                    continue;
                }
                Log.Error("ThreadWork-{0};ExitCode-{1}", t.Task.Name, t.Proc.ExitCode);
            }
        }

        private void KillTasks()
        {
            foreach (var t in taskList)
            {
                t.Proc.Kill(true);
                Log.Information("{0} Kill", t.Task.Name);
            }
        }

        private void ThreadWork(TaskInfo ti)
        {
            try
            {
                Log.Information("ThreadWork-{0}", ti.Task.Name);
                ti.Active = true;
                ti.Proc = Process.Start(ti.Psi);
                ti.Proc.WaitForExit();
                Log.Information("ThreadWorkStop-{0}", ti.Task.Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ThreadWorkErr");
                ti.Ex = ex;
            }
            finally
            {
                Thread.Sleep(ti.Task.CoolDown ?? 0);
                ti.Active = false;
                Log.Information("{0} Active = false", ti.Task.Name);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Debug("ExecuteAsync");
            task = Task.Run(ServiceTasksLoop);
            await task;
        }
    }
}
