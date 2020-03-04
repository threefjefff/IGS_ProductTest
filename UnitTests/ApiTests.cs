using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using API.Controllers;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace UnitTests
{
    public class ApiTests
    {
        [Test]
        public async Task Create_NegativePrice_ReturnsBadRequest()
        {
            var request = new ProductRequest("Coconut", -0.5f);
            var controller = new ProductsController(new Mock<IProductRepository>().Object);

            var response = await controller.Post(request, ApiVersion.Default);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Create_ZeroPrice_ReturnsOK()
        {
            var request = new ProductRequest("Coconut", 0f);
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Create(request))
                .ReturnsAsync(new Product(56, "Coconut", 0f));
            var controller = new ProductsController(repoMock.Object);

            var response = await controller.Post(request, ApiVersion.Default);

            Assert.IsInstanceOf<CreatedAtRouteResult>(response);
            var result = (CreatedAtRouteResult)response;
            Assert.AreEqual("GetById", result.RouteName);
            Assert.AreEqual(56, result.RouteValues["id"]);
            var product = (Product)result.Value;
            Assert.AreEqual(56, product.Id);
            Assert.AreEqual("Coconut", product.Name);
            Assert.AreEqual(0f, product.Price);
        }

        [Test]
        public async Task Create_ValidProduct_ReturnsOK()
        {
            var request = new ProductRequest("Coconut", 0.5f);
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Create(request))
                .ReturnsAsync(new Product(56, "Coconut", 0.5f));
            var controller = new ProductsController(repoMock.Object);

            var response = await controller.Post(request, ApiVersion.Default);

            Assert.IsInstanceOf<CreatedAtRouteResult>(response);
            var result = (CreatedAtRouteResult) response;
            Assert.AreEqual("GetById", result.RouteName);
            Assert.AreEqual(56, result.RouteValues["id"]);
            var product = (Product)result.Value;
            Assert.AreEqual(56,product.Id);
            Assert.AreEqual("Coconut", product.Name);
            Assert.AreEqual(0.5f, product.Price);
        }

        [Test]
        public async Task Put_NegativePrice_ReturnsBadRequest()
        {
            var request = new ProductRequest("Coconut", -0.5f);
            var controller = new ProductsController(new Mock<IProductRepository>().Object);

            var response = await controller.Put(request, 98, ApiVersion.Default);

            Assert.IsInstanceOf<BadRequestObjectResult>(response);
        }

        [Test]
        public async Task Put_ZeroPrice_ReturnsOK()
        {
            var request = new ProductRequest("Coconut", 0f);
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Update(It.IsAny<Product>()))
                .ReturnsAsync(new Product(42, "Coconut", 0f));
            var controller = new ProductsController(repoMock.Object);

            var response = await controller.Put(request, 42, ApiVersion.Default);

            Assert.IsInstanceOf<OkObjectResult>(response);
            var product = (Product)((OkObjectResult)response).Value;
            Assert.AreEqual(42, product.Id);
            Assert.AreEqual("Coconut", product.Name);
            Assert.AreEqual(0f, product.Price);
        }

        [Test]
        public async Task Put_NotExisting_ReturnsOK()
        {
            var request = new ProductRequest("Coconut", 0.5f);
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Update(It.IsAny<Product>()))
                .ReturnsAsync(new Product(24, "Coconut", 0.5f, true));
            var controller = new ProductsController(repoMock.Object);

            var response = await controller.Put(request, 24, ApiVersion.Default);

            Assert.IsInstanceOf<CreatedAtRouteResult>(response);
            var result = (CreatedAtRouteResult)response;
            Assert.AreEqual("GetById", result.RouteName);
            Assert.AreEqual(24, result.RouteValues["id"]);
            var product = (Product)result.Value;
            Assert.AreEqual(24, product.Id);
            Assert.AreEqual("Coconut", product.Name);
            Assert.AreEqual(0.5f, product.Price);
        }

        [Test]
        public async Task Put_Existing_ReturnsOK()
        {
            var request = new ProductRequest( "Coconut", 0.5f);
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Update(It.IsAny<Product>()))
                .ReturnsAsync(new Product(24, "Coconut", 0.5f));
            var controller = new ProductsController(repoMock.Object);

            var response = await controller.Put(request, 24, ApiVersion.Default);

            Assert.IsInstanceOf<OkObjectResult>(response);
            var product = (Product)((OkObjectResult)response).Value;
            Assert.AreEqual(24, product.Id);
            Assert.AreEqual("Coconut", product.Name);
            Assert.AreEqual(0.5f, product.Price);
        }

        [Test]
        public async Task GetAll_ReturnsAllProducts()
        {
            var expectedProducts = new List<Product>
            {
                new Product(8024, "a", 12.4f),
                new Product(1241, "b", 94f),
                new Product(12348, "c", 123f),
                new Product(1293849, "d", 143f)
            };
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Get())
                .ReturnsAsync(expectedProducts);
            var controller = new ProductsController(repoMock.Object);

            var response = await controller.Get();

            Assert.IsInstanceOf<OkObjectResult>(response);
            var list = (List<Product>)(((OkObjectResult)response).Value);
            Assert.AreEqual(4, list.Count);
        }

        [Test]
        public async Task Get_ProductExists_ReturnsProduct()
        {
            var productId = 127;
            var expectedProduct = new Product(productId, "c", 123f);
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Get(productId))
                .ReturnsAsync(expectedProduct);
            var controller = new ProductsController(repoMock.Object);

            var response = await controller.GetById(productId);

            Assert.IsInstanceOf<OkObjectResult>(response);
            var product = (Product)(((OkObjectResult)response).Value);
            Assert.AreEqual(productId, product.Id);
            Assert.AreEqual("c", product.Name);
            Assert.AreEqual(123f, product.Price);
        }

        [Test]
        public async Task Get_ProductNotFound_ReturnsNotFound()
        {
            var productId = 127;
            var expectedProduct = new Product(productId, "c", 123f);
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Get(productId))
                .ThrowsAsync(new ProductNotFoundException());
            var controller = new ProductsController(repoMock.Object);

            var response = await controller.GetById(productId);

            Assert.IsInstanceOf<NotFoundResult>(response);
        }

        [Test]
        public async Task Delete_ProductFound_ReturnsNoContent()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Delete(1234))
                .Returns(Task.CompletedTask);
            var controller = new ProductsController(repoMock.Object);

            var result = await controller.Delete(1234);
            
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task Delete_ProductNotFound_ReturnsNoContent()
        {
            var repoMock = new Mock<IProductRepository>();
            repoMock.Setup(x => x.Delete(1234))
                .ThrowsAsync(new ProductNotFoundException());
            var controller = new ProductsController(repoMock.Object);

            var result = await controller.Delete(1234);

            Assert.IsInstanceOf<NoContentResult>(result);
        }
    }
}