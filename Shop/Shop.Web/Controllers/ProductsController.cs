using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shop.Web.Data;
using Shop.Web.Data.Entities;
using Shop.Web.Data.IRepositories;
using Shop.Web.Helper;
using Shop.Web.Models;

namespace Shop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly IUserHelper userHelper;

        public ProductsController(IProductRepository productRepository, IUserHelper userHelper)
        {
            this.productRepository = productRepository;
            this.userHelper = userHelper;
        }

        // GET: Products
        public IActionResult Index()
        {
            return View(this.productRepository.GetAll().OrderBy(p=> p.Name));
        }

        // GET: Products/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this.productRepository.GetByIdAsync(id.Value).Result;
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                var path = string.Empty;

                if (productViewModel.ImageFile != null && productViewModel.ImageFile.Length> 0)
                {
                    var guid = Guid.NewGuid().ToString();
                    string file = $"{guid}.jpg";

                    path = Path.Combine(Directory.GetCurrentDirectory(),
                        "Image\\products", 
                        file);
                    using (var stream = new FileStream(path,FileMode.Create))
                    {
                        await productViewModel.ImageFile.CopyToAsync(stream);
                    }

                    path = $"~/Image/Products/{file}";
                }
                var product = this.ConvertToProduct(productViewModel,path);

                //TODO: change for logged user
                product.User = await this.userHelper.GetUserByEmailAsync("jhevava80@gmail.com");

                await this.productRepository.CreateAsync(product);
                await this.productRepository.SaveAllAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        private Product ConvertToProduct(ProductViewModel productViewModel, string path)
        {
            return new Product
            {
                Id = productViewModel.Id,
                ImageUrl = path,
                IsAvaliable = productViewModel.IsAvaliable,
                LastPurchase = productViewModel.LastPurchase,
                LastSale = productViewModel.LastSale,
                Name = productViewModel.Name,
                Price = productViewModel.Price,
                Stock = productViewModel.Stock,
                User = productViewModel.User
            };
        }

        // GET: Products/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this.productRepository.GetByIdAsync(id.Value).Result;
            if (product == null)
            {
                return NotFound();
            }
            var productViewModel = this.ProductToProductViewModel(product);
            return View(productViewModel);
        }

        private ProductViewModel ProductToProductViewModel(Product product)
        {
            return new ProductViewModel {
                Id= product.Id,
                ImageFile= null,
                ImageUrl = product.ImageUrl,
                IsAvaliable = product.IsAvaliable,
                LastPurchase = product.LastPurchase,
                LastSale = product.LastSale,
                Name = product.Name,
                Price =product.Price,
                Stock = product.Stock,
                User = product.User
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,  ProductViewModel productViewModel)
        {
            if (id != productViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var path = productViewModel.ImageUrl;

                    if (productViewModel.ImageFile != null && productViewModel.ImageFile.Length > 0)
                    {
                        var guid = Guid.NewGuid().ToString();
                        string file = $"{guid}.jpg";

                        path = Path.Combine(Directory.GetCurrentDirectory(), "Image\\products", file);
                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await productViewModel.ImageFile.CopyToAsync(stream);
                        }

                        path = $"~/Image/Products/{file}";
                    }

                    //TODO: change for logged user
                    productViewModel.User = await this.userHelper.GetUserByEmailAsync("jhevava80@gmail.com");
                    var product = this.ConvertToProduct(productViewModel, path);
                    await this.productRepository.UpdateAsync(product);
                    await this.productRepository.SaveAllAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(productViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(productViewModel);
        }

        // GET: Products/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = this.productRepository.GetByIdAsync(id.Value).Result;
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = this.productRepository.GetByIdAsync(id);
            await this.productRepository.DeleteAsync(product.Result);
            await this.productRepository.SaveAllAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return  this.productRepository.ExistAsync(id).Result  ;
        }
    }
}
