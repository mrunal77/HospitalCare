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
public class RolesControllerTests
{
    private Mock<IRoleService> _roleServiceMock = null!;
    private RolesController _controller = null!;
    private TestFixture _testFixture = null!;

    [SetUp]
    public void Setup()
    {
        _roleServiceMock = new Mock<IRoleService>();
        _controller = new RolesController(_roleServiceMock.Object);
        _testFixture = new TestFixture();
    }

    #region Get All Tests

    [Test]
    public async Task GetAll_ReturnsAllRoles()
    {
        // Arrange
        var roles = new List<RoleDto>
        {
            new RoleDto(Guid.NewGuid(), "Admin", "Administrator role", "FullAccess", true, DateTime.UtcNow),
            new RoleDto(Guid.NewGuid(), "Patient", "Patient role", "PatientAccess", true, DateTime.UtcNow)
        };

        _roleServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(roles);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedRoles = okResult?.Value as IEnumerable<RoleDto>;
        returnedRoles.Should().HaveCount(2);
    }

    #endregion

    #region Get Active Roles Tests

    [Test]
    public async Task GetActiveRoles_ReturnsActiveRoles()
    {
        // Arrange
        var roles = new List<RoleDto>
        {
            new RoleDto(Guid.NewGuid(), "Admin", "Administrator role", "FullAccess", true, DateTime.UtcNow),
            new RoleDto(Guid.NewGuid(), "Patient", "Patient role", "PatientAccess", true, DateTime.UtcNow)
        };

        _roleServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(roles);

        // Act
        var result = await _controller.GetActiveRoles();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Get By Id Tests

    [Test]
    public async Task GetById_ExistingRole_ReturnsRole()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new RoleDto(roleId, "Admin", "Administrator role", "FullAccess", true, DateTime.UtcNow);

        _roleServiceMock
            .Setup(x => x.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        // Act
        var result = await _controller.GetById(roleId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(role);
    }

    [Test]
    public async Task GetById_NonExistingRole_ReturnsNotFound()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleServiceMock
            .Setup(x => x.GetByIdAsync(roleId))
            .ReturnsAsync((RoleDto?)null);

        // Act
        var result = await _controller.GetById(roleId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_ValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateRoleDto("Doctor", "Doctor role", "DoctorAccess");

        var role = new RoleDto(
            Guid.NewGuid(),
            "Doctor",
            "Doctor role",
            "DoctorAccess",
            true,
            DateTime.UtcNow
        );

        _roleServiceMock
            .Setup(x => x.CreateAsync(createDto))
            .ReturnsAsync(role);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public async Task Create_DuplicateRole_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateRoleDto("Admin", "Administrator role", "FullAccess");

        _roleServiceMock
            .Setup(x => x.CreateAsync(createDto))
            .Throws(new InvalidOperationException("Role already exists"));

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task Update_ValidData_ReturnsOk()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var updateDto = new UpdateRoleDto("Admin Updated", "Updated description", "FullAccess");

        var role = new RoleDto(
            roleId,
            "Admin Updated",
            "Updated description",
            "FullAccess",
            true,
            DateTime.UtcNow
        );

        _roleServiceMock
            .Setup(x => x.UpdateAsync(roleId, updateDto))
            .ReturnsAsync(role);

        // Act
        var result = await _controller.Update(roleId, updateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task Update_NonExistingRole_ReturnsNotFound()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var updateDto = new UpdateRoleDto("Admin Updated", "Updated description", "FullAccess");

        _roleServiceMock
            .Setup(x => x.UpdateAsync(roleId, updateDto))
            .Throws(new InvalidOperationException("Role not found"));

        // Act
        var result = await _controller.Update(roleId, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_ExistingRole_ReturnsNoContent()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleServiceMock
            .Setup(x => x.DeleteAsync(roleId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(roleId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Delete_NonExistingRole_ReturnsNotFound()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleServiceMock
            .Setup(x => x.DeleteAsync(roleId))
            .Throws(new InvalidOperationException("Role not found"));

        // Act
        var result = await _controller.Delete(roleId);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Activate Tests

    [Test]
    public async Task Activate_ExistingRole_ReturnsOk()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new RoleDto(roleId, "Admin", "Administrator role", "FullAccess", true, DateTime.UtcNow);

        _roleServiceMock
            .Setup(x => x.ActivateAsync(roleId))
            .ReturnsAsync(role);

        // Act
        var result = await _controller.Activate(roleId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task Activate_NonExistingRole_ReturnsNotFound()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleServiceMock
            .Setup(x => x.ActivateAsync(roleId))
            .Throws(new InvalidOperationException("Role not found"));

        // Act
        var result = await _controller.Activate(roleId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion

    #region Deactivate Tests

    [Test]
    public async Task Deactivate_ExistingRole_ReturnsOk()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var role = new RoleDto(roleId, "Admin", "Administrator role", "FullAccess", false, DateTime.UtcNow);

        _roleServiceMock
            .Setup(x => x.DeactivateAsync(roleId))
            .ReturnsAsync(role);

        // Act
        var result = await _controller.Deactivate(roleId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task Deactivate_NonExistingRole_ReturnsNotFound()
    {
        // Arrange
        var roleId = Guid.NewGuid();

        _roleServiceMock
            .Setup(x => x.DeactivateAsync(roleId))
            .Throws(new InvalidOperationException("Role not found"));

        // Act
        var result = await _controller.Deactivate(roleId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    #endregion
}