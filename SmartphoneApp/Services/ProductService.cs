using SmartphoneApp.ModelDTOs;
using SmartphoneApp.Models.Responses;
using SmartphoneApp.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SmartphoneApp.Services
{
    /// <summary>
    /// Service for managing product-related operations.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<ProductService> _logger;
        private readonly IConfiguration _configuration;

        public ProductService(IHttpClientFactory factory, ILogger<ProductService> logger, IConfiguration configuration)
        {
            _factory = factory;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Retrieves the top three most expensive smartphones.
        /// </summary>
        /// <param name="token">The authorization token.</param>
        /// <returns>A list of the top three expensive smartphones.</returns>
        public async Task<List<ProductDTO>> GetTopThreeExpensiveSmartphonesAsync(string token)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token); // Improved header setting
            var url = $"{_configuration["ApiSettings:BaseUrl"]}/auth/products/category/smartphones?sortBy=price&order=desc&limit={_configuration["ApiSettings:TopProductsNumber"]}";

            try
            {
                _logger.LogInformation("GET {Url}", url);
                var resp = await client.GetAsync(url);
                resp.EnsureSuccessStatusCode(); // Throws if not successful
                var content = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation("Api Response: {Content}", content);

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
                _logger.LogError(ex, "GetTopThreeExpensiveSmartphones failed");
                return new List<ProductDTO>();
            }
        }

        /// <summary>
        /// Updates the price of a specified product.
        /// </summary>
        /// <param name="token">The authorization token.</param>
        /// <param name="productId">The ID of the product to update.</param>
        /// <param name="newPrice">The new price to set for the product.</param>
        /// <returns>The updated product details, or null if the update failed.</returns>
        public async Task<ProductDTO?> UpdateProductPriceAsync(string token, int productId, decimal newPrice)
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token); // Improved header setting
            var url = $"{_configuration["ApiSettings:BaseUrl"]}/auth/products/{productId}";
            var payload = new { price = newPrice };

            try
            {
                _logger.LogInformation("PUT {Url} {Payload}", url, payload);
                var resp = await client.PutAsJsonAsync(url, payload);
                resp.EnsureSuccessStatusCode(); // Throws if not successful
                var content = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation("Api Response: {Content}", content);

                var result = JsonSerializer.Deserialize<Product>(content);

                return result == null ? null : new ProductDTO
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