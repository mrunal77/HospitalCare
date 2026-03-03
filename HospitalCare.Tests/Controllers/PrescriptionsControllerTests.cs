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
public class PrescriptionsControllerTests
{
    private Mock<IPrescriptionService> _prescriptionServiceMock = null!;
    private PrescriptionsController _controller = null!;
    private TestFixture _testFixture = null!;

    [SetUp]
    public void Setup()
    {
        _prescriptionServiceMock = new Mock<IPrescriptionService>();
        _controller = new PrescriptionsController(_prescriptionServiceMock.Object);
        _testFixture = new TestFixture();
    }

    #region Get All Tests

    [Test]
    public async Task GetAll_ReturnsAllPrescriptions()
    {
        var prescriptions = new List<PrescriptionDto>
        {
            new PrescriptionDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Dr. Smith",
                "John Doe",
                "Flu",
                new List<MedicineDetailDto>
                {
                    new MedicineDetailDto(Guid.NewGuid(), "Paracetamol", "500mg", "10 tablets", "Take after food", null, 7)
                },
                "Take rest",
                DateTime.UtcNow
            ),
            new PrescriptionDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Dr. Johnson",
                "Jane Doe",
                "Cold",
                new List<MedicineDetailDto>(),
                null,
                DateTime.UtcNow
            )
        };

        _prescriptionServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(prescriptions);

        var result = await _controller.GetAll();

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedPrescriptions = okResult?.Value as IEnumerable<PrescriptionDto>;
        returnedPrescriptions.Should().HaveCount(2);
    }

    #endregion

    #region Get By Id Tests

    [Test]
    public async Task GetById_ExistingPrescription_ReturnsPrescription()
    {
        var prescriptionId = Guid.NewGuid();
        var prescription = new PrescriptionDto(
            prescriptionId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Dr. Smith",
            "John Doe",
            "Flu",
            new List<MedicineDetailDto>
            {
                new MedicineDetailDto(Guid.NewGuid(), "Paracetamol", "500mg", "10 tablets", "Take after food", null, 7)
            },
            "Take rest",
            DateTime.UtcNow
        );

        _prescriptionServiceMock
            .Setup(x => x.GetByIdAsync(prescriptionId))
            .ReturnsAsync(prescription);

        var result = await _controller.GetById(prescriptionId);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(prescription);
    }

    [Test]
    public async Task GetById_NonExistingPrescription_ReturnsNotFound()
    {
        var prescriptionId = Guid.NewGuid();

        _prescriptionServiceMock
            .Setup(x => x.GetByIdAsync(prescriptionId))
            .ReturnsAsync((PrescriptionDto?)null);

        var result = await _controller.GetById(prescriptionId);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Get By Patient Id Tests

    [Test]
    public async Task GetByPatientId_ExistingPatient_ReturnsPrescriptions()
    {
        var patientId = Guid.NewGuid();
        var prescriptions = new List<PrescriptionDto>
        {
            new PrescriptionDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                patientId,
                "Dr. Smith",
                "John Doe",
                "Flu",
                new List<MedicineDetailDto>(),
                null,
                DateTime.UtcNow
            )
        };

        _prescriptionServiceMock
            .Setup(x => x.GetByPatientIdAsync(patientId))
            .ReturnsAsync(prescriptions);

        var result = await _controller.GetByPatientId(patientId);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Get By Doctor Id Tests

    [Test]
    public async Task GetByDoctorId_ExistingDoctor_ReturnsPrescriptions()
    {
        var doctorId = Guid.NewGuid();
        var prescriptions = new List<PrescriptionDto>
        {
            new PrescriptionDto(
                Guid.NewGuid(),
                Guid.NewGuid(),
                doctorId,
                Guid.NewGuid(),
                "Dr. Smith",
                "John Doe",
                "Flu",
                new List<MedicineDetailDto>(),
                null,
                DateTime.UtcNow
            )
        };

        _prescriptionServiceMock
            .Setup(x => x.GetByDoctorIdAsync(doctorId))
            .ReturnsAsync(prescriptions);

        var result = await _controller.GetByDoctorId(doctorId);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Get By Appointment Id Tests

    [Test]
    public async Task GetByAppointmentId_ExistingAppointment_ReturnsPrescriptions()
    {
        var appointmentId = Guid.NewGuid();
        var prescriptions = new List<PrescriptionDto>
        {
            new PrescriptionDto(
                Guid.NewGuid(),
                appointmentId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Dr. Smith",
                "John Doe",
                "Flu",
                new List<MedicineDetailDto>(),
                null,
                DateTime.UtcNow
            )
        };

        _prescriptionServiceMock
            .Setup(x => x.GetByAppointmentIdAsync(appointmentId))
            .ReturnsAsync(prescriptions);

        var result = await _controller.GetByAppointmentId(appointmentId);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Create Tests

    [Test]
    public async Task Create_ValidData_ReturnsCreated()
    {
        var createDto = new CreatePrescriptionDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Viral Fever",
            new List<CreateMedicineDetailDto>
            {
                new CreateMedicineDetailDto("Paracetamol", "500mg", "10 tablets", "1 tablet thrice daily", "Take after food", 7),
                new CreateMedicineDetailDto("Cetirizine", "10mg", "10 tablets", "1 tablet at night", null, 5)
            },
            "Drink plenty of water"
        );

        var prescription = new PrescriptionDto(
            Guid.NewGuid(),
            createDto.AppointmentId,
            createDto.DoctorId,
            createDto.PatientId,
            "Dr. Smith",
            "John Doe",
            createDto.Diagnosis,
            createDto.Medicines.Select(m => new MedicineDetailDto(
                Guid.NewGuid(),
                m.Name,
                m.Dosage,
                m.Amount,
                m.Routine,
                m.Instructions,
                m.DurationDays
            )).ToList(),
            createDto.Notes,
            DateTime.UtcNow
        );

        _prescriptionServiceMock
            .Setup(x => x.CreateAsync(createDto))
            .ReturnsAsync(prescription);

        var result = await _controller.Create(createDto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test]
    public async Task Create_WithNoMedicines_ReturnsCreated()
    {
        var createDto = new CreatePrescriptionDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "General Checkup",
            new List<CreateMedicineDetailDto>(),
            "No medications prescribed"
        );

        var prescription = new PrescriptionDto(
            Guid.NewGuid(),
            createDto.AppointmentId,
            createDto.DoctorId,
            createDto.PatientId,
            "Dr. Smith",
            "John Doe",
            createDto.Diagnosis,
            new List<MedicineDetailDto>(),
            createDto.Notes,
            DateTime.UtcNow
        );

        _prescriptionServiceMock
            .Setup(x => x.CreateAsync(createDto))
            .ReturnsAsync(prescription);

        var result = await _controller.Create(createDto);

        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task Update_ExistingPrescription_ReturnsOk()
    {
        var prescriptionId = Guid.NewGuid();
        var updateDto = new UpdatePrescriptionDto("Updated Diagnosis", "Updated notes");

        var prescription = new PrescriptionDto(
            prescriptionId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Dr. Smith",
            "John Doe",
            "Updated Diagnosis",
            new List<MedicineDetailDto>(),
            "Updated notes",
            DateTime.UtcNow
        );

        _prescriptionServiceMock
            .Setup(x => x.UpdateAsync(prescriptionId, updateDto))
            .ReturnsAsync(prescription);

        var result = await _controller.Update(prescriptionId, updateDto);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task Update_NonExistingPrescription_ReturnsNotFound()
    {
        var prescriptionId = Guid.NewGuid();
        var updateDto = new UpdatePrescriptionDto("Updated Diagnosis", null);

        _prescriptionServiceMock
            .Setup(x => x.UpdateAsync(prescriptionId, updateDto))
            .ReturnsAsync((PrescriptionDto?)null);

        var result = await _controller.Update(prescriptionId, updateDto);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task Delete_ExistingPrescription_ReturnsNoContent()
    {
        var prescriptionId = Guid.NewGuid();

        _prescriptionServiceMock
            .Setup(x => x.DeleteAsync(prescriptionId))
            .ReturnsAsync(true);

        var result = await _controller.Delete(prescriptionId);

        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task Delete_NonExistingPrescription_ReturnsNotFound()
    {
        var prescriptionId = Guid.NewGuid();

        _prescriptionServiceMock
            .Setup(x => x.DeleteAsync(prescriptionId))
            .ReturnsAsync(false);

        var result = await _controller.Delete(prescriptionId);

        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Add Medicine Tests

    [Test]
    public async Task AddMedicine_ExistingPrescription_ReturnsOk()
    {
        var prescriptionId = Guid.NewGuid();
        var addMedicineDto = new AddMedicineDto(
            "Aspirin",
            "100mg",
            "30 tablets",
            "1 tablet daily",
            "Take after breakfast",
            30
        );

        var medicineDetailDto = new MedicineDetailDto(
            Guid.NewGuid(),
            addMedicineDto.Name,
            addMedicineDto.Dosage,
            addMedicineDto.Amount,
            addMedicineDto.Routine,
            addMedicineDto.Instructions,
            addMedicineDto.DurationDays
        );

        var prescription = new PrescriptionDto(
            prescriptionId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Dr. Smith",
            "John Doe",
            "Flu",
            new List<MedicineDetailDto> { medicineDetailDto },
            null,
            DateTime.UtcNow
        );

        _prescriptionServiceMock
            .Setup(x => x.AddMedicineAsync(prescriptionId, addMedicineDto))
            .ReturnsAsync(prescription);

        var result = await _controller.AddMedicine(prescriptionId, addMedicineDto);

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    public async Task AddMedicine_NonExistingPrescription_ReturnsNotFound()
    {
        var prescriptionId = Guid.NewGuid();
        var addMedicineDto = new AddMedicineDto(
            "Aspirin",
            "100mg",
            "30 tablets",
            "1 tablet daily",
            null,
            30
        );

        _prescriptionServiceMock
            .Setup(x => x.AddMedicineAsync(prescriptionId, addMedicineDto))
            .ReturnsAsync((PrescriptionDto?)null);

        var result = await _controller.AddMedicine(prescriptionId, addMedicineDto);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    #endregion

    #region Remove Medicine Tests

    [Test]
    public async Task RemoveMedicine_ExistingPrescription_ReturnsNoContent()
    {
        var prescriptionId = Guid.NewGuid();
        var medicineId = Guid.NewGuid();

        _prescriptionServiceMock
            .Setup(x => x.RemoveMedicineAsync(prescriptionId, medicineId))
            .ReturnsAsync(true);

        var result = await _controller.RemoveMedicine(prescriptionId, medicineId);

        result.Should().BeOfType<NoContentResult>();
    }

    [Test]
    public async Task RemoveMedicine_NonExistingPrescription_ReturnsNotFound()
    {
        var prescriptionId = Guid.NewGuid();
        var medicineId = Guid.NewGuid();

        _prescriptionServiceMock
            .Setup(x => x.RemoveMedicineAsync(prescriptionId, medicineId))
            .ReturnsAsync(false);

        var result = await _controller.RemoveMedicine(prescriptionId, medicineId);

        result.Should().BeOfType<NotFoundResult>();
    }

    #endregion
}
