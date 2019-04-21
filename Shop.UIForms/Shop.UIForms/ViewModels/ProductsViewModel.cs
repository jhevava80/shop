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
        private bool isRefreshing;

        public bool IsRefreshing
        {
            get => this.isRefreshing;
            set => this.SetValue(ref this.isRefreshing, value);
        }

        public ObservableCollection<Product> ProductsList
        {
            get => this.productsList; 
            set => this.SetValue(ref this.productsList, value); 
        }

        public ProductsViewModel()
        {
            this.apiService = new ApiService();
            this.LoadProducts();
        }

        private async void LoadProducts()
        {
            this.IsRefreshing = true;
            var response = await this.apiService.GetListAsync<Product>(
                "https://shopweb20190405104833.azurewebsites.net",
                "/api",
                "/products"
                );

            this.IsRefreshing = false;

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
