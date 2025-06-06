using SmartphoneApp.ModelDTOs;

namespace SmartphoneApp.Services
{
    public class ProductService : IProductService
    {
        public List<ProductDTO> GetTopExpensiveProducts(IEnumerable<ProductDTO> products, int count)
        {
            return products
                .OrderByDescending(p => p.Price)
                .Take(count)
                .ToList();
        }
    }
}
