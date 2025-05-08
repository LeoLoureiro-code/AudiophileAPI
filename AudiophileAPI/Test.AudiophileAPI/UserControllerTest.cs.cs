using AudiophileAPI.Controllers;
using AudiophileAPI.DTO;
using AudiophileAPI.DataAccess.EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Services;


namespace AudiophileAPI.Test.AudiophileAPI
{
    public class UserControllerTest
    {
        private readonly Mock<UsersRepository> _mockRepo;
        private readonly Mock<PasswordService> _mockPasswordService;
        private readonly UserController _userController;

        public UserControllerTest()
        {
            _mockRepo = new Mock<UsersRepository>();
            _mockPasswordService = new Mock<PasswordService>();
            _userController = new UserController(_mockRepo.Object, _mockPasswordService.Object);
        }

        [Fact]
        public async Task GetUser_ReturnUser_WhenUserExists()
        {
            //Arrange
            int testUserId = 1;
            var testUser = new User
            {
                UsersId = testUserId,
                FirstName = "Leo",
                LastName = "NoLastName",
                Email = "Leo@test.com",
                PasswordHash = "lskdmlksamdklas"
            };

            _mockRepo.Setup(repo => repo.GetUserById(testUserId)).ReturnsAsync(testUser);

            //Act
            var result = await _userController.GetUser(testUserId);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(testUserId, returnUser.UsersId);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WHenUserDoesNotExists()
        {
            // Arrange
            int testUserId = 999;
            _mockRepo.Setup(repo => repo.GetUserById(testUserId))
                     .ThrowsAsync(new Exception("User not found"));

            // Act
            var result = await _userController.GetUser(testUserId);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);
        }
    }

}
