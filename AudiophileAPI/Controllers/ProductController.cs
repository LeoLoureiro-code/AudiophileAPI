using AudiophileAPI.DataAccess.EF.Interfaces;
using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Repositories;
using AudiophileAPI.DTO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace AudiophileAPI.Controllers
{
    [Route("audiophile/Product")]
    [ApiController]

    public class ProductController : ControllerBase
    {

        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpGet("all-products")]

        public async Task<ActionResult<IEnumerable<Product>>> GetAllProducts()
        {
            try
            {
                var product = await _productRepository.GetAllProducts();

                if (product == null || !product.Any())
                {
                    return NotFound("No products found");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error ocurred while fetching products",
                    statusCode: StatusCodes.Status500InternalServerError
                    );
            }
        }

        [HttpGet("find-by-id/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProductById(id);

                if (product == null)
                {
                    return NotFound(new
                    {
                        Message = $"Product with ID {id} was not found."
                    });
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error ocurred while fetching the product",
                    statusCode: StatusCodes.Status500InternalServerError
                    );
            }
        }

        [HttpPost("create-product")]

        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        Message = "All fields are required."
                    });
                }

                var createdProduct = await _productRepository.AddProduct(product);
                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
            }

            catch (Exception ex) 
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while creating the product.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut("update-product/{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] ProductDTO productDto)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(productDto.Name) ||
                    string.IsNullOrWhiteSpace(productDto.Description) ||
                    string.IsNullOrWhiteSpace(productDto.Features) ||
                    productDto.Price <= 0 ||
                    productDto.Stock < 0)
                {
                    return BadRequest("Invalid product data. Please check all fields.");
                }

                var updated = await _productRepository.UpdateProduct(
                    id,
                    productDto.Name,
                    productDto.Description,
                    productDto.Features,
                    productDto.Price,
                    productDto.Stock,
                    productDto.CategoryId,
                    productDto.ImageURL
                );

                if (updated == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while updating the product.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProductById(id);
                if (product == null)
                {
                    return NotFound($"Product with ID {id} not found.");
                }

                await _productRepository.DeleteProduct(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: ex.Message,
                    title: "An error occurred while deleting the product.",
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
