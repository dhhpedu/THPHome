﻿using ApplicationCore.Models.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Api
{
    [Authorize, Route("api/[controller]")]
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync([FromForm] IFormFile file)
        {
            if (file?.Length > 0)
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "files");

                var filePath = Path.Combine(uploadPath, file.FileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok(new { succeeded = true, url = $"/files/{file.FileName}" });
            }
            return Ok(new { succeeded = false, message = "File not found", url = string.Empty });
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
    }
}