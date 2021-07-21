﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Api
{
    [Route("api/[controller]"), Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<IdentityUser> _userManager;

        public CommentController(ICommentService commentService, UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            _commentService = commentService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody] Comment comment)
        {
            var user = await _userManager.GetUserAsync(User);
            comment.CreatedBy = user.Id;
            return Ok(await _commentService.AddAsync(comment));
        }

        [HttpPost("change-status")]
        public async Task<IActionResult> ChangeStatusAsync([FromBody] Comment comment) => Ok(await _commentService.ChangeStatusAsync(comment));

        [Route("get-list")]
        public async Task<IActionResult> GetListAsync() => Ok(await _commentService.GetListAsync());

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id) => Ok(await _commentService.DeleteAsync(id));

        [Route("get-count-in-user/{id}")]
        public async Task<IActionResult> GetCountInUserAsync(string id) => Ok(await _commentService.GetCountInUserAsync(id));
    }
}
