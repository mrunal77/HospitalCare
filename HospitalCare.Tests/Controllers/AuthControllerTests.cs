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
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock = null!;
    private AuthController _controller = null!;
    private TestFixture _testFixture = null!;

    [SetUp]
    public void Setup()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
        _testFixture = new TestFixture();
    }

    #region Login Tests

    [Test]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "password123");
        var authResponse = new AuthResponseDto(
            "token",
            "test@example.com",
            "John",
            "Doe",
            "Patient",
            DateTime.UtcNow.AddDays(1)
        );

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync((authResponse, false));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(authResponse);
    }

    [Test]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "wrongpassword");

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync(((AuthResponseDto?, bool))((null, false)));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Test]
    public async Task Login_InactiveUser_ReturnsUnauthorizedWithDeactivatedMessage()
    {
        // Arrange
        var loginDto = new LoginDto("test@example.com", "password123");

        _authServiceMock
            .Setup(x => x.LoginAsync(loginDto))
            .ReturnsAsync(((AuthResponseDto?, bool))((null, true)));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult?.Value?.ToString().Should().Contain("deactivated");
    }

    #endregion

    #region Register Tests

    [Test]
    public async Task Register_ValidData_ReturnsCreated()
    {
        // Arrange
        var registerDto = new RegisterUserDto(
            "newuser@example.com",
            "password123",
            "John",
            "Doe",
            "Patient"
        );

        var authResponse = new AuthResponseDto(
            "token",
            "newuser@example.com",
            "John",
            "Doe",
            "Patient",
            DateTime.UtcNow.AddDays(1)
        );

        _authServiceMock
            .Setup(x => x.UserExistsAsync(registerDto.Email))
            .ReturnsAsync(false);

        _authServiceMock
            .Setup(x => x.RegisterAsync(registerDto))
            .ReturnsAsync(authResponse);

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(
            new List<Claim> { new(ClaimTypes.Role, "Admin") }
        );

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public async Task Register_UserAlreadyExists_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterUserDto(
            "existing@example.com",
            "password123",
            "John",
            "Doe",
            "Patient"
        );

        _authServiceMock
            .Setup(x => x.UserExistsAsync(registerDto.Email))
            .ReturnsAsync(true);

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(
            new List<Claim> { new(ClaimTypes.Role, "Admin") }
        );

        // Act
        var result = await _controller.Register(registerDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Change Password Tests

    [Test]
    public async Task ChangePassword_ValidData_ReturnsNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var changePasswordDto = new ChangePasswordDto("oldpassword", "newpassword123");

        _authServiceMock
            .Setup(x => x.ChangePasswordAsync(userId, changePasswordDto))
            .ReturnsAsync(true);

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(
            new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }
        );

        // Act
        var result = await _controller.ChangePassword(changePasswordDto);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task ChangePassword_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var changePasswordDto = new ChangePasswordDto("wrongpassword", "newpassword123");

        _authServiceMock
            .Setup(x => x.ChangePasswordAsync(userId, changePasswordDto))
            .ReturnsAsync(false);

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(
            new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) }
        );

        // Act
        var result = await _controller.ChangePassword(changePasswordDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Get Current User Tests

    [Test]
    public void GetCurrentUser_AuthenticatedUser_ReturnsUserInfo()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "test@example.com"),
            new(ClaimTypes.GivenName, "John"),
            new(ClaimTypes.Surname, "Doe"),
            new(ClaimTypes.Role, "Patient")
        };

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(claims);

        // Act
        var result = _controller.GetCurrentUser();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Get All Users Tests

    [Test]
    public async Task GetAllUsers_AdminRole_ReturnsUsersList()
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

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(
            new List<Claim> { new(ClaimTypes.Role, "Admin") }
        );

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedUsers = okResult?.Value as IEnumerable<UserDto>;
        returnedUsers.Should().HaveCount(2);
    }

    #endregion

    #region Reset Password Tests

    [Test]
    public async Task ResetUserPassword_ValidData_ReturnsOk()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resetPasswordDto = new ResetPasswordDto("newpassword123");

        _authServiceMock
            .Setup(x => x.ResetUserPasswordAsync(userId, resetPasswordDto.NewPassword))
            .ReturnsAsync(true);

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(
            new List<Claim> { new(ClaimTypes.Role, "Admin") }
        );

        // Act
        var result = await _controller.ResetUserPassword(userId, resetPasswordDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task ResetUserPassword_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resetPasswordDto = new ResetPasswordDto("");

        _authServiceMock
            .Setup(x => x.ResetUserPasswordAsync(userId, resetPasswordDto.NewPassword))
            .ReturnsAsync(false);

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(
            new List<Claim> { new(ClaimTypes.Role, "Admin") }
        );

        // Act
        var result = await _controller.ResetUserPassword(userId, resetPasswordDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task ResetUserPassword_EmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var resetPasswordDto = new ResetPasswordDto("");

        _controller.ControllerContext = _testFixture.CreateControllerContextWithClaims(
            new List<Claim> { new(ClaimTypes.Role, "Admin") }
        );

        // Act
        var result = await _controller.ResetUserPassword(userId, resetPasswordDto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}