using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Maui.Alerts;
using Grocery.App.Views;
using System.Text.Json;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private int stock;

        [ObservableProperty]
        private string shelfLife = DateTime.Today.ToString("dd-MM-yy");

        [ObservableProperty]
        private string price;

        [ObservableProperty]
        private string errorMessage = "";

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;
        }

        [RelayCommand]
        public async Task AddProductAsync()
        {
            if (TryParseDecimal(price, out decimal parsedPrice))
            {
                _productService.Add(new Product(0, name, stock, DateOnly.Parse(ShelfLife.Split(' ')[0]), parsedPrice));
                // Close current page and navigate back to products page
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                errorMessage = "Ongeldige prijs. Gebruik een punt of komma als decimaalteken.";
            }
        }

        static bool TryParseDecimal(string input, out decimal value)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                value = 0;
                return false;
            }

            // Sta zowel punt als komma toe als decimaalteken
            string normalized = input.Replace(',', '.');

            return decimal.TryParse(
                normalized,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out value);
        }
    }
}   
