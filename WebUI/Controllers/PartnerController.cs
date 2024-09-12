﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces.IService;
using ApplicationCore.Models.Filters;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Entities;
using WebUI.Extensions;
using WebUI.Foundations;
using WebUI.Models.Filters.Parners;

namespace WebUI.Api;

public class PartnerController : BaseController
{
    private readonly IPartnerService _partnerService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public PartnerController(IPartnerService partnerService, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context) : base(context)
    {
        _partnerService = partnerService;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync([FromRoute] int id) => Ok(await _context.Partners.FindAsync(id));

    [HttpGet("list")]
    public async Task<IActionResult> GetListAsync([FromQuery] PartnerFilterOptions filterOptions) => Ok(await _partnerService.GetListAsync(filterOptions));

    [HttpPost("add")]
    public async Task<IActionResult> AddAsync([FromBody] Partner partner)
    {
        var user = await _userManager.FindByIdAsync(User.GetId());
        if (user != null)
        {
            partner.CreatedBy = user.Id;
        }
        return Ok(await _partnerService.AddAsync(partner));
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] Partner args)
    {
        var partner = await _context.Partners.FindAsync(args.Id);
        if (partner is null) return BadRequest("Data not found!");
        partner.Status = args.Status;
        partner.Name = args.Name;
        partner.Url = args.Url;
        partner.Logo = args.Logo;
        partner.Description = args.Description;
        var user = await _userManager.FindByIdAsync(User.GetId());
        if (user != null)
        {
            partner.ModifiedBy = user.Id;
        }
        _context.Update(partner);
        await _context.SaveChangesAsync();
        return Ok(IdentityResult.Success);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id) => Ok(await _partnerService.DeleteAsync(id));

    [HttpPost("upload")]
    public async Task<IActionResult> UploadAsync([FromForm] IFormFile file)
    {
        if (file?.Length > 0)
        {
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "files");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { succeeded = true, fileUrl = $"/files/{fileName}" });
        }
        return Ok(new { succeeded = false, fileUrl = "" });
    }
}
