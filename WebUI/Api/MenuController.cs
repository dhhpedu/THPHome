﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces.IService;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUI.Api
{
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;
        private readonly UserManager<ApplicationUser> _userManager;

        public MenuController(IMenuService menuService, UserManager<ApplicationUser> userManager, ApplicationDbContext context) : base(context)
        {
            _menuService = menuService;
            _userManager = userManager;
        }

        [Route("get-list")]
        public async Task<IActionResult> GetListAsync(MenuType type = MenuType.MAIN)
        {
            var lang = GetLanguage();
            return Ok(await _menuService.GetListAsync(lang, type));
        }

        [HttpGet("parent-options")]
        public async Task<IActionResult> ParentOptionsAsync([FromQuery] MenuType type)
        {
            var lang = GetLanguage();
            return Ok(await _context.Menus.Where(x => x.ParrentId == 0 && x.Type == type && x.Language == lang).Select(x => new
            {
                label = x.Name,
                value = x.Id
            }).ToListAsync());
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody] Menu menu)
        {
            var user = await _userManager.GetUserAsync(User);
            menu.CreatedBy = user.Id;
            menu.ModifiedBy = user.Id;
            menu.CreatedDate = DateTime.Now;
            menu.ModifiedDate = DateTime.Now;
            menu.Language = GetLanguage();
            return Ok(await _menuService.AddAsync(menu));
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] Menu menu)
        {
            var user = await _userManager.GetUserAsync(User);
            var data = await _context.Menus.FindAsync(menu.Id);
            if (data is null) return BadRequest("Data not found!");
            data.ModifiedBy = user.Id;
            data.CreatedDate = DateTime.Now;
            data.Name = menu.Name;
            data.Description = menu.Description;
            data.Icon = menu.Icon;
            data.ParrentId = menu.ParrentId;
            data.Index = menu.Index;
            data.Url = menu.Url;
            data.Type = menu.Type;
            return Ok(await _menuService.UpdateAsync(data));
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id) => Ok(await _menuService.DeleteAsyn(id));

        [Route("parrent-list")]
        public async Task<IActionResult> ListParrentAsync(MenuType? type) => Ok(await _menuService.GetListParrentAsync(type));
    }
}
