﻿using THPHome.Entities;
using THPHome.Interfaces.Base;
using WebUI.Models.Filters.Settings;
using WebUI.Models.ViewModel;

namespace THPHome.Interfaces.IRepository;

public interface IBannerRepository : IAsyncRepository<Banner>
{
    Task<ListResult<Banner>> GetListAsync(BannerFilterOptions filterOptions);
    Task RemoveRangeAsync(long id);
}
