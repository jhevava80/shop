using Shop.Common.Models;
using Shop.Common.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;

namespace Shop.UIForms.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        private readonly ApiService apiService;
        private ObservableCollection<Product> productsList;
       
        public ObservableCollection<Product> ProductsList
        {
            get { return this.productsList; }
            set { this.SetValue(ref this.productsList, value); }
        }

        public ProductsViewModel()
        {
            this.apiService = new ApiService();
            this.LoadProducts();
        }

        private async void LoadProducts()
        {
            var response = await this.apiService.GetListAsync<Product>(
                "https://shopweb20190405104833.azurewebsites.net",
                "/api",
                "/products"
                );

            if (!response.IsSucces)
            {
                await Application.Current.MainPage.DisplayAlert("Error",response.Message,"Accept");
                return;
            }

            var misProductos = (List<Product>)response.Result;
            this.ProductsList = new ObservableCollection<Product>(misProductos);

        }
    }
}
