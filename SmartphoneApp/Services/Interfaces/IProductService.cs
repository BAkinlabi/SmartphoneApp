using SmartphoneApp.ModelDTOs;

namespace SmartphoneApp.Services
{
    public interface IProductService
    {
        // List<ProductDTO> GetTopExpensiveProducts(IEnumerable<ProductDTO> products, int count);
        Task<List<ProductDTO>> GetTopThreeExpensiveSmartphonesAsync(string token);
        Task<ProductDTO?> UpdateProductPriceAsync(string token, int productId, decimal newPrice);
    }
}
