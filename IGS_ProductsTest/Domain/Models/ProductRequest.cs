namespace Domain.Models
{
    public class ProductRequest : ProductBase
    {
        public ProductRequest()
        {
        }

        public ProductRequest(string name, float price)
        {
            Name = name;
            Price = price;
        }
    }
}