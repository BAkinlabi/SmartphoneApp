using SmartphoneApp.ModelDTOs;

namespace SmartphoneApp.Services
{
    public interface IProductService
    {
        List<ProductDTO> GetTopExpensiveProducts(IEnumerable<ProductDTO> products, int count);
    }
}
