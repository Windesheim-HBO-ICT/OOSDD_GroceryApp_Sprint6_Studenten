using CommunityToolkit.Mvvm.Input;
using Grocery.App.Views;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;


namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        Client client;
        private readonly IProductService _productService;
        public ObservableCollection<Product> Products { get; set; }

        public ProductViewModel(IProductService productService, GlobalViewModel global)
        {
            client = global.Client;
            _productService = productService;
            Products = [];
            foreach (Product p in _productService.GetAll()) Products.Add(p);
        }
        [RelayCommand]
        public async Task ShowAddProduct()
        {
            if (client.Role == Role.Admin) await Shell.Current.GoToAsync(nameof(AddProductView), true);
        }
    }
}
