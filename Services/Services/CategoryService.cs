using Data;
using Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services._Exceptions;
using Services.Abstractions;

namespace Services
{
    public class CategoryService : ICategoryService
    {
        private ReadLaterDataContext _ReadLaterDataContext;
        private readonly ILoginManager _loginManager;
        public CategoryService(ReadLaterDataContext readLaterDataContext, ILoginManager loginManager)
        {
            _ReadLaterDataContext = readLaterDataContext;
            _loginManager = loginManager;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            var currentUserId = await _loginManager.GetCurrentUserIdWithOutBasicAuth();
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new AuthErrorException("User not valid");
            }

            category.UserId = currentUserId;
            _ReadLaterDataContext.Add(category);
            await _ReadLaterDataContext.SaveChangesAsync();
            return category;
        }

        public async Task UpdateCategory(Category category)
        {
            var currentUserId = await _loginManager.GetCurrentUserIdWithOutBasicAuth();
            if (string.IsNullOrEmpty(currentUserId))
            {
                throw new AuthErrorException("User not valid");
            }

            category.UserId = currentUserId;

            _ReadLaterDataContext.Update(category);
            await _ReadLaterDataContext.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategoriesByUserId(string userId)
        {
            return await _ReadLaterDataContext.Categories.Where(x => x.UserId == userId).ToListAsync();
        }

        public Category GetCategory(int Id)
        {
            return _ReadLaterDataContext.Categories.Where(c => c.ID == Id).FirstOrDefault();
        }

        public Category GetCategory(string Name)
        {
            return _ReadLaterDataContext.Categories.Where(c => c.Name == Name).FirstOrDefault();
        }

        public void DeleteCategory(Category category)
        {
            _ReadLaterDataContext.Categories.Remove(category);
            _ReadLaterDataContext.SaveChanges();
        }
    }
}
