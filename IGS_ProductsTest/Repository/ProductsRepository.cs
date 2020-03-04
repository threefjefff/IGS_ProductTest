using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using ProductDTO = Domain.Models.Product;

namespace Repository
{
    public class ProductsRepository : IProductRepository
    {
        private readonly ProductContext _context;

        public ProductsRepository(ProductContext context)
        {
            _context = context;
        }

        public async Task<ProductDTO> Update(ProductDTO update)
        {
            try
            {
                var product = await Find(update.Id);
                product.Name = update.Name;
                product.Price = update.Price;
                await _context.SaveChangesAsync();

                return product.ToDomain();
            }
            catch (ProductNotFoundException)
            {
                return await Create(new Product
                {
                    Id = update.Id, 
                    Name = update.Name, 
                    Price = update.Price
                });
            }
        }

        private async Task<ProductDTO> Create(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product.ToDomain(true);
        }

        public async Task<ProductDTO> Create(ProductRequest update)
        {
            var product = new Product
            {
                Name = update.Name,
                Price = update.Price
            };
            return await Create(product);
        }

        public async Task<ProductDTO> Get(int code)
        {
            var product = await Find(code);
            return product.ToDomain();
        }

        public async Task<IEnumerable<ProductDTO>> Get()
        {
            return await _context.Products
                .Select(p => new ProductDTO(p.Id, p.Name, p.Price, false))
                .ToListAsync();
        }

        public async Task Delete(int code)
        {
            var product = await Find(code);
            _context.Remove(product);
            await _context.SaveChangesAsync();
        }

        private async Task<Product> Find(int code)
        {
            var storedProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == code);
            if (storedProduct == null)
            {
                throw new ProductNotFoundException();
            }

            return storedProduct;
        }
    }
}
