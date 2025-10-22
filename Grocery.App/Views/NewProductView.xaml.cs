using Grocery.App.ViewModels;

namespace Grocery.App.Views
{
    public partial class NewProductView : ContentPage
    {
        public NewProductView(NewProductViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            
            if (Application.Current.MainPage is Shell shell)
            {
                
                var productView = shell.CurrentPage;
                if (productView?.BindingContext is ProductViewModel productViewModel)
                {
                    productViewModel.RefreshProducts();
                }
            }
        }
    }
}