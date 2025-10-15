using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        [ObservableProperty]
        private string name = "";

        [ObservableProperty]
        private int stock = 0;

        [ObservableProperty]
        private DateOnly shelfLife = new DateOnly();

        [ObservableProperty]
        private Decimal price = 0.0M;

        [ObservableProperty]
        private string errorMessage = "";

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;
        }

        [RelayCommand]
        public void AddProduct()
        {
            _productService.Add(new Product(0, name, stock, shelfLife, price));
        }
    }
}   
