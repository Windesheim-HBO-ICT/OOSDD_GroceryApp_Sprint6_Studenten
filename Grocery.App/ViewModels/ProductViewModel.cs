using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        public ObservableCollection<Product> Products { get; set; }
        public Client Client { get; set; }

        public ProductViewModel(IProductService productService, GlobalViewModel global)
        {
            _productService = productService;
            Products = [];
            Client = global.Client;
            foreach (Product p in _productService.GetAll()) Products.Add(p);
        }
        [RelayCommand]
        public async Task ShowNewProduct()
        {
            if (Client.Role == Role.Admin) await Shell.Current.GoToAsync(nameof(NewProductView), true);
        }
    }
}
