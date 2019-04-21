using Microsoft.AspNetCore.Identity;
using Shop.Web.Data.Entities;
using Shop.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Data
{
    public class SeedDb
    {
        private readonly DataContext context;
        private readonly IUserHelper userHelper;
        private Random random;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            this.context = context;
            this.userHelper = userHelper;
            this.random = new Random();
        }

        public async Task SeedAsync()
        {
            await this.context.Database.EnsureCreatedAsync();

            await this.userHelper.CheckRoleAsync("Admin");
            await this.userHelper.CheckRoleAsync("Customer");

            //creamos usuario si no existe
            var user = await this.userHelper.GetUserByEmailAsync("jhevava80@gmail.com");
            if (user == null)
            {
                user = new User
                {
                    LastName="vargas",
                    FirstName ="Jhon",
                    Email = "jhevava80@gmail.com",
                    UserName = "jhevava80@gmail.com",
                    PhoneNumber = "3192079541"
                };

                var result = await this.userHelper.AddUserAsync(user, "123456");
                if(!result.Succeeded)
                {
                    throw new InvalidOperationException("could not create the user with seeder");
                }

                await this.userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var isUserInRole = await this.userHelper.IsUserInRoleAsync(user, "Admin");
            if(!isUserInRole)
            {
                await this.userHelper.AddUserToRoleAsync(user,"Admin");
            }

            //Creamos productos
            if (! this.context.Products.Any())
            {
                this.AddProduct("First product",user);
                this.AddProduct("Second product", user);
                this.AddProduct("Third product", user);
                await this.context.SaveChangesAsync();
            }
        }


        public void AddProduct(string name,User user)
        {
            this.context.Products.Add(new Product {
                Name = name,
                Price = this.random.Next(100000),
                IsAvaliable = true,
                Stock = this.random.Next(200),
                User = user
            });

        }
    }
}
