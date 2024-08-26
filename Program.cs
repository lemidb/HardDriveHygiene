using MycleanerService;

var builder = Host.CreateDefaultBuilder(args).UseWindowsService(options =>{
    options.ServiceName="HardDriveHygiene Service";
});

builder.ConfigureServices(
    (hostcontext,services) => {
        services.AddHostedService<Worker>();
    }
);

var host = builder.Build();
host.Run();
