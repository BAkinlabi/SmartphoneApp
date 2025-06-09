
using Microsoft.Extensions.Logging;

namespace SmartphoneApp.Services
{
    /// <summary>
    /// Service for handling user input.
    /// </summary>
    public class UserInputService : IUserInputService
    {
        private readonly ILogger<UserInputService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInputService"/> class.
        /// </summary>
        /// <param name="logger">The logger to log user input information.</param>
        public UserInputService(ILogger<UserInputService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Prompts the user to enter their credentials (username and password).
        /// </summary>
        /// <returns>A tuple containing the username and password.</returns>
        public (string Username, string Password) GetCredentials()
        {
            string username;
            string password;

            try
            {
                do
                {
                    Console.Write("Please enter a Username: ");
                    username = (Console.ReadLine() ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(username))
                        Console.WriteLine("Username cannot be empty. Please try again.");
                } while (string.IsNullOrWhiteSpace(username));

                _logger.LogInformation("User entered username: {username}", username);

                do
                {
                    Console.Write("Please enter a Password: ");
                    password = (Console.ReadLine() ?? string.Empty).Trim();
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Password cannot be empty. Please try again.");
                    }
                } while (string.IsNullOrWhiteSpace(password));

                _logger.LogInformation("User entered password: {password}", "******");

                return (username, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting credentials.");
                throw;
            }
        }

        /// <summary>
        /// Prompts the user to enter a percentage to increase the price.
        /// </summary>
        /// <returns>The percentage entered by the user as a positive integer.</returns>
        public int GetPercentage()
        {
            while (true)
            {
                try
                {
                    Console.Write("Enter percentage to increase price: ");
                    var input = Console.ReadLine();
                    _logger.LogInformation("User entered percentage: {Percent}", input);
                    if (int.TryParse(input, out int percent) && percent > 0)
                        return percent;

                    Console.WriteLine("Invalid input. Please enter a positive integer.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while getting percentage.");
                    Console.WriteLine("An error occurred. Please try again.");
                }
            }
        }
    }
}
