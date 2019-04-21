using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.UIForms.ViewModels
{
    public class MainViewModel
    {
        //implement singlenton
        private static MainViewModel instance;

        public LoginViewModel Login { get; set; }
        public ProductsViewModel Products { get; set; }

        public MainViewModel()
        {
            instance = this;
        }

        //para llamar al singleton desde cualquier lugar
        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }
            return instance;
        }
        
    }
}
