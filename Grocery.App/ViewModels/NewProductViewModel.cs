using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Windows.Input;

namespace Grocery.App.ViewModels
{
    public class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        public string Name { get; set; } = string.Empty;
        public int Stock { get; set; }
        public DateOnly ShelfLife { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public decimal Price { get; set; }

        public ICommand SaveCommand { get; }

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;
            SaveCommand = new Command(OnSave);
        }

        private async void OnSave()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Shell.Current.DisplayAlert("Fout", "Naam is verplicht.", "OK");
                return;
            }

            var product = new Product(0, Name, Stock, ShelfLife, Price);
            _productService.Add(product);

            await Shell.Current.DisplayAlert("Succes", $"{Name} toegevoegd!", "OK");
            await Shell.Current.GoToAsync("..");
        }
    }
}
