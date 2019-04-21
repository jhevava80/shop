using Microsoft.AspNetCore.Http;
using Shop.Web.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Web.Models
{
    public class ProductViewModel : Product
    {
        [DisplayName("Image")]
        public IFormFile ImageFile { get; set; }
    }
}
