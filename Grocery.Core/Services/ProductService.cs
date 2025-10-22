using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public List<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product Add(Product item)
        {
            // Validatie
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (string.IsNullOrWhiteSpace(item.Name))
                throw new ArgumentException("Productnaam is verplicht");

            if (item.Price < 0)
                throw new ArgumentException("Prijs mag niet negatief zijn");

            return _productRepository.Add(item);
        }

        public Product? Delete(Product item)
        {
            return _productRepository.Delete(item);
        }

        public Product? Get(int id)
        {
            return _productRepository.Get(id);
        }

        public Product? Update(Product item)
        {
            return _productRepository.Update(item);
        }
    }
}
