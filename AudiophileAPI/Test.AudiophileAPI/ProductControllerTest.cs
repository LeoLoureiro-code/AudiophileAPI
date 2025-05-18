using AudiophileAPI.Controllers;
using AudiophileAPI.DataAccess.EF.Interfaces;
using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Repositories;
using AudiophileAPI.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace AudiophileAPI.Test.AudiophileAPI
{
    public class ProductControllerTest
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductController _productController;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IProductRepository>();
            _productController = new ProductController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllProducts_ReturnProducts_WhenUsersExist()
        {
            //Arrange
            var testProducts = new List<Product>
            {
                new Product {
                    ProductId = 1,
                    Description = "earphones",
                    Features = "features",
                    Price = 99.99M,
                    Stock = 10,
                    CategoryId = 2,
                    ImageUrl = "Image.png"
                },
                 new Product {
                    ProductId = 2,
                    Description = "earphones",
                    Features = "features",
                    Price = 99.99M,
                    Stock = 10,
                    CategoryId = 2,
                    ImageUrl = "Image.png"
                }
            };

            _mockRepo.Setup(r => r.GetAllProducts()).ReturnsAsync(testProducts);

            //Act
            var result = await _productController.GetAllProducts();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProducts = Assert.IsAssignableFrom<List<Product>>(okResult.Value);
            Assert.Equal(testProducts.Count, returnProducts.Count);
        }

        [Fact]

        public async Task GetAllProducts_ReturnNotFound_WhenProductsDoesNotExist()
        {
            var testProducts = new List<Product>();

            _mockRepo.Setup(r => r.GetAllProducts()).ReturnsAsync(testProducts);

            //Act
            var result = await _productController.GetAllProducts();

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);

        }

        [Fact]
        public async Task GetProduct_ReturnProduct_WhenUserExists()
        {
            //Arrange
            int testProductId = 1;
            var testProduct = new Product
            {
                ProductId = testProductId,
                Description = "earphones",
                Features = "features",
                Price = 99.99M,
                Stock = 10,
                CategoryId = 2,
                ImageUrl = "Image.png"
            };

            _mockRepo.Setup(repo => repo.GetProductById(testProductId)).ReturnsAsync(testProduct);

            //Act
            var result = await _productController.GetProduct(testProductId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(testProductId, returnProduct.ProductId);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WHenProductDoesNotExists()
        {
            // Arrange
            int testProductId = 999;
            _mockRepo.Setup(repo => repo.GetProductById(testProductId))
                     .ThrowsAsync(new Exception("Product not found"));

            // Act
            var result = await _productController.GetProduct(testProductId);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAt_WhenValidProduct()
        {
            // Arrange
            var productInput = new ProductDTO
            {
                Name = "name",
                Description = "earphones",
                Features = "features",
                Price = 99.99M,
                Stock = 10,
                CategoryId = 2,
                ImageURL = "Image.png"
            };

            var createdProduct = new Product
            {
                ProductId = 1,
                Name = "name",
                Description = "earphones",
                Features = "features",
                Price = 99.99M,
                Stock = 10,
                CategoryId = 2,
                ImageUrl = "Image.png"
            };

            _mockRepo.Setup(r => r.AddProduct(productInput)).ReturnsAsync(createdProduct);

            // Act
            var result = await _productController.CreateProduct(productInput);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal(1, returnProduct.ProductId);
        }

        [Theory]
        [InlineData("Name", "Description", "Features", 10, 1, 1, "URL")]
        [InlineData("", "Description", "Features", 10, 1, 1, "URL")]
        [InlineData("Name", "", "Features", 10, 1, 1, "URL")]
        [InlineData("Name", "Description", "", 10, 1, 1, "URL")]
        [InlineData("Name", "Description", "Features", 10, 1, 1, "")]
        public async Task CreateProduct_ReturnBadRequest_WhenFieldsMissing(
            string productName, string productDescription, string productFeatures, decimal productPrice, int productStock, int productCategoryId, string productImageURL
            )
        {
            //Arrange

            var productDTO = new ProductDTO
            {
                Name = productName,
                Description = productDescription,
                Features = productFeatures,
                Price = productPrice,
                Stock = productStock,
                CategoryId = productCategoryId,
                ImageURL = productImageURL
            };

            ValidateModel(productDTO, _productController);

            //Act
            var result = await _productController.CreateProduct( productDTO );

            //Assert

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);

        }


        [Fact]
        public async Task CreateProduct_ReturnsProblem_WhenExceptionThrown()
        {

            //Arrange
            var productDto = new ProductDTO { Name = "name", Description = "deacription", Features = "features", Price= 10, Stock =1, CategoryId = 1, ImageURL="URL" };

            _mockRepo.Setup(r => r.AddProduct(productDto)).ThrowsAsync(new Exception("Database error"));

            //Act
            var result = await _productController.CreateProduct(productDto);

            //Assert
            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_WhenProductIsUpdated()
        {
            // Arrange
            var productDto = new ProductDTO
            {
                Name = "Updated",
                Description = "desc",
                Features = "feat",
                Price = 10,
                Stock = 1,
                CategoryId = 1,
                ImageURL = "URL"
            };

            var updatedProduct = new Product
            {
                ProductId = 1,
                Name = productDto.Name,
                Description = productDto.Description,
                Features = productDto.Features,
                Price = productDto.Price,
                Stock = productDto.Stock,
                CategoryId = productDto.CategoryId,
                ImageUrl = productDto.ImageURL
            };

            _mockRepo.Setup(r => r.UpdateProduct(
                1,
                productDto.Name,
                productDto.Description,
                productDto.Features,
                productDto.Price,
                productDto.Stock,
                productDto.CategoryId,
                productDto.ImageURL
            )).ReturnsAsync(updatedProduct);

            // Act
            var result = await _productController.UpdateProduct(1, productDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            //Arrange
            var productDto = new ProductDTO
            {
                Name = "Name",
                Description = "Desc",
                Features = "Features",
                Price = 10,
                Stock = 1,
                CategoryId = 1,
                ImageURL = "URL"
            };

            _mockRepo.Setup(r => r.UpdateProduct(
                1,
                productDto.Name,
                productDto.Description,
                productDto.Features,
                productDto.Price,
                productDto.Stock,
                productDto.CategoryId,
                productDto.ImageURL
            )).ReturnsAsync((Product)null!);


            var result = await _productController.UpdateProduct(1, productDto);

            //Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenProductIsDeleted()
        {
            //Arrange
            var product = new Product { ProductId = 1 };

            _mockRepo.Setup(r => r.GetProductById(1)).ReturnsAsync(product);
            _mockRepo.Setup(r => r.DeleteProduct(1)).Returns(Task.CompletedTask);

            //Act
            var result = await _productController.DeleteProduct(1);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            //Arrange
            _mockRepo.Setup(r => r.GetProductById(999)).ReturnsAsync((Product)null!);

            //Act
            var result = await _productController.DeleteProduct(999);

            //Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsProblem_WhenExceptionThrown()
        {
            //Arrange
            _mockRepo.Setup(r => r.GetProductById(1)).ThrowsAsync(new Exception("DB crash"));

            //Act
            var result = await _productController.DeleteProduct(1);

            //Assert
            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
        }

        private void ValidateModel(object model, ControllerBase controller)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);

            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
        }
    }

}
