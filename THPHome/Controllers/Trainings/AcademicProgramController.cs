﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using THPHome.Data;
using THPHome.Foundations;
using THPHome.Interfaces.IService.ITrainings;

namespace THPHome.Controllers.Trainings;

[Route("api/academic-program")]
public class AcademicProgramController(ApplicationDbContext context, IAcademicProgramService _academicProgramService) : BaseController(context)
{
    [HttpGet("all-high-quality-program"), AllowAnonymous]
    public async Task<IActionResult> GetAllHighQualityProgramAsync() => Ok(new { data = await _academicProgramService.GetAllHighQualityProgramAsync() });
}
