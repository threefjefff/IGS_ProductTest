using System;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("v{version:apiVersion}/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductsController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ProductRequest req, ApiVersion ver)
        {
            Validate(req);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _productRepository.Create(req);

            return CreatedAtRoute(nameof(GetById), new {id = result.Id, version = ver.ToString()}, result);
        }

        [HttpPut]
        [Route("{id:int:min(1)}", Name = nameof(Put))]
        public async Task<IActionResult> Put(ProductRequest product, int id, ApiVersion ver)
        {
            Validate(product);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _productRepository.Update(new Product(id, product.Name, product.Price));
            if (result.Created)
            {
                return CreatedAtRoute(nameof(GetById), new { id = result.Id, version = ver.ToString() }, result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _productRepository.Get());
        }

        [HttpGet]
        [Route("{id:int:min(1)}", Name = nameof(GetById))]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var product = await _productRepository.Get(id);
                return Ok(product);
            }
            catch (ProductNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete]
        [Route("{id:int:min(1)}", Name = nameof(Delete))]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productRepository.Delete(id);
            }
            catch (ProductNotFoundException)
            {
            }

            return NoContent();
        }

        private void Validate(ProductBase product)
        {
            if (product.Price < 0)
            {
                ModelState.AddModelError(nameof(product.Price), "Price must be non-negative");
            }
        }
    }
}
