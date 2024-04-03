﻿using ApplicationCore.Entities;
using ApplicationCore.Models.Files;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebUI.Api;
using WebUI.Entities;
using WebUI.Extensions;
using WebUI.Interfaces.IService;
using WebUI.Models.Filters.Files;
using WebUI.Models.ViewModel;

namespace WebUI.Controllers;

public class FileController : BaseController
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ITelegramService _telegramService;
    private readonly UserManager<ApplicationUser> _userManager;

    public FileController(IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, ITelegramService telegramService, UserManager<ApplicationUser> userManager) : base(context)
    {
        _webHostEnvironment = webHostEnvironment;
        _telegramService = telegramService;
        _userManager = userManager;
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] FileFilterOptions filterOptions)
    {
        var query = from a in _context.ApplicationFiles
                    join b in _context.ApplicationFolders on a.FolderId equals b.Id
                    into ab
                    from b in ab.DefaultIfEmpty()
                    join c in _context.Users on a.CreatedBy equals c.Id
                    join d in _context.Users on a.ModifiedBy equals d.Id
                    into ad
                    from d in ad.DefaultIfEmpty()
                    select new
                    {
                        a.Id,
                        a.Url,
                        fileName = a.Name,
                        a.FolderId,
                        createdBy = c.UserName,
                        folderName = b.Name,
                        modifiedBy = d.UserName,
                        a.CreatedDate,
                        a.ModifiedDate,
                        a.Size,
                        a.ContentType
                    };
        query = query.OrderByDescending(x => x.ModifiedDate);
        return Ok(new
        {
            data = await query.Skip((filterOptions.PageIndex - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadAsync([FromForm] IFormFile file)
    {
        try
        {
            if (file?.Length > 0)
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "files");

                var filePath = Path.Combine(uploadPath, file.FileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                var host = Request.Host.Value;
                var url = $"https://{host}/files/{file.FileName}";

                var user = await _userManager.FindByIdAsync(User.GetId());

                await _context.ApplicationFiles.AddAsync(new ApplicationFile
                {
                    ContentType = file.ContentType,
                    Name = file.FileName,
                    Size = file.Length,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Url = url,
                    CreatedBy = user?.Id
                });
                await _context.SaveChangesAsync();

                return Ok(new { succeeded = true, url });
            }
            return Ok(new { succeeded = false, message = "File not found", url = string.Empty });
        }
        catch (Exception ex)
        {
            await _telegramService.SendMessageAsync(ex.ToString());
            return BadRequest(ex.ToString());
        }
    }

    [HttpPost("image/upload")]
    public async Task<IActionResult> ImageUploadAsync([FromForm] IFormFile file)
    {
        if (file is null) return BadRequest("File not found!");
        var folder = $"{DateTime.Now.Year}-{DateTime.Now.Month:00}-{DateTime.Now.Day}";
        var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", folder);
        if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);
        var filePath = Path.Combine(uploadPath, file.FileName);

        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        var host = Request.Host.Value;
        return Ok(new { succeeded = true, url = $"https://{host}/img/{folder}/{file.FileName}" });
    }

    [HttpGet("directories")]
    public IActionResult Folders(string path = "files", int pageIndex = 1, int pageSize = 10)
    {
        var folders = new List<dynamic>();
        string pathCombine = Path.Combine(_webHostEnvironment.WebRootPath, path);
        var directories = Directory.GetDirectories(pathCombine);

        var files = new List<dynamic>();

        var filesInDirectory = Directory.GetFiles(pathCombine).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        foreach (var item in filesInDirectory)
        {
            var file = new FileInfo(item);
            files.Add(new
            {
                file.Name,
                file.LastWriteTime,
                file.CreationTime,
                length = file.Length / 1024,
                fullName = $"/{path}/{file.Name}"
            });
        }

        foreach (var item in directories)
        {
            var directory = new DirectoryInfo(item);
            folders.Add(new
            {
                directory.CreationTime,
                directory.Name,
                directory.LastWriteTime
            });
        }
        return Ok(new
        {
            folders,
            path,
            files
        });
    }

    [HttpPost("delete")]
    public IActionResult Delete([FromBody] FileInfoModel file)
    {
        var path = _webHostEnvironment.WebRootPath + file.FullName;
        if (!System.IO.File.Exists(path))
        {
            return Ok(new { succeeded = false, message = "File not exist!" });
        }
        System.IO.File.Delete(path);
        return Ok(new { succeeded = true, message = "Succeeded!" });
    }

    [HttpGet("custom-css")]
    public IActionResult GetCustomCss() => Ok(System.IO.File.ReadAllText(Path.Combine(_webHostEnvironment.WebRootPath, "css", "style.css")));

    [HttpGet("total")]
    public async Task<IActionResult> TotalAsync()
    {
        return Ok(await _context.Attachments.CountAsync());
    }
}