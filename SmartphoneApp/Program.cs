using Microsoft.Extensions.DependencyInjection;
using SmartphoneApp.Services;
using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.Extensions.Logging;

// Build config file
var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .CreateLogger();

var services = new ServiceCollection();

services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddSerilog();
});

services.AddSingleton<IConfiguration>(config);
services.AddTransient<IUserInputService, UserInputService>();
services.AddTransient<IApiService, ApiService>();
services.AddTransient<IAppService, AppService>();
services.AddTransient<IProductService, ProductService>();
services.AddHttpClient();

var provider = services.BuildServiceProvider();
var app = provider.GetRequiredService<IAppService>();

await app.RunAsync();