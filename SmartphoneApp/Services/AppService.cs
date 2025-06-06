using Microsoft.Extensions.Logging;

namespace SmartphoneApp.Services
{
    public class AppService : IAppService
    {
        private readonly IApiService _api;
        private readonly IUserInputService _input;
        private readonly IProductService _productService; 
        private readonly ILogger<AppService> _logger;


        public AppService(IApiService api, IUserInputService input, ILogger<AppService> logger, IProductService productService)
        {
            _api = api;
            _input = input;
            _logger = logger;
            _productService = productService;
        }

        public async Task RunAsync()
        {
            try
            {
                _logger.LogInformation("Application started.");

                // Login
                var (username, password) = _input.GetCredentials();
                var loginResult = await _api.LoginAsync(username, password);
                if (!loginResult.Success)
                {
                    _logger.LogError("Login failed: {Message}", loginResult.ErrorMessage);
                    Console.WriteLine("Login failed: " + loginResult.ErrorMessage);
                    return;
                }
                _logger.LogInformation("Login successful for user {User}", username);

                // Get products
                var products = await _api.GetSmartphonesAsync(loginResult.Token);

                var top3 = _productService.GetTopExpensiveProducts(products, 3);

                Console.WriteLine("Top 3 most expensive smartphones:");
                foreach (var p in top3)
                    Console.WriteLine($"{p.Brand} - {p.Title}: ${p.Price}");

                // Get percentage
                var percent = _input.GetPercentage();
                _logger.LogInformation("User entered percentage: {Percent}", percent);

                // Update prices
                foreach (var p in top3)

                {
                    var newPrice = p.Price + (p.Price * percent / 100);
                    var updated = await _api.UpdateProductPriceAsync(loginResult.Token, p.Id, (int)newPrice);
                    if (updated != null)
                        Console.WriteLine($"Updated {updated.Brand} - {updated.Title}: ${updated.Price}");
                    else
                        Console.WriteLine($"Failed to update {p.Title}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
