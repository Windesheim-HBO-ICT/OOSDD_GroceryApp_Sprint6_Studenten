using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        public ObservableCollection<Product> Products { get; set; }

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            Products = [];
            foreach (Product p in _productService.GetAll()) Products.Add(p);
        }

        [RelayCommand]
        public async Task GoToAddProduct() {
            await Shell.Current.Navigation.PushAsync(new ProductAddPopup(this));
        }

        public void AddProduct(Product newProduct) {
            Product addedProduct = _productService.Add(newProduct);
            if (!string.IsNullOrWhiteSpace(addedProduct.Name))
                Products.Add(addedProduct);
        }
    }
}
