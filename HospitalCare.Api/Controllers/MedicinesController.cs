using HospitalCare.Application.DTOs;
using HospitalCare.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalCare.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicineDto>>> GetAll()
    {
        var medicines = await _medicineService.GetAllAsync();
        return Ok(medicines);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<SearchMedicineDto>>> Search([FromQuery] string q, [FromQuery] int limit = 20)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return Ok(Enumerable.Empty<SearchMedicineDto>());
        }
        var medicines = await _medicineService.SearchAsync(q, limit);
        return Ok(medicines);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategories()
    {
        var categories = await _medicineService.GetAllSubCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("category/{subCategory}")]
    public async Task<ActionResult<IEnumerable<MedicineDto>>> GetByCategory(string subCategory)
    {
        var medicines = await _medicineService.GetBySubCategoryAsync(subCategory);
        return Ok(medicines);
    }
}
