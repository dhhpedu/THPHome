﻿using ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.IRepository
{
    public interface IAttachmentRepository : IAsyncRepository<Attachment>
    {
        Task<List<Attachment>> GetListInPostAsync(long id);
        Task RemoveRangeAsync(List<Attachment> listData);
    }
}
