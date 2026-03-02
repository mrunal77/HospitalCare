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
public class PatientsControllerTests
{
    private Mock<IPatientService> _patientServiceMock = null!;
    private PatientsController _controller = null!;
    private TestFixture _testFixture = null!;

    [SetUp]
    public void Setup()
    {
        _patientServiceMock = new Mock<IPatientService>();
        _controller = new PatientsController(_patientServiceMock.Object);
        _testFixture = new TestFixture();
    }

    #region Get All Tests

    [Test]
    public async Task GetAll_ReturnsAllPatients()
    {
        // Arrange
        var patients = new List<PatientDto>
        {
            new PatientDto(Guid.NewGuid(), "John", "Doe", DateTime.UtcNow.AddYears(-30), "john@example.com", "1234567890", "Address 1"),
            new PatientDto(Guid.NewGuid(), "Jane", "Smith", DateTime.UtcNow.AddYears(-25), "jane@example.com", "0987654321", "Address 2")
        };

        _patientServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(patients);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedPatients = okResult?.Value as IEnumerable<PatientDto>;
        returnedPatients.Should().HaveCount(2);
    }

    #endregion

    #region Get By Id Tests

    [Test]
    public async Task GetById_ExistingPatient_ReturnsPatient()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var patient = new PatientDto(patientId, "John", "Doe", DateTime.UtcNow.AddYears(-30), "john@example.com", "1234567890", "Address");

        _patientServiceMock
            .Setup(x => x.GetByIdAsync(patientId))
            .ReturnsAsync(patient);

        // Act
        var result = await _controller.GetById(patientId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(patient);
    }

    [Test]
    public async Task GetById_NonExistingPatient_ReturnsNotFound()
    {
        // Arrange
        var patientId = Guid.NewGuid();

        _patientServiceMock
            .Setup(x => x.GetByIdAsync(patientId))
            .ReturnsAsync((PatientDto?)null);

        // Act
        var result = await _controller.GetById(patientId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_ValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreatePatientDto(
            "John",
            "Doe",
            DateTime.UtcNow.AddYears(-30),
            "john@example.com",
            "1234567890",
            "Address"
        );

        var patient = new PatientDto(
            Guid.NewGuid(),
            "John",
            "Doe",
            DateTime.UtcNow.AddYears(-30),
            "john@example.com",
            "1234567890",
            "Address"
        );

        _patientServiceMock
            .Setup(x => x.CreateAsync(createDto))
            .ReturnsAsync(patient);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task Update_ValidData_ReturnsOk()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var updateDto = new UpdatePatientDto("newemail@example.com", "9999999999", "New Address");

        var patient = new PatientDto(
            patientId,
            "John",
            "Doe",
            DateTime.UtcNow.AddYears(-30),
            "newemail@example.com",
            "9999999999",
            "New Address"
        );

        _patientServiceMock
            .Setup(x => x.UpdateAsync(patientId, updateDto))
            .ReturnsAsync(patient);

        // Act
        var result = await _controller.Update(patientId, updateDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task Update_NonExistingPatient_ReturnsNotFound()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var updateDto = new UpdatePatientDto("newemail@example.com", "9999999999", "New Address");

        _patientServiceMock
            .Setup(x => x.UpdateAsync(patientId, updateDto))
            .ReturnsAsync((PatientDto?)null);

        // Act
        var result = await _controller.Update(patientId, updateDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_ExistingPatient_ReturnsNoContent()
    {
        // Arrange
        var patientId = Guid.NewGuid();

        _patientServiceMock
            .Setup(x => x.DeleteAsync(patientId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(patientId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Delete_NonExistingPatient_ReturnsNotFound()
    {
        // Arrange
        var patientId = Guid.NewGuid();

        _patientServiceMock
            .Setup(x => x.DeleteAsync(patientId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(patientId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}
