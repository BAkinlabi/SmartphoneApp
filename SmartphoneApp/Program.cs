using Microsoft.Extensions.DependencyInjection;
using SmartphoneApp.Services;
using SmartphoneApp.Config;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Configure config file
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

services.Configure<ApiSettings>(config.GetSection("ApiSettings"));

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    //.MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/smartphoneApp-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog(dispose: true);
});

services.AddSingleton<IUserInputService, UserInputService>();
services.AddSingleton<IApiService, ApiService>();
services.AddSingleton<IAppService, AppService>();
services.AddSingleton<IProductService, ProductService>();
services.AddHttpClient();

// Configure logging
//services.AddLogging(loggingBuilder =>
//{
//    loggingBuilder.AddSerilog(dispose: true);
//});

var provider = services.BuildServiceProvider();
var app = provider.GetRequiredService<IAppService>();

await app.RunAsync();

Log.CloseAndFlush();
