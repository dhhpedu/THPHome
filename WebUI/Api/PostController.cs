﻿using ApplicationCore.Constants;
using ApplicationCore.Entities;
using ApplicationCore.Interfaces.IService;
using ApplicationCore.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebUI.Models.Api.Admin;

namespace WebUI.Api
{
    [Route("api/[controller]"), Authorize]
    public class PostController : Controller
    {
        private readonly IPostService _postService;
        private readonly IPostCategoryService _postCategoryService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAttachmentService _attachmentService;
        public PostController(IAttachmentService attachmentService, IPostService postService, IPostCategoryService postCategoryService, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
        {
            _postService = postService;
            _postCategoryService = postCategoryService;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
            _attachmentService = attachmentService;
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetListAsync([FromQuery] PostFilterOptions filterOptions) => Ok(await _postService.GetListAsync(filterOptions));

        [Route("get-in-category/{id}")]
        public async Task<IActionResult> GetInCategoryAsync(int id) => Ok(await _postService.GetInCategoryAsync(id));

        [HttpPost("export")]
        public async Task<IActionResult> ExportAsync() => File(await _postService.ExportAsync(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

        [HttpPost("import")]
        public async Task<IActionResult> ImportAsync(IFormFile file) => Ok(await _postService.ImportAsync(file));

        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody]PostParam post)
        {
            post.Post.CreatedBy = _userManager.GetUserId(User);
            post.Post.ModifiedBy = _userManager.GetUserId(User);
            var data = await _postService.AddAsync(post.Post);
            if (data.Id > 0)
            {
                await _postCategoryService.AddAsync(post.ListCategoryId, data.Id);
                await _attachmentService.MapAsync(post.Attachments, data.Id);
            }
            return CreatedAtAction(nameof(AddAsync), new { succeeded = true });
        }

        [HttpPost("set-status")]
        public async Task<IActionResult> SetStatusAsync(Post post) => Ok(await _postService.SetStatusAsync(post));

        [HttpPost("remove/{id}")]
        public async Task<IActionResult> RemoveAsync([FromRoute] long id) => Ok(await _postService.RemoveAsync(id));

        [Route("get/{id}")]
        public async Task<IActionResult> GetAsync([FromRoute] long id) => Ok(await _postService.FindAsync(id));

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAsync([FromBody]PostParam post)
        {
            await _postCategoryService.DeleteAsync(post.Post.Id);
            await _postCategoryService.AddAsync(post.ListCategoryId, post.Post.Id);
            await _attachmentService.MapAsync(post.Attachments, post.Post.Id);
            post.Post.ModifiedBy = _userManager.GetUserId(User);
            return Ok(await _postService.EditAsync(post.Post));
        }

        [Route("get-list-category-id-in-post/{postId}")]
        public async Task<IActionResult> GetListCategoryIdInPostAsync(long postId) => Ok(await _postCategoryService.GetListCategoryIdInPostAsync(postId));

        [HttpGet("attachment-list-in-post/{id}")]
        public async Task<IActionResult> GetAttachmentsAsync([FromRoute] long id) => Ok(await _attachmentService.GetListInPostAsync(id));

        [Route("get-total")]
        public async Task<IActionResult> GetTotalAsync() => Ok(await _postService.GetTotalAsync());

        [Route("get-view")]
        public async Task<IActionResult> GetView() => Ok(await _postService.GetTotalViewAsync());

        [Route("get-count-in-user/{id}")]
        public async Task<IActionResult> GetCountInUserAsync([FromRoute] string id) => Ok(await _postService.GetCountInUserAsync(id));

        [Route("get-list-popular")]
        public async Task<IActionResult> GetListPopularAsync() => Ok(await _postService.GetListPopularAsync());

        [Route("get-list-in-user/{id}")]
        public async Task<IActionResult> GetListByUserAsync(string id) => Ok(await _postService.GetListByUserAsync(id));

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAsync([FromForm] IFormFile file)
        {
            if (file?.Length > 0)
            {
                var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "files");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var fileName = Guid.NewGuid();
                var extension = Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadPath, fileName + extension);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                var attach = new Attachment
                {
                    Id = fileName,
                    Extension = extension,
                    Name = file.FileName
                };

                await _attachmentService.AddAsync(attach);

                return Ok(new { succeeded = true, fileUrl = $"/files/{fileName}{extension}", attach });
            }
            return Ok(new { succeeded = false, fileUrl = "" });
        }

        [HttpPost("active/{id}")]
        public async Task<IActionResult> SetActiveAsync([FromRoute] long id)
        {
            if (User.IsInRole(RoleName.ADMIN))
            {
                return Ok(await _postService.SetActiveAsync(id));
            }
            else
            {
                return Ok(new { succeeded = false, message = "Truy cập bị từ chối!" });
            }
        }

        [HttpDelete("file/delete/{id}")]
        public async Task<IActionResult> DeleteAttachmentAsync([FromRoute] Guid id)
        {
            return Ok(await _attachmentService.DeleteAsync(id));
        }
    }
}
