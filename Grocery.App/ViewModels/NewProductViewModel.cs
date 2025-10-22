using CommunityToolkit.Mvvm.ComponentModel;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;
using System.Windows.Input;

namespace Grocery.App.ViewModels
{
    public partial class NewProductViewModel : BaseViewModel
    {
        private readonly IProductService _productService;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private int stock;

        [ObservableProperty]
        private decimal price;

        [ObservableProperty]
        private DateTime expirationDate = DateTime.Now.AddMonths(1);

        [ObservableProperty]
        private bool isBusy;

        //   MinimumDate eigenschap voor de DatePicker
        public DateTime MinimumDate => DateTime.Now;

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public NewProductViewModel(IProductService productService)
        {
            _productService = productService;

            SaveCommand = new Command(async () => await SaveProduct());
            CancelCommand = new Command(async () => await Cancel());
        }

        private bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name) && Price >= 0;
        }

        private async Task SaveProduct()
        {
            try
            {
                IsBusy = true;

                // Maak nieuw product aan
                var newProduct = new Product(
                    0, // ID wordt door database gegenereerd
                    Name,
                    Stock,
                    DateOnly.FromDateTime(ExpirationDate),
                    Price
                );

                // Product toevoegen via service
                _productService.Add(newProduct);

                // Toon bevestiging
                await Application.Current.MainPage.DisplayAlert(
                    "Succes",
                    $"Product '{Name}' is toegevoegd!",
                    "OK"
                );

                // Navigeer terug
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Fout",
                    $"Product kon niet worden toegevoegd: {ex.Message}",
                    "OK"
                );
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
