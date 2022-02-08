using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Services._Exceptions;
using Services.Abstractions;
using Services.Models.Args;
using Services.Models.Data;

namespace Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly ReadLaterDataContext _readLaterDataContext;
        private readonly ILogger<BookmarkService> _logger;
        private readonly IMapper _mapper;
        private readonly ILoginManager _loginManager;

        public BookmarkService(ReadLaterDataContext readLaterDataContext, ILogger<BookmarkService> logger, IMapper mapper, ILoginManager loginManager)
        {
            _readLaterDataContext = readLaterDataContext;
            _logger = logger;
            _mapper = mapper;
            _loginManager = loginManager;
        }

        public async Task<bool> Create(BookmarkArgs args,string userId)
        {
            try
            {
                //Creating category name
                var splitted = args.ShortDescription.Split(' ');
                Random rnd = new Random();
                var categoryName = splitted[rnd.Next(splitted.Length)] + " " + splitted[rnd.Next(splitted.Length)];

                var entityCategory = new Category
                {
                    Name = categoryName,
                    UserId = userId
                };

                var categoryId = 0;
                try
                {
                    var categoryResult = await _readLaterDataContext.Categories.AddAsync(entityCategory);

                    await _readLaterDataContext.SaveChangesAsync();

                    if (categoryResult.Entity.ID != 0)
                    {
                        categoryId = categoryResult.Entity.ID;
                    }
                }
                catch (Exception e)
                {

                }

                var entityBookmark = new Bookmark
                {
                    URL = args.URL,
                    ShortDescription = args.ShortDescription,
                    CreateDate = DateTime.Now,
                    CategoryId = categoryId == 0 ? null : categoryId,
                    UserId = userId
                };

                var result = await _readLaterDataContext.Bookmark.AddAsync(entityBookmark);

                await _readLaterDataContext.SaveChangesAsync();

                return result.Entity.ID != 0;
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Bookmark {nameof(Create)} function failed with error msg - {e.Message}");
                return false;
            }
        }

        public async Task<BookmarkData> RetrieveById(int bookmarkId)
        {
            try
            {
                var bookmark = await _readLaterDataContext.Bookmark.Where(x => x.ID == bookmarkId).Include(x => x.Category).FirstOrDefaultAsync();
                if (bookmark == null)
                {
                    return null;
                }

                return _mapper.Map<Bookmark, BookmarkData>(bookmark);
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Bookmark {nameof(RetrieveById)} function failed with error msg - {e.Message}");
                return null;
            }
        }

        public async Task Update(BookmarkArgs args,string userId)
        {
            try
            {
               
                // it uses the bookmark entity because the update is fullUpdate and the CreateDate is lost if it is not set.
                var bookmark = await _readLaterDataContext.Bookmark.Where(x => x.ID == args.Id && x.UserId == userId).FirstOrDefaultAsync();

                if (bookmark != null)
                {
                    var entity = new Bookmark
                    {
                        ID = bookmark.ID,
                        ShortDescription = args.ShortDescription,
                        URL = args.URL,
                        CategoryId = args.CategoryId,
                        UserId = userId,
                        CreateDate = bookmark.CreateDate
                    };

                    _readLaterDataContext.Update(entity);
                    await _readLaterDataContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Bookmark {nameof(Update)} function failed with error msg - {e.Message}");
            }
        }

        public async Task<List<BookmarkData>> RetrieveAllByUserId(string userId)
        {
            try
            {
                var data = await _readLaterDataContext.Bookmark.Where(x => x.UserId == userId).Include(x => x.Category).ToListAsync();
                return _mapper.Map<List<Bookmark>, List<BookmarkData>>(data);
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Bookmark {nameof(RetrieveAllByUserId)} function failed with error msg - {e.Message}");
                return null;
            }
        }

        public async Task Delete(int bookmarkId)
        {
            try
            {
                var bookmarkEntity = await _readLaterDataContext.Bookmark.Where(x => x.ID == bookmarkId).FirstOrDefaultAsync();
                if (bookmarkEntity != null)
                {

                    _readLaterDataContext.Bookmark.Remove(bookmarkEntity);
                    await _readLaterDataContext.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Bookmark {nameof(Delete)} function failed with error msg - {e.Message}");
            }
        }

        public async Task<BookmarkData> RetrieveBookmarkByIdForBasicAuth(int bookmarkId)
        {
            try
            {
                var userId = await _loginManager.GetCurrentUserIdWithBasicAuth();
                if (string.IsNullOrEmpty(userId))
                {
                    return null;
                }
                var bookmark = await _readLaterDataContext.Bookmark.Where(x => x.ID == bookmarkId && x.UserId == userId).Include(x => x.Category).FirstOrDefaultAsync();

                return _mapper.Map<Bookmark, BookmarkData>(bookmark);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<List<BookmarkData>> RetrieveLastFiveCreatedBookmarks(string userId)
        {
            try
            {
                var bookmarks = await _readLaterDataContext.Bookmark.Where(x => x.UserId == userId).OrderByDescending(x => x.CreateDate.Date).Take(5).ToListAsync();
                return _mapper.Map<List<Bookmark>, List<BookmarkData>>(bookmarks);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
