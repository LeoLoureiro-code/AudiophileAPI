using AudiophileAPI.Controllers;
using AudiophileAPI.DataAccess.EF.Interfaces;
using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
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

    }

    
}
