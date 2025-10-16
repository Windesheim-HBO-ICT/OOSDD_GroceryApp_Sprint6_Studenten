using Grocery.App.ViewModels;
using Grocery.Core.Models;


namespace Grocery.App.Views {
    public partial class ProductAddPopup : ContentPage {
        public ProductAddPopup(ProductViewModel viewModel) {
            InitializeComponent();
            BindingContext = viewModel;
            ShelfLifePicker.Date = DateTime.Today;
        }

        private async void OnCancelClicked(object sender, EventArgs e) {
            await Shell.Current.Navigation.PopAsync();
        }

        private async void OnAddClicked(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(NameEntry.Text)) {
                DisplayAlert("het product moet een naam hebben");
                return;
            }

            if (!int.TryParse(StockEntry.Text, out int stock)) {
                DisplayAlert("Voorraad moet een getal zijn");
                return;
            }

            if (!decimal.TryParse(PriceEntry.Text, out decimal price)) {
                DisplayAlert("Prijs moet een geldig bedrag zijn");
                return;
            }

            Product product = new Product(
                0, 
                NameEntry.Text,
                stock,
                DateOnly.FromDateTime(ShelfLifePicker.Date),
                price);

            if (BindingContext is ProductViewModel viewModel) {
                viewModel.AddProduct(product);
            }

            await Shell.Current.Navigation.PopAsync();
        }

        private async void DisplayAlert(string message = "Vul alle verplichte velden in") {
            await Application.Current.MainPage.DisplayAlert("Fout", message, "OK");
        }
    }
}