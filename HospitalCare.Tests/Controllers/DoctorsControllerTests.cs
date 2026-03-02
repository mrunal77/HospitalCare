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
public class DoctorsControllerTests
{
    private Mock<IDoctorService> _doctorServiceMock = null!;
    private DoctorsController _controller = null!;
    private TestFixture _testFixture = null!;

    [SetUp]
    public void Setup()
    {
        _doctorServiceMock = new Mock<IDoctorService>();
        _controller = new DoctorsController(_doctorServiceMock.Object);
        _testFixture = new TestFixture();
    }

    #region Get All Tests

    [Test]
    public async Task GetAll_ReturnsAllDoctors()
    {
        // Arrange
        var doctors = new List<DoctorDto>
        {
            new DoctorDto(Guid.NewGuid(), "John", "Doe", "Cardiology", "john@example.com", "1234567890", "LIC001"),
            new DoctorDto(Guid.NewGuid(), "Jane", "Smith", "Neurology", "jane@example.com", "0987654321", "LIC002")
        };

        _doctorServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(doctors);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedDoctors = okResult?.Value as IEnumerable<DoctorDto>;
        returnedDoctors.Should().HaveCount(2);
    }

    #endregion

    #region Get By Id Tests

    [Test]
    public async Task GetById_ExistingDoctor_ReturnsDoctor()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var doctor = new DoctorDto(doctorId, "John", "Doe", "Cardiology", "john@example.com", "1234567890", "LIC001");

        _doctorServiceMock
            .Setup(x => x.GetByIdAsync(doctorId))
            .ReturnsAsync(doctor);

        // Act
        var result = await _controller.GetById(doctorId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(doctor);
    }

    [Test]
    public async Task GetById_NonExistingDoctor_ReturnsNotFound()
    {
        // Arrange
        var doctorId = Guid.NewGuid();

        _doctorServiceMock
            .Setup(x => x.GetByIdAsync(doctorId))
            .ReturnsAsync((DoctorDto?)null);

        // Act
        var result = await _controller.GetById(doctorId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Get By Specialization Tests

    [Test]
    public async Task GetBySpecialization_ExistingSpecialization_ReturnsDoctors()
    {
        // Arrange
        var specialization = "Cardiology";
        var doctors = new List<DoctorDto>
        {
            new DoctorDto(Guid.NewGuid(), "John", "Doe", "Cardiology", "john@example.com", "1234567890", "LIC001"),
            new DoctorDto(Guid.NewGuid(), "Jane", "Smith", "Cardiology", "jane@example.com", "0987654321", "LIC002")
        };

        _doctorServiceMock
            .Setup(x => x.GetBySpecializationAsync(specialization))
            .ReturnsAsync(doctors);

        // Act
        var result = await _controller.GetBySpecialization(specialization);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedDoctors = okResult?.Value as IEnumerable<DoctorDto>;
        returnedDoctors.Should().HaveCount(2);
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_ValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateDoctorDto(
            "John",
            "Doe",
            "Cardiology",
            "john@example.com",
            "1234567890",
            "LIC001"
        );

        var doctor = new DoctorDto(
            Guid.NewGuid(),
            "John",
            "Doe",
            "Cardiology",
            "john@example.com",
            "1234567890",
            "LIC001"
        );

        _doctorServiceMock
            .Setup(x => x.CreateAsync(createDto))
            .ReturnsAsync(doctor);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_ExistingDoctor_ReturnsNoContent()
    {
        // Arrange
        var doctorId = Guid.NewGuid();

        _doctorServiceMock
            .Setup(x => x.DeleteAsync(doctorId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(doctorId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Delete_NonExistingDoctor_ReturnsNotFound()
    {
        // Arrange
        var doctorId = Guid.NewGuid();

        _doctorServiceMock
            .Setup(x => x.DeleteAsync(doctorId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(doctorId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}