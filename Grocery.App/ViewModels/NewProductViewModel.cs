using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Models;
using Grocery.Core.Interfaces.Services;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        public string Name { get; set; }
        public int Stock { get; set; }
        public DateOnly ShelfLife { get; set; }
        public decimal Price { get; set; }

        [ObservableProperty]
        private string errorMessage;

        [ObservableProperty]
        private string message;

        public event Action? ProductAdded;

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;
            ShelfLife = DateOnly.FromDateTime(DateTime.Today);
        }

        public bool NameExists(string name)
        {
            return _productService.NameExists(name);
        }

        [RelayCommand]
        public void AddProduct()
        {
            ErrorMessage = string.Empty;
            Message = string.Empty;
            if (NameExists(Name))
            {
                ErrorMessage = "Productnaam bestaat al, vul een andere naam in";
                return;
            }
            if (Stock < 0)
            {
                ErrorMessage = "Voorraad mag niet negatief zijn, vul opnieuw in";
                return;
            }
            if (Price <= 0)
            {
                ErrorMessage = "Prijs mag niet negatief of nul zijn, vul opnieuw in";
                return;
            }
            Product product = new Product(0, Name, Stock, ShelfLife, Price);
            _productService.Add(product);
            Message = "Product toegevoegd!";
            ProductAdded?.Invoke();
        }
    }
}