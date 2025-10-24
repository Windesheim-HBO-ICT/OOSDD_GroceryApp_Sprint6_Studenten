
using Grocery.Core.Interfaces.Repositories;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _productCategoriesRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public List<ProductCategory> GetAll()
        {
            return _productCategoriesRepository.GetAll();
        }

        public ProductCategoryService(IProductCategoryRepository productCategoriesRepository,
                                      IProductRepository productRepository,
                                      ICategoryRepository categoryRepository)
        {
            _productCategoriesRepository = productCategoriesRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        private void FillRelations(ProductCategory pc)
        {
            pc.Product = _productRepository.Get(pc.ProductId) ?? new(0, "Onbekend Product", 0);
            pc.Category = _categoryRepository.Get(pc.CategoryId) ?? new(0, "Onbekende Categorie");
        }

        public List<ProductCategory> GetAllOnCategoryId(int id)
        {
            List<ProductCategory> productCategories = _productCategoriesRepository.GetAll()
                .Where(p => p.CategoryId == id).ToList();

            foreach (ProductCategory pc in productCategories)
            {
                FillRelations(pc);
            }

            return productCategories;
        }

        public ProductCategory Add(ProductCategory item)
        {
            _productCategoriesRepository.Add(item);
            FillRelations(item);
            return item;
        }

    }
}
