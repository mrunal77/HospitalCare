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
public class MedicinesControllerTests
{
    private Mock<IMedicineService> _medicineServiceMock = null!;
    private MedicinesController _controller = null!;
    private TestFixture _testFixture = null!;

    [SetUp]
    public void Setup()
    {
        _medicineServiceMock = new Mock<IMedicineService>();
        _controller = new MedicinesController(_medicineServiceMock.Object);
        _testFixture = new TestFixture();
    }

    #region GetAll Tests

    [Test]
    public async Task GetAll_ReturnsOkWithMedicines()
    {
        var medicines = new List<MedicineDto>
        {
            new MedicineDto(
                Guid.NewGuid(),
                "Pain Relief",
                "Paracetamol",
                "Paracetamol 500mg",
                50.00m,
                "PharmaCorp",
                "Pain reliever",
                null,
                null,
                true
            ),
            new MedicineDto(
                Guid.NewGuid(),
                "Antibiotic",
                "Amoxicillin",
                "Amoxicillin 250mg",
                120.00m,
                "MedLife",
                "Antibiotic",
                null,
                null,
                true
            )
        };

        _medicineServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(medicines);

        var result = await _controller.GetAll();

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedMedicines = okResult?.Value as IEnumerable<MedicineDto>;
        returnedMedicines.Should().HaveCount(2);
    }

    [Test]
    public async Task GetAll_EmptyList_ReturnsOkWithEmptyCollection()
    {
        _medicineServiceMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(Enumerable.Empty<MedicineDto>());

        var result = await _controller.GetAll();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region Search Tests

    [Test]
    public async Task Search_ValidQuery_ReturnsOkWithResults()
    {
        var searchResults = new List<SearchMedicineDto>
        {
            new SearchMedicineDto(Guid.NewGuid(), "Paracetamol 500mg", "Paracetamol", "PharmaCorp", "Pain Relief")
        };

        _medicineServiceMock
            .Setup(x => x.SearchAsync("paracetamol", 20))
            .ReturnsAsync(searchResults);

        var result = await _controller.Search("paracetamol");

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var results = okResult?.Value as IEnumerable<SearchMedicineDto>;
        results.Should().HaveCount(1);
    }

    [Test]
    public async Task Search_EmptyQuery_ReturnsOkWithEmptyCollection()
    {
        var result = await _controller.Search("");

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var results = okResult?.Value as IEnumerable<SearchMedicineDto>;
        results.Should().BeEmpty();
    }

    [Test]
    public async Task Search_NullQuery_ReturnsOkWithEmptyCollection()
    {
        var result = await _controller.Search(null!);

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var results = okResult?.Value as IEnumerable<SearchMedicineDto>;
        results.Should().BeEmpty();
    }

    [Test]
    public async Task Search_WithCustomLimit_ReturnsOkWithResults()
    {
        var searchResults = new List<SearchMedicineDto>();

        _medicineServiceMock
            .Setup(x => x.SearchAsync("aspirin", 10))
            .ReturnsAsync(searchResults);

        var result = await _controller.Search("aspirin", 10);

        result.Result.Should().BeOfType<OkObjectResult>();
        _medicineServiceMock.Verify(x => x.SearchAsync("aspirin", 10), Times.Once);
    }

    #endregion

    #region GetCategories Tests

    [Test]
    public async Task GetCategories_ReturnsOkWithCategories()
    {
        var categories = new List<string> { "Pain Relief", "Antibiotic", "Vitamin" };

        _medicineServiceMock
            .Setup(x => x.GetAllSubCategoriesAsync())
            .ReturnsAsync(categories);

        var result = await _controller.GetCategories();

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedCategories = okResult?.Value as IEnumerable<string>;
        returnedCategories.Should().HaveCount(3);
    }

    [Test]
    public async Task GetCategories_EmptyList_ReturnsOkWithEmptyCollection()
    {
        _medicineServiceMock
            .Setup(x => x.GetAllSubCategoriesAsync())
            .ReturnsAsync(Enumerable.Empty<string>());

        var result = await _controller.GetCategories();

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion

    #region GetByCategory Tests

    [Test]
    public async Task GetByCategory_ValidCategory_ReturnsOkWithMedicines()
    {
        var medicines = new List<MedicineDto>
        {
            new MedicineDto(
                Guid.NewGuid(),
                "Pain Relief",
                "Ibuprofen",
                "Ibuprofen 400mg",
                75.00m,
                "PharmaCorp",
                "Anti-inflammatory",
                null,
                null,
                true
            )
        };

        _medicineServiceMock
            .Setup(x => x.GetBySubCategoryAsync("Pain Relief"))
            .ReturnsAsync(medicines);

        var result = await _controller.GetByCategory("Pain Relief");

        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var returnedMedicines = okResult?.Value as IEnumerable<MedicineDto>;
        returnedMedicines.Should().HaveCount(1);
    }

    [Test]
    public async Task GetByCategory_NoMedicinesInCategory_ReturnsOkWithEmptyCollection()
    {
        _medicineServiceMock
            .Setup(x => x.GetBySubCategoryAsync("NonExistent"))
            .ReturnsAsync(Enumerable.Empty<MedicineDto>());

        var result = await _controller.GetByCategory("NonExistent");

        result.Result.Should().BeOfType<OkObjectResult>();
    }

    #endregion
}
