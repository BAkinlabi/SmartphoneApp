using Microsoft.Extensions.Logging;

namespace SmartphoneApp.Services
{
    /// <summary>
    /// Represents the application service that handles the main application logic.
    /// </summary>
    public class AppService : IAppService
    {
        private readonly IApiService _apiService;
        private readonly IUserInputService _userInputService;
        private readonly IProductService _productService;
        private readonly ILogger<AppService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppService"/> class.
        /// </summary>
        /// <param name="api">The API service for making requests.</param>
        /// <param name="input">The user input service for gathering user input.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        /// <param name="productService">The product service for managing products.</param>
        public AppService(IApiService api, IUserInputService input, ILogger<AppService> logger, IProductService productService)
        {
            _apiService = api;
            _userInputService = input;
            _logger = logger;
            _productService = productService;
        }

        /// <summary>
        /// Runs the main application logic asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            try
            {
                _logger.LogInformation("Application started.");

                // Login
                var (username, password) = _userInputService.GetCredentials();
                var loginResult = await _apiService.LoginAsync(username, password);
                if (!loginResult.Success)
                {
                    _logger.LogError("Login failed: {Message}", loginResult.ErrorMessage);
                    Console.WriteLine("Login failed: " + loginResult.ErrorMessage);
                    Console.WriteLine("This application requires a vaid username and password.");
                    Console.ReadLine();
                    _logger.LogInformation("Application terminated.");
                    return;
                }
                _logger.LogInformation("Login successful for user {User}", username);

                // Get the top three most expensive smart phones
                var smartPhones = await _productService.GetTopThreeExpensiveSmartphonesAsync(loginResult.Token);

                Console.WriteLine("The three most expensive smartphones:");
                foreach (var p in smartPhones)
                    Console.WriteLine($"{p.Brand} - {p.Title}: ${p.Price}");

                // Get percentage
                var percent = _userInputService.GetPercentage();

                // Update prices
                foreach (var p in smartPhones)
                {
                    var newPrice = p.Price + (p.Price * percent / 100);
                    var updated = await _productService.UpdateProductPriceAsync(loginResult.Token, p.Id, (decimal)newPrice);
                    if (updated != null)
                        Console.WriteLine($"Updated {updated.Brand} - {updated.Title}: ${updated.Price.ToString("#.##")}");
                    else
                        Console.WriteLine($"Failed to update {p.Title}");
                }

                Console.WriteLine("Please press any key to terminate.");
                Console.ReadKey();
                _logger.LogInformation("Application terminated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}