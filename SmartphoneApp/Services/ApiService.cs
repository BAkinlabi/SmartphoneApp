using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using SmartphoneApp.Models.Responses;
using Microsoft.Extensions.Configuration;

namespace SmartphoneApp.Services
{
    /// <summary>
    /// Service for handling API calls.
    /// </summary>
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<ApiService> _logger;
        private readonly IConfiguration _configuration;

        public ApiService(IHttpClientFactory factory, ILogger<ApiService> logger, IConfiguration configuration)
        {
            _factory = factory;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Logs in a user with the provided username and password.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>A tuple containing a success flag, the access token, and an error message if applicable.</returns>
        public async Task<(bool Success, string Token, string ErrorMessage)> LoginAsync(string username, string password)
        {
            var client = _factory.CreateClient();
            var url = $"{_configuration["ApiSettings:BaseUrl"]}/auth/login";
            var payload = new { username, password };
            try
            {
                _logger.LogInformation("POST {Url}", url);
                _logger.LogInformation("Attempting login for user: {username}", username);

                var resp = await client.PostAsJsonAsync(url, payload);
                var content = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation("Api Response: {Content}", content);

                if (!resp.IsSuccessStatusCode)
                    return (false, "", "Invalid credentials");

                var result = JsonSerializer.Deserialize<LoginResponse>(content);
                return (true, result?.accessToken ?? "", "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed.");
                return (false, "", ex.Message);
            }
        }
    }
}