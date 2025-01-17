﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using THPHome.Interfaces.Base;

namespace ApplicationCore.Interfaces.IRepository
{
    public interface IVideoRepository : IAsyncRepository<Video>
    {
        Task<IReadOnlyList<Video>> GetListAsync(int pageSize);
    }
}
