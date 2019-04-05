using Microsoft.EntityFrameworkCore;
using Shop.Web.Data.Entities;
using Shop.Web.Data.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Data.CRepositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DataContext context;
        public ProductRepository(DataContext context) : base(context)
        {
            this.context = context;
        }

        public IQueryable GetAllWithUsers()
        {
            // el include incluye objetos relacionados a productos
            return this.context.Products.Include( p=> p.User);
        }
    }
}
