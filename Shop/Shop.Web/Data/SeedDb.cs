using Shop.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext context;
        private Random random;

        public SeedDb(DataContext context)
        {
            this.context = context;
            this.random = new Random();
        }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();

            if (! this.context.Products.Any())
            {
                this.AddProduct("First product");
                this.AddProduct("First product");
                this.AddProduct("First product");
                await this.context.SaveChangesAsync();
            }
        }


        public void AddProduct(string name)
        {
            this.context.Products.Add(new Product {
                Name = name,
                Price = this.random.Next(100000),
                IsAvaliable = true,
                Stock = this.random.Next(200)
            });

        }
    }
}
