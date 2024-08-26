using System.IO;
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
        // Setup Serilog to log to a file
        Log.Logger = new LoggerConfiguration()
        .WriteTo.File(@$"C:\Users\{Environment.UserName}\Desktop\MyCleanerServiceLog.txt")
        .CreateLogger();
        //After Identifying the temp directories and files we can setup their filepath like the following then keep their list.
        string temp = @"C:\Windows\Temp";
        string temp2 = Path.Combine(@$"C:\Users\{Environment.UserName}\AppData\Local\Temp");
        string prefecth = @"C:\Windows\Prefetch";
        string[] directories = { temp, temp2, prefecth };

        while (!stoppingToken.IsCancellationRequested)
        {
            System.Console.WriteLine($"MyCleaner Service is started...");
            foreach (var directory in directories)
            {
                // keeping the log on external file, that we cleaned this directory at this time and date optional not necessary actually
                Log.Information($"We are cleaning {directory} now ...,{DateTimeOffset.Now}");
                CleanUpRecursively(directory);
            }

            await Task.Delay(TimeSpan.FromHours(6), stoppingToken);

        }


    }


    public void CleanUpRecursively(string rootDirectory)
    {
        try
        {
            System.Console.WriteLine($"CleanupRecursively is taking :{rootDirectory}");
                // Check if the root directory exists
                if (Directory.Exists(rootDirectory)){
                // Get all directories in the root directory
                var directories = Directory.GetDirectories(rootDirectory);
                if (directories.Length > 0)
                {
                    foreach (var dir in directories)
                    {
                        // Recursively clean up subdirectories
                        CleanUpRecursively(dir);
                        // Clean up files in the current directory
                        CleanUp(Path.Combine(rootDirectory, Path.GetFileName(dir)));

                    }
                }
                CleanUp(rootDirectory);
            }
                

            
        }
        catch (Exception ex)
        {
            // Log exceptions if necessary
            Console.WriteLine($"Error cleaning up recursively in {rootDirectory}: {ex.Message}");
        }
    }


     public void CleanUp(string directoryPath)
    {
        try
        {
            System.Console.WriteLine($"CleanUp is being called with directory path :{directoryPath}");
                // Check if the directory exists
                if (Directory.Exists(directoryPath)){
                // Get all files in the directory
                var files = Directory.GetFiles(directoryPath);
                if (files.Length > 0)
                {
                    foreach (var file in files)
                    {
                        // Delete each file
                        File.Delete(file);
                        System.Console.WriteLine($"We are deleting {file}");
                    }
                }
                Directory.Delete(directoryPath);
            }
            
        }
        catch (Exception ex)
        {
            // Log exceptions if necessary
            Console.WriteLine($"Error cleaning up {directoryPath}: {ex.Message}");
        }
    }


}
