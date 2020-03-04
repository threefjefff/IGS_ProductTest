using System;
using System.Text.Json.Serialization;

namespace Domain.Models
{
    public class Product : ProductBase
    {
        public Product(int id, string name, float price, bool created = false)
        {
            Id = id;
            Name = name;
            Price = price;
            Created = created;
        }
        public int Id { get; set; }
        [JsonIgnore]
        public bool Created { get; set; }
    }
}
