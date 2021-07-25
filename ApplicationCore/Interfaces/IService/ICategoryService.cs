﻿using ApplicationCore.Entities;
using ApplicationCore.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Models.Categories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.IService
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<Category>> ListAllAsync();
        Task<Category> GetCategory(int id);
        Task<IReadOnlyList<Category>> CategoriesByType(int pageSize);
        Task<PaginatedList<Category>> ListParrentAsync(int? parrentId, int pageIndex, int pageSize);
        Task<Category> FindAsync(int id);
        Task<IEnumerable<Category>> GetListAsyc(int id);
        Task<IEnumerable<Category>> GetChildCategoriesAsync(int parentId);
        Task<dynamic> AddAsync(Category category);
        Task<dynamic> DeleteAsync(int id);
        Task<Category> GetParrentAsync(int categoryId);
        Task<List<Category>> GetListInPostAsync(long postId);
        Task<dynamic> UpdateAsync(Category category);
        Task<List<GroupCategory>> GetGroupCategories();
    }
}
