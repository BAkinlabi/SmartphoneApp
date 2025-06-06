using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SmartphoneApp.Models;
using SmartphoneApp.ModelDTOs;
using SmartphoneApp.Config;
using Microsoft.Extensions.Options;
using System.Text.Json;
using SmartphoneApp.Models.Responses;

namespace SmartphoneApp.Services
{
    public class ApiService : IApiService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<ApiService> _logger;
        private readonly ApiSettings _settings;

        public ApiService(IHttpClientFactory factory, ILogger<ApiService> logger, IOptions<ApiSettings> settings)
        {
            _factory = factory;
            _logger = logger;
            _settings = settings.Value;
        }

        public async Task<(bool Success, string Token, string ErrorMessage)> LoginAsync(string username, string password)
        {
            var client = _factory.CreateClient();
            var url = $"{_settings.BaseUrl}/auth/login";
            var payload = new { username, password };
            try
            {
                _logger.LogInformation("POST {Url} {Payload}", url, payload);
                var resp = await client.PostAsJsonAsync(url, payload);
                var content = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation("Response: {Content}", content);

                if (!resp.IsSuccessStatusCode)
                    return (false, "", "Invalid credentials");

                var result = JsonSerializer.Deserialize<LoginResponse>(content);
                return (true, result?.accessToken ?? "", "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return (false, "", ex.Message);
            }
        }

        public async Task<List<ProductDTO>> GetSmartphonesAsync(string token)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = $"{_settings.BaseUrl}/auth/products/category/smartphones";
           
            try
            {
                _logger.LogInformation("GET {Url}", url);
                var resp = await client.GetAsync(url);
                var content = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation("Response: {Content}", content);

                if (!resp.IsSuccessStatusCode)
                    return new List<ProductDTO>();
                
                var result = JsonSerializer.Deserialize<ProductListResponse>(content);

                return result?.products?.Select(p => new ProductDTO
                {
                    Id = p.id,
                    Title = p.title,
                    Brand = p.brand,
                    Price = p.price,
                    Description = p.description,
                }).ToList() ?? new List<ProductDTO>();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetSmartphones failed");
                return new List<ProductDTO>();
            }
        }

        public async Task<ProductDTO?> UpdateProductPriceAsync(string token, int productId, int newPrice)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = $"{_settings.BaseUrl}/auth/products/{productId}";
            var payload = new { price = newPrice };
            try
            {
                _logger.LogInformation("PUT {Url} {Payload}", url, payload);
                var resp = await client.PutAsJsonAsync(url, payload);
                var content = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation("Response: {Content}", content);

                if (!resp.IsSuccessStatusCode)
                    return null;

                var result = JsonSerializer.Deserialize<Product>(content);

                if (result == null)
                    return null;

                return new ProductDTO
                {
                    Id = result.id,
                    Title = result.title,
                    Brand = result.brand,
                    Price = result.price,
                    Description = result.description,
                };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProductPrice failed");
                return null;
            }
        }
    }
}
