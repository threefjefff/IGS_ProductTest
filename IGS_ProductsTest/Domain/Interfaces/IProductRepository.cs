using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Interfaces
{
    public interface IProductRepository
    {
        public Task<Product> Create(ProductRequest req);
        public Task<Product> Update(Product req);
        public Task<Product> Get(int code);
        public Task<IEnumerable<Product>> Get();
        public Task Delete(int code);
    }
}
