﻿using ApplicationCore.Entities;
using ApplicationCore.Enums;
using ApplicationCore.Helpers;
using ApplicationCore.Interfaces.IRepository;
using ApplicationCore.Interfaces.IService;
using ApplicationCore.Models.Posts;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationCore.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPostCategoryRepository _postCategoryRepository;
        private readonly IBannerRepository _bannerRepository;
        public PostService(
            IPostRepository postRepository,
            ICategoryRepository categoryRepository,
            IPostCategoryRepository postCategoryRepository,
            IBannerRepository bannerRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _postCategoryRepository = postCategoryRepository;
            _bannerRepository = bannerRepository;
        }

        public async Task<Post> AddAsync(Post post)
        {
            post.CreatedDate = DateTime.Now;
            post.ModifiedDate = DateTime.Now;
            post.Status = PostStatus.PUBLISH;
            post.Type = PostType.DEFAULT;
            post.View = 0;
            return await _postRepository.AddAsync(post);
        }

        public async Task<dynamic> EditAsync(Post post)
        {
            post.ModifiedDate = DateTime.Now;
            await _postRepository.UpdateAsync(post);
            return new { succeeded = true };
        }

        public async Task<byte[]> ExportAsync()
        {
            var posts = await _postRepository.ListAllAsync();
            var categories = await _categoryRepository.ListAllAsync();
            var postCategories = await _postCategoryRepository.ListAllAsync();
            return await ExcelHelper.ExportProduct(
                posts,
                categories,
                postCategories
                );
        }

        public Task<Post> FindAsync(long id) => _postRepository.FindAsync(id);

        public Task<dynamic> GetDataBarChartAsync() => _postRepository.GetDataBarChartAsync();

        public Task<IEnumerable<Post>> GetInCategoryAsync(int id) => _postRepository.GetInCategoryAsync(id);

        public Task<dynamic> GetListAsync(int pageIndex, int pageSize, string searchTerm) => _postRepository.GetListAsync(pageIndex, pageSize, searchTerm);

        public async Task<PaginatedList<PostView>> GetListInCategoryAsync(int categoryId, string searchTerm, int pageIndex) => await PaginatedList<PostView>.CreateAsync(_postRepository.GetListInCategory(categoryId, searchTerm), pageIndex, 10);

        public Task<IEnumerable<PostView>> GetListRandomAsync(int pageSize, int categoryId = 0) => _postRepository.GetListRandomAsync(pageSize, categoryId);

        public Task<IEnumerable<Post>> GetListAsync(PostType type) => _postRepository.GetListAsync(type);

        public Task<IEnumerable<Post>> GetTopViewAsync(int pageSize) => _postRepository.GetTopViewAsync(pageSize);

        public Task<int> GetTotalAsync() => _postRepository.CountAsync();

        public Task<int> GetTotalViewAsync() => _postRepository.GetTotalViewAsync();

        public async Task<dynamic> ImportAsync(IFormFile file)
        {
            var posts = ExcelHelper.ImportPost(file);
            return await _postRepository.AddRangeAsync(posts);
        }

        public Task<int> IncreaseViewAsync(Post post) => _postRepository.IncreaseViewAsync(post);

        public async Task<dynamic> RemoveAsync(long id)
        {
            var post = await _postRepository.FindAsync(id);
            await _postRepository.DeleteAsync(post);
            var postCategories = await _postCategoryRepository.GetListInPostAsync(id);
            await _postCategoryRepository.RemoveRangeAsync(postCategories);
            await _bannerRepository.RemoveRangeAsync(id);
            // todo
            return new { succeeded = true };
        }

        public Task<dynamic> SetStatusAsync(Post post) => _postRepository.SetStatusAsync(post);

        public Task<int> GetCountInUserAsync(string id) => _postRepository.GetCountInUserAsync(id);

        public Task<PaginatedList<PostView>> GetListInTagSync(string name, string searchTerm) => _postRepository.GetPostsInTagSync(name, searchTerm);

        public Task<IEnumerable<PostView>> GetRandomPostsAsync() => _postRepository.GetRandomPostsAsync();

        public Task<PaginatedList<PostView>> GetListAsync(int pageIndex) => _postRepository.GetListAsync(pageIndex);

        public Task<IEnumerable<Post>> GetListPopularAsync() => _postRepository.GetListPopularAsync();

        public Task<IEnumerable<Post>> GetListByUserAsync(string id) => _postRepository.GetListByUserAsync(id);

        public Task<IEnumerable<PostView>> GetListInTagAsync(string tagName, int pageSize) => _postRepository.GetListInTagAsync(tagName, pageSize);

        public Task<IEnumerable<PostView>> GetLastedListAsync(int pageSize) => _postRepository.GetLastedListAsync(pageSize);

        public Task<IEnumerable<Post>> GetRelatedListAsync(string keyword, int pageSize) => _postRepository.GetRelatedListAsync(keyword, pageSize);

        public Task<PaginatedList<PostView>> SearchAsync(string searchTerm, int? categoryId, int pageIndex, int pageSize) => _postRepository.SearchAsync(searchTerm, categoryId, pageIndex, pageSize);

        public Task<IEnumerable<PostView>> GetListByCategoryAsync(string normalizeName, int pageIndex, int pageSize) => _postRepository.GetListByCategoryAsync(normalizeName, pageIndex, pageSize);

        public Task<List<CategoryWithPost>> GetListByAllCategoryAsync()
        {
            return _postRepository.GetListByAllCategoryAsync();
        }

        public Task<IEnumerable<PostView>> GetListByTypeAsync(PostType type, int pageIndex, int pageSize)
        {
            return _postRepository.GetListByTypeAsync(type, pageIndex, pageSize);
        }
    }
}
