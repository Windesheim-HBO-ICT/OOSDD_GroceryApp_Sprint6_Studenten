using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Grocery.App.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        public ObservableCollection<Product> Products { get; set; }
        public ICommand NavigateToNewProductCommand { get; }

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            Products = new ObservableCollection<Product>();

            NavigateToNewProductCommand = new Command(async () => await NavigateToNewProduct());

            LoadProducts();
        }

        private void LoadProducts()
        {
            var products = _productService.GetAll();
            Products.Clear();
            foreach (Product p in products)
            {
                Products.Add(p);
            }
        }

        public void RefreshProducts()
        {
            LoadProducts();
        }

        private async Task NavigateToNewProduct()
        {
            // TODO: Check of gebruiker admin-rol heeft
            // Voor nu: navigeer altijd
            await Shell.Current.GoToAsync("newproduct");
        }
    }
}