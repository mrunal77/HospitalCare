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
public class AppointmentsControllerTests
{
    private Mock<IAppointmentService> _appointmentServiceMock = null!;
    private AppointmentsController _controller = null!;
    private TestFixture _testFixture = null!;

    [SetUp]
    public void Setup()
    {
        _appointmentServiceMock = new Mock<IAppointmentService>();
        _controller = new AppointmentsController(_appointmentServiceMock.Object);
        _testFixture = new TestFixture();
    }

    #region Get All Tests

    [Test]
    public async Task GetAll_ReturnsAllAppointments()
    {
        // Arrange
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "John Doe", "Dr. Smith", DateTime.UtcNow.AddDays(1), 30, "Checkup", null, "Scheduled"),
            new AppointmentDto(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Jane Doe", "Dr. Johnson", DateTime.UtcNow.AddDays(2), 45, "Consultation", null, "Scheduled")
        };

        _appointmentServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(appointments);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedAppointments = okResult?.Value as IEnumerable<AppointmentDto>;
        returnedAppointments.Should().HaveCount(2);
    }

    #endregion

    #region Get By Id Tests

    [Test]
    public async Task GetById_ExistingAppointment_ReturnsAppointment()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var appointment = new AppointmentDto(appointmentId, Guid.NewGuid(), Guid.NewGuid(), "John Doe", "Dr. Smith", DateTime.UtcNow.AddDays(1), 30, "Checkup", null, "Scheduled");

        _appointmentServiceMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync(appointment);

        // Act
        var result = await _controller.GetById(appointmentId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(appointment);
    }

    [Test]
    public async Task GetById_NonExistingAppointment_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentServiceMock
            .Setup(x => x.GetByIdAsync(appointmentId))
            .ReturnsAsync((AppointmentDto?)null);

        // Act
        var result = await _controller.GetById(appointmentId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Get By Patient Id Tests

    [Test]
    public async Task GetByPatientId_ExistingPatient_ReturnsAppointments()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto(Guid.NewGuid(), patientId, Guid.NewGuid(), "John Doe", "Dr. Smith", DateTime.UtcNow.AddDays(1), 30, "Checkup", null, "Scheduled")
        };

        _appointmentServiceMock
            .Setup(x => x.GetByPatientIdAsync(patientId))
            .ReturnsAsync(appointments);

        // Act
        var result = await _controller.GetByPatientId(patientId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Get By Doctor Id Tests

    [Test]
    public async Task GetByDoctorId_ExistingDoctor_ReturnsAppointments()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var appointments = new List<AppointmentDto>
        {
            new AppointmentDto(Guid.NewGuid(), Guid.NewGuid(), doctorId, "John Doe", "Dr. Smith", DateTime.UtcNow.AddDays(1), 30, "Checkup", null, "Scheduled")
        };

        _appointmentServiceMock
            .Setup(x => x.GetByDoctorIdAsync(doctorId))
            .ReturnsAsync(appointments);

        // Act
        var result = await _controller.GetByDoctorId(doctorId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_ValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateAppointmentDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(1),
            30,
            "Checkup",
            null
        );

        var appointment = new AppointmentDto(
            Guid.NewGuid(),
            createDto.PatientId,
            createDto.DoctorId,
            "John Doe",
            "Dr. Smith",
            createDto.AppointmentDate,
            createDto.DurationMinutes,
            createDto.Reason,
            createDto.Notes,
            "Scheduled"
        );

        _appointmentServiceMock
            .Setup(x => x.CreateAsync(createDto))
            .ReturnsAsync(appointment);

        // Act
        var result = await _controller.Create(createDto);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    #endregion

    #region Cancel Tests

    [Test]
    public async Task Cancel_ExistingAppointment_ReturnsNoContent()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentServiceMock
            .Setup(x => x.CancelAsync(appointmentId, It.IsAny<string?>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Cancel(appointmentId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Cancel_NonExistingAppointment_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentServiceMock
            .Setup(x => x.CancelAsync(appointmentId, It.IsAny<string?>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Cancel(appointmentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Complete Tests

    [Test]
    public async Task Complete_ExistingAppointment_ReturnsNoContent()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentServiceMock
            .Setup(x => x.CompleteAsync(appointmentId, It.IsAny<string?>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Complete(appointmentId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Complete_NonExistingAppointment_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();

        _appointmentServiceMock
            .Setup(x => x.CompleteAsync(appointmentId, It.IsAny<string?>()))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Complete(appointmentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Reschedule Tests

    [Test]
    public async Task Reschedule_ExistingAppointment_ReturnsOk()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var rescheduleDto = new RescheduleAppointmentDto(DateTime.UtcNow.AddDays(2), 45);

        var appointment = new AppointmentDto(
            appointmentId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "John Doe",
            "Dr. Smith",
            rescheduleDto.NewDate,
            rescheduleDto.NewDurationMinutes,
            "Checkup",
            null,
            "Scheduled"
        );

        _appointmentServiceMock
            .Setup(x => x.RescheduleAsync(appointmentId, rescheduleDto))
            .ReturnsAsync(appointment);

        // Act
        var result = await _controller.Reschedule(appointmentId, rescheduleDto);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task Reschedule_NonExistingAppointment_ReturnsNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var rescheduleDto = new RescheduleAppointmentDto(DateTime.UtcNow.AddDays(2), 45);

        _appointmentServiceMock
            .Setup(x => x.RescheduleAsync(appointmentId, rescheduleDto))
            .ReturnsAsync((AppointmentDto?)null);

        // Act
        var result = await _controller.Reschedule(appointmentId, rescheduleDto);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}