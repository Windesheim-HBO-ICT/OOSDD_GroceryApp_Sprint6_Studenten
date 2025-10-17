using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grocery.Core.Models;

namespace Grocery.App.ViewModels
{
    public partial class AddProductViewModel : BaseViewModel
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

        public AddProductViewModel(IProductService productService)
        {
            _productService = productService;
        }

        [RelayCommand]
        public async Task AddProduct()
        {
            decimal formatedPricerice = TryParseDecimal(price);
            if (formatedPricerice >= 0)
            {
                _productService.Add(new Product(0, name, stock, DateOnly.Parse(shelfLife.Split(' ')[0]), formatedPricerice));
                // Close current page and navigate back to products page
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                errorMessage = "Ongeldige prijs. Gebruik een punt of komma als decimaalteken.";
            }
        }

        static decimal TryParseDecimal(string input)
        {
            decimal value;
            if (string.IsNullOrWhiteSpace(input))
            {
                 value = 0;
            }else
            {
                string normalized = input.Replace(',', '.');
                if (!decimal.TryParse(normalized, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out  value))
                {
                    value = 0;
                }
            }
            return value;
        }
    }
}
