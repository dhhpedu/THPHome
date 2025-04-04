﻿using THPHome.ExternalAPI.Models;
using THPHome.Models.Args.Communications;

namespace WebUI.ExternalAPI.Interfaces;

public interface ITHPAuthen
{
    Task<ThpUserResponse?> LoginAsync(string userName, string passWord);
    Task<List<ThpUserResponse>> GetListUser(SendEmailArgs args);
}
