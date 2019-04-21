using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Common.Models
{
    public class Product
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public long Price { get; set; }

        [JsonProperty("imageUrl")]
        public object ImageUrl { get; set; }

        [JsonProperty("lastPurchase")]
        public object LastPurchase { get; set; }

        [JsonProperty("lastSale")]
        public object LastSale { get; set; }

        [JsonProperty("isAvaliable")]
        public bool IsAvaliable { get; set; }

        [JsonProperty("stock")]
        public long Stock { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("imageFullPath")]
        public object ImageFullPath { get; set; }

        public override string ToString()
        {
            return $"{this.Name} {this.Price:C2}";
        }
    }
}
