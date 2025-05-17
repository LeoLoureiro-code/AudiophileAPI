using AudiophileAPI.Controllers;
using AudiophileAPI.DTO;
using AudiophileAPI.DataAccess.EF.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using AudiophileAPI.DataAccess.EF.Models;
using AudiophileAPI.DataAccess.EF.Services;
using AudiophileAPI.DataAccess.EF.Interfaces;
using System.ComponentModel.DataAnnotations;


namespace AudiophileAPI.Test.AudiophileAPI
{
    public class UserControllerTest
    {
        private readonly Mock<IUsersRepository> _mockRepo;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly UserController _userController;

        public UserControllerTest()
        {
            _mockRepo = new Mock<IUsersRepository>();
            _mockPasswordService = new Mock<IPasswordService>();
            _userController = new UserController(_mockRepo.Object, _mockPasswordService.Object);
        }

        [Fact]
        public async Task GetAllUsers_ReturnUsers_WhenUsersExist()
        {
            //Arrange
            var testUsers = new List<User>
            {
                new User {
                    UsersId = 1,
                    FirstName = "Leo",
                    LastName = "NoLastName",
                    Email = "Leo@test.com",
                    PasswordHash = "lskdmlksamdklas",
                    Role = "admin"
                },
                new User {
                    UsersId = 2,
                    FirstName = "Eol",
                    LastName = "NoLastName",
                    Email = "Leo@test.com",
                    PasswordHash = "lskdmlksamdklas",
                    Role = "user"
                }
            };

            _mockRepo.Setup(r => r.GetAllUsers()).ReturnsAsync(testUsers);

            //Act
            var result = await _userController.GetAllUsers();

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnUsers = Assert.IsAssignableFrom<List<User>>(okResult.Value);
            Assert.Equal(testUsers.Count, returnUsers.Count);
        }

        [Fact]

        public async Task GetAllUsers_ReturnNotFound_WhenUsersDoesNotExist()
        {
            var testUsers = new List<User>();

            _mockRepo.Setup(r => r.GetAllUsers()).ReturnsAsync(testUsers);

            //Act
            var result = await _userController.GetAllUsers();

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);

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

        [Fact]
        public async Task CreateUser_ReturnsCreatedAt_WhenValidUser()
        {
            // Arrenge
            var userDto = new UsersDTO { Email = "test@example.com", Password = "Password123" };
            var createdUser = new User { UsersId = 1, Email = userDto.Email, PasswordHash = "hashed" };

            _mockRepo.Setup(r => r.CreateUser(userDto)).ReturnsAsync(createdUser);
            
            // Act
            var result = await _userController.CreateUser(userDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnUser = Assert.IsType<User>(createdResult.Value);
            Assert.Equal(1, returnUser.UsersId);
        }

        //Fix This
        [Theory]
        [InlineData("FirstName", "LastName", null, "Password123", "admin")]
        [InlineData("FirstName", "LastName", "test@example.com", null, "admin")]
        [InlineData("FirstName", "LastName", "", "Password123", "admin")]
        public async Task CreateUser_ReturnsBadRequest_WhenMissingFields(
        string firstName, string lastName, string email, string password, string role)
        {
            // Arrange
            var userDto = new UsersDTO
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Password = password,
                Role = role
            };

            ValidateModel(userDto, _userController);

            // Act
            var result = await _userController.CreateUser(userDto);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task CreateUser_ReturnsProblem_WhenExceptionThrown()
        {

            //Arrange
            var userDto = new UsersDTO { Email = "test@example.com", Password = "Password123" };

            _mockRepo.Setup(r => r.CreateUser(userDto)).ThrowsAsync(new Exception("Database error"));

            //Act
            var result = await _userController.CreateUser(userDto);

            //Assert
            var problem = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, problem.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenUserIsUpdated()
        {

            //Arrange
            var user = new User
            {
                UsersId = 1,
                FirstName = "Updated",
                LastName = "User",
                Email = "updated@example.com",
                PasswordHash = "original",
                Role = "admin"
            };

            _mockRepo.Setup(r => r.UpdateUser(
                user.UsersId, user.FirstName, user.LastName, user.Email, It.IsAny<string>(), user.Role))
                .ReturnsAsync(user);

            _mockPasswordService.Setup(p => p.HashPassword(user.PasswordHash)).Returns("hashed");

            //Act
            var result = await _userController.UpdateUser(user.UsersId, user);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            //Arrange
            var user = new User { UsersId = 2, Email = "test@test.com", PasswordHash = "pass" };

            //Act
            var result = await _userController.UpdateUser(1, user);

            //Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            //Arrange
            var user = new User
            {
                UsersId = 1,
                Email = "nonexistent@test.com",
                PasswordHash = "pass",
                FirstName = "No",
                LastName = "User",
                Role = "user"
            };

            _mockRepo.Setup(r => r.UpdateUser(
                user.UsersId, user.FirstName, user.LastName, user.Email, It.IsAny<string>(), user.Role))
                .ReturnsAsync((User)null!);

            _mockPasswordService.Setup(p => p.HashPassword(user.PasswordHash)).Returns("hashed");

            //Act
            var result = await _userController.UpdateUser(user.UsersId, user);

            //Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNoContent_WhenUserIsDeleted()
        {
            //Arrange
            var user = new User { UsersId = 1 };

            _mockRepo.Setup(r => r.GetUserById(1)).ReturnsAsync(user);
            _mockRepo.Setup(r => r.DeleteUser(1)).Returns(Task.CompletedTask);

            //Act
            var result = await _userController.DeleteUser(1);

            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            //Arrange
            _mockRepo.Setup(r => r.GetUserById(999)).ReturnsAsync((User)null!);

            //Act
            var result = await _userController.DeleteUser(999);

            //Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFound.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_ReturnsProblem_WhenExceptionThrown()
        {
            //Arrange
            _mockRepo.Setup(r => r.GetUserById(1)).ThrowsAsync(new Exception("DB crash"));

            //Act
            var result = await _userController.DeleteUser(1);

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
