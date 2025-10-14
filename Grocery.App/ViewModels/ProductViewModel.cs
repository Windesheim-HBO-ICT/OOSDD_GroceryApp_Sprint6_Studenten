using CommunityToolkit.Mvvm.Input;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Collections.ObjectModel;

namespace Grocery.App.ViewModels
{
    public partial class ProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;
        public ObservableCollection<Product> Products { get; set; }

        // Fields for creating a new product
        public ProductViewModel(IProductService productService)
        {
            _productService = productService;
            Products = [];
            LoadProducts();
        }

        void LoadProducts()
        {
            Products.Clear();
            foreach (Product p in _productService.GetAll()) Products.Add(p);
        }

        // New product form properties
        public string NewName { get; set; } = string.Empty;
        public int NewStock { get; set; } = 0;
        // Use a toggle to decide whether to set shelf life and a DateTime for DatePicker
        public bool UseShelfLife { get; set; } = false;
        public DateTime NewShelfLifeDate { get; set; } = DateTime.Today;
        public decimal NewPrice { get; set; } = 0m;

        [RelayCommand]
        void AddProduct()
        {
            if (string.IsNullOrWhiteSpace(NewName)) return;
            if (NewStock < 0) NewStock = 0;
            if (NewPrice < 0) NewPrice = 0;

            var shelf = UseShelfLife ? DateOnly.FromDateTime(NewShelfLifeDate) : default;
            var added = _productService.Add(new Product(0, NewName, NewStock, shelf, NewPrice));

            // Update UI list
            Products.Add(added);

            // Reset form
            NewName = string.Empty;
            NewStock = 0;
            UseShelfLife = false;
            NewShelfLifeDate = DateTime.Today;
            NewPrice = 0m;
            OnPropertyChanged(nameof(NewName));
            OnPropertyChanged(nameof(NewStock));
            OnPropertyChanged(nameof(UseShelfLife));
            OnPropertyChanged(nameof(NewShelfLifeDate));
            OnPropertyChanged(nameof(NewPrice));
        }
    }
}
