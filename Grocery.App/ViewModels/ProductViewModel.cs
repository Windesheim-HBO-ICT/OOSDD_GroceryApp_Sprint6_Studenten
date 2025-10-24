using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using Grocery.App.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Grocery.App.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        public ObservableCollection<Product> Products { get; set; }

        
        public ICommand AddNewProductCommand { get; }

        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            Products = [];

            foreach (Product p in _productService.GetAll())
                Products.Add(p);

            
            AddNewProductCommand = new Command(OnAddNewProduct);
        }    
        private async void OnAddNewProduct()
        {
            
            await Shell.Current.GoToAsync(nameof(NewProductView));
        }
        public void RefreshProducts()
        {
            Products.Clear();
            foreach (Product p in _productService.GetAll())
                Products.Add(p);
        }

    }
}
