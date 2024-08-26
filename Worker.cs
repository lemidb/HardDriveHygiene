using System.IO.Enumeration;
using System.Linq.Expressions;
using Serilog;

namespace MycleanerService;

public class Worker : BackgroundService
{
       
        private readonly IServiceScopeFactory serviceScope;

    public Worker(IServiceScopeFactory serviceScope)
    {
            this.serviceScope = serviceScope;
    
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Logger = new LoggerConfiguration()
        .WriteTo.File(@$"C:\Users\{Environment.UserName}\Desktop\MyCleanerServiceLog.txt")
        .CreateLogger();

        string temp = @"C:\Windows\Temp";
        string temp2 =Path.Combine(@$"C:\Users\{Environment.UserName}\AppData\Local\Temp");
        string prefecth = @"C:\Windows\Prefetch";
        var directories = new List<string>{
            temp,
            temp2,
            prefecth
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            Log.Information("We are about to cleanup the temporary directories");
            await cleanDirectories(directories, stoppingToken);
            await Task.Delay(TimeSpan.FromHours(6), stoppingToken); 
        }
    }

    public async Task cleanDirectories(List<string> directories, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var directory in directories)
            {
                Log.Information($"We are cleaning {directory} now ...,{DateTimeOffset.Now}");  
                var files = Directory.GetFiles(directory);
                foreach (var file in files)
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                            System.Console.WriteLine($"#### Deleting file @ {file}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine($"Exception occured in foreach: {ex.Message}");
                    }
                }
            Log.Information($"The Directory {directory} has been cleaned Successfully.");
            } 
            await Task.Delay(TimeSpan.FromSeconds(10),stoppingToken);
        }

    }
}
