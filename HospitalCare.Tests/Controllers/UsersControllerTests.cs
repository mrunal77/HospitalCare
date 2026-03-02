using System.Security.Claims;
using FluentAssertions;
using HospitalCare.Api.Controllers;
using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using HospitalCare.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace HospitalCare.Tests.Controllers;

[TestFixture]
public class UsersControllerTests
{
    private Mock<IAuthService> _authServiceMock = null!;
    private UsersController _controller = null!;
    private TestFixture _testFixture = null!;

    [SetUp]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new UsersController(_authServiceMock.Object);
        _testFixture = new TestFixture();
    }

    #region Get All Tests

    [Test]
    public async Task GetAll_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<UserDto>
        {
            new UserDto(Guid.NewGuid(), "user1@example.com", "John", "Doe", "Patient", true, DateTime.UtcNow),
            new UserDto(Guid.NewGuid(), "user2@example.com", "Jane", "Smith", "Doctor", true, DateTime.UtcNow)
        };

        _authServiceMock
            .Setup(x => x.GetAllUsersAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedUsers = okResult?.Value as IEnumerable<UserDto>;
        returnedUsers.Should().HaveCount(2);
    }

    #endregion

    #region Get By Id Tests

    [Test]
    public async Task GetById_ExistingUser_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserDto(userId, "test@example.com", "John", "Doe", "Patient", true, DateTime.UtcNow);

        _authServiceMock
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.GetById(userId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(user);
    }

    [Test]
    public async Task GetById_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _authServiceMock
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetById(userId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_ValidData_ReturnsCreated()
    {
        // Arrange
        var registerDto = new RegisterUserDto("newuser@example.com", "password123", "John", "Doe", "Patient");

        var authResponse = new AuthResponseDto(
            "token",
            "newuser@example.com",
            "John",
            "Doe",
            "Patient",
            DateTime.UtcNow.AddDays(1)
        );

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _controller.Create(registerDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public async Task Create_UserAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterUserDto("existing@example.com", "password123", "John", "Doe", "Patient");

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto))
            .ReturnsAsync((AuthResponseDto?)null);

        // Act
        var result = await _controller.Create(registerDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Enable User Tests

    [Test]
    public async Task EnableUser_ExistingUser_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _authServiceMock
            .Setup(x => x.EnableUserAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.EnableUser(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task EnableUser_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _authServiceMock
            .Setup(x => x.EnableUserAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.EnableUser(userId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Disable User Tests

    [Test]
    public async Task DisableUser_ExistingUser_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _authServiceMock
            .Setup(x => x.DisableUserAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DisableUser(userId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task DisableUser_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _authServiceMock
            .Setup(x => x.DisableUserAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DisableUser(userId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Delete User Tests

    [Test]
    public async Task DeleteUser_ExistingUser_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _authServiceMock
            .Setup(x => x.DeleteUserAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task DeleteUser_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _authServiceMock
            .Setup(x => x.DeleteUserAsync(userId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Reset Password Tests

    [Test]
    public async Task ResetPassword_ExistingUser_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = "newpassword123";

        _authServiceMock
            .Setup(x => x.ResetUserPasswordAsync(userId, newPassword))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.ResetPassword(userId, newPassword);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task ResetPassword_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var newPassword = "newpassword123";

        _authServiceMock
            .Setup(x => x.ResetUserPasswordAsync(userId, newPassword))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.ResetPassword(userId, newPassword);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}