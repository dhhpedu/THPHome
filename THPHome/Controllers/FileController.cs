﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using THPCore.Extensions;
using THPHome.Data;
using THPHome.Models.Files;
using THPIdentity.Entities;
using WebUI.Entities;
using WebUI.Foundations;
using WebUI.Interfaces.IService;
using WebUI.Models.Args.Files;
using WebUI.Models.Filters.Files;

namespace THPHome.Controllers;

public class FileController(IWebHostEnvironment _webHostEnvironment, ApplicationDbContext context, ITelegramService _telegramService, UserManager<ApplicationUser> _userManager) : BaseController(context)
{
    [HttpGet("list")]
    public async Task<IActionResult> ListAsync([FromQuery] FileFilterOptions filterOptions)
    {
        var query = from a in _context.ApplicationFiles
                    join b in _context.ApplicationFolders on a.FolderId equals b.Id
                    into ab
                    from b in ab.DefaultIfEmpty()
                    select new
                    {
                        a.Id,
                        a.Url,
                        fileName = a.Name,
                        a.FolderId,
                        folderName = b.Name,
                        a.CreatedDate,
                        a.ModifiedDate,
                        a.Size,
                        a.ContentType
                    };
        return Ok(new
        {
            data = await query.OrderByDescending(x => x.ModifiedDate).Skip((filterOptions.Current - 1) * filterOptions.PageSize).Take(filterOptions.PageSize).ToListAsync(),
            total = await query.CountAsync()
        });
    }

    [HttpPost("upload-multi")]
    public async Task<IActionResult> UploadMultiAsync([FromForm] UploadMultiArgs args)
    {
        try
        {
            if (args.Files != null || args.Files is null) return BadRequest("Không tìm thấy tệp tin!");
            var rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "files");
            var applicationFiles = new List<ApplicationFile>();
            foreach (var file in args.Files)
            {
                var folder = Guid.NewGuid().ToString();
                var folderPath = Path.Combine(rootPath, folder);
                if (Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                var filePath = Path.Combine(folderPath, file.FileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }
                var host = Request.Host.Value;
                var url = $"https://{host}/files/{file.FileName}";

                var user = await _userManager.FindByIdAsync(User.GetId());
                var applicationFile = new ApplicationFile
                {
                    ContentType = file.ContentType,
                    Name = file.FileName,
                    Size = file.Length,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Url = url,
                    CreatedBy = user?.Id,
                    FolderId = args.FolderId
                };
                applicationFiles.Add(applicationFile);
            }
            await _context.ApplicationFiles.AddRangeAsync(applicationFiles);
            await _context.SaveChangesAsync();

            return Ok(applicationFiles.Select(x => new
            {
                x.Id,
                x.CreatedDate,
                x.ContentType,
                x.Url,
                x.Name,
                x.Size,
                x.FolderId
            }));
        }
        catch (Exception ex)
        {
            await _telegramService.SendMessageAsync(ex.ToString());
            return BadRequest(ex.ToString());
        }
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
                var applicationFile = new ApplicationFile
                {
                    ContentType = file.ContentType,
                    Name = file.FileName,
                    Size = file.Length,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Url = url,
                    CreatedBy = user?.Id
                };
                await _context.ApplicationFiles.AddAsync(applicationFile);
                await _context.SaveChangesAsync();

                return Ok(new { succeeded = true, url, applicationFile.Id });
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
        try
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
            var url = $"https://{host}/img/{folder}/{file.FileName}";
            return Ok(new { succeeded = true, url });
        }
        catch (Exception ex)
        {
            await _telegramService.SendMessageAsync(ex.ToString());
            return BadRequest(ex.ToString());
        }
    }

    [HttpGet("directories")]
    public IActionResult Folders(string path = "files", int current = 1, int pageSize = 10)
    {
        var folders = new List<dynamic>();
        string pathCombine = Path.Combine(_webHostEnvironment.WebRootPath, path);
        var directories = Directory.GetDirectories(pathCombine);

        var files = new List<dynamic>();

        var filesInDirectory = Directory.GetFiles(pathCombine).Skip((current - 1) * pageSize).Take(pageSize);
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
