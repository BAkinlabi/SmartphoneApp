using SmartphoneApp.ModelDTOs;

namespace SmartphoneApp.Services
{
    public interface IApiService
    {
        Task<(bool Success, string Token, string ErrorMessage)> LoginAsync(string username, string password);
        Task<List<ProductDTO>> GetSmartphonesAsync(string token);
        Task<ProductDTO?> UpdateProductPriceAsync(string token, int productId, int newPrice);
    }
}
