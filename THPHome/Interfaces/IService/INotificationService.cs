﻿using THPHome.Entities.Notifications;
using THPHome.Services.Notifications.Models;

namespace THPHome.Interfaces.IService;

public interface INotificationService
{
    Task CreatePrivateAsync(CreatePrivateArgs args);
    Task<Notification?> GetAsync(Guid id);
    Task<int> GetUnreadCountAsync(string userName);
}
