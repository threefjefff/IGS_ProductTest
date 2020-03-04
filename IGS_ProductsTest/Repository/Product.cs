using System.Runtime.CompilerServices;

namespace Repository
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }

        public Domain.Models.Product ToDomain(bool created = false)
        {
            return new Domain.Models.Product(Id, Name, Price, created);
        }
    }
}
