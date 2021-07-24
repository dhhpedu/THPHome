﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using ApplicationCore.Entities;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces.IService;
using ApplicationCore.Models.Posts;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebUI.Models;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly IBannerService _bannerService;
        private readonly IMenuService _menuService;
        public HomeController(IPostService postService, IBannerService bannerService, IMenuService menuService)
        {
            _postService = postService;
            _bannerService = bannerService;
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Slides = await _bannerService.GetListAsync(BannerType.SLIDE);
            ViewBag.LastedList = await _postService.GetLastedListAsync(5);
            ViewBag.BoxMenu = await _menuService.GetListAsync(MenuType.BOX);
            return View(await _postService.GetListByAllCategoryAsync());
        }

        public IActionResult Privacy() => View();
        public IActionResult Feedback() => View();

        public IActionResult SiteMap() => View();

        [Route("not-found.html")]
        public IActionResult PageNotFound() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
