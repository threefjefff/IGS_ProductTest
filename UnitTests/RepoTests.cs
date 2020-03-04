using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Repository;
using Product = Domain.Models.Product;

namespace UnitTests
{
    public class RepoTests
    {
        [Test]
        public async Task Create_AddsToDatabase()
        {
            var opt = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(nameof(Create_AddsToDatabase))
                .Options;
            var request = new ProductRequest("Coconut", 0.5f);

            await using (var ctx = new ProductContext(opt))
            {
                var service = new ProductsRepository(ctx);
                await service.Create(request);
            }

            await using (var ctx = new ProductContext(opt))
            {
                Assert.AreEqual(1, ctx.Products.Count());
                Assert.AreEqual(0.5f, ctx.Products.Single().Price);
            }
        }

        [Test]
        public async Task Update_Existing_UpdatesProduct()
        {
            var opt = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(nameof(Update_Existing_UpdatesProduct))
                .Options;
            var request = new ProductRequest("Coconut", 0.5f);
            Product createdProduct;
            Product fetchedProduct;
            await using (var ctx = new ProductContext(opt))
            {
                var service = new ProductsRepository(ctx);
                createdProduct = await service.Create(request);
            }

            await using (var ctx = new ProductContext(opt))
            {
                createdProduct.Price = 1.5f;
                var service = new ProductsRepository(ctx);
                fetchedProduct = await service.Update(createdProduct);
            }

            await using (var ctx = new ProductContext(opt))
            {
                Assert.AreEqual(1, ctx.Products.Count());
                Assert.AreEqual(1.5f, ctx.Products.Single().Price);
                Assert.AreEqual(false, fetchedProduct.Created);
            }
        }

        [Test]
        public async Task Update_NonExisting_AddsToDatabase()
        {
            var opt = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(nameof(Update_NonExisting_AddsToDatabase))
                .Options;
            var request = new Product(12, "Coconut", 0.5f);
            Product createdProduct;
            await using (var ctx = new ProductContext(opt))
            {
                var service = new ProductsRepository(ctx);
                createdProduct = await service.Update(request);
            }

            await using (var ctx = new ProductContext(opt))
            {
                Assert.AreEqual(1, ctx.Products.Count());
                Assert.AreEqual(0.5f, ctx.Products.Single().Price);
                Assert.AreEqual(true, createdProduct.Created);
            }
        }

        [Test]
        public async Task Get_Existing_ReturnsProduct()
        {
            var opt = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(nameof(Get_Existing_ReturnsProduct))
                .Options;
            var request = new ProductRequest("Coconut", 0.5f);
            Product createdProduct;
            Product fetchedProduct;
            await using (var ctx = new ProductContext(opt))
            {
                var service = new ProductsRepository(ctx);
                createdProduct = await service.Create(request);
            }

            await using (var ctx = new ProductContext(opt))
            {
                var service = new ProductsRepository(ctx);
                fetchedProduct = await service.Get(createdProduct.Id);
            }

            await using (var ctx = new ProductContext(opt))
            {
                Assert.AreEqual(1, ctx.Products.Count());
                Assert.AreEqual(0.5f, ctx.Products.Single().Price);
                Assert.AreEqual(false, fetchedProduct.Created);
            }
        }

        [Test]
        public async Task Get_NotExisting_ThrowsProductNotFoundException()
        {
            var opt = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(nameof(Get_NotExisting_ThrowsProductNotFoundException))
                .Options;

            await using var ctx = new ProductContext(opt);
            var service = new ProductsRepository(ctx);
            Assert.ThrowsAsync<ProductNotFoundException>(() => service.Get(12));
        }

        [Test]
        public async Task Delete_Existing_RemovesFromDatabase()
        {
            var opt = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(nameof(Delete_Existing_RemovesFromDatabase))
                .Options;
            var request = new ProductRequest("Coconut", 0.5f);
            Product createdProduct;
            await using (var ctx = new ProductContext(opt))
            {
                var service = new ProductsRepository(ctx);
                createdProduct = await service.Create(request);
            }

            await using (var ctx = new ProductContext(opt))
            {
                var service = new ProductsRepository(ctx);
                await service.Delete(createdProduct.Id);
            }

            await using (var ctx = new ProductContext(opt))
            {
                Assert.AreEqual(0, ctx.Products.Count());
            }
        }

        [Test]
        public async Task Delete_NotExisting_ThrowsProductNotFoundException()
        {
            var opt = new DbContextOptionsBuilder<ProductContext>()
                .UseInMemoryDatabase(nameof(Delete_NotExisting_ThrowsProductNotFoundException))
                .Options;

            await using var ctx = new ProductContext(opt);
            var service = new ProductsRepository(ctx);
            Assert.ThrowsAsync<ProductNotFoundException>(() => service.Delete(12));
        }
    }
}
