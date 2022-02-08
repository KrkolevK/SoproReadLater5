using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Models.Args;
using Services.Models.Data;

namespace Services.Abstractions
{
    public interface IBookmarkService
    {
        /// <summary>
        /// Creates bookmark for user
        /// </summary>
        /// <param name="args"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> Create(BookmarkArgs args,string userId);
        /// <summary>
        /// Retrieves bookmark by id
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        Task<BookmarkData> RetrieveById(int bookmarkId);

        /// <summary>
        /// Updates bookmark with new data for the specific user
        /// </summary>
        /// <param name="args"></param>
        /// <param name="userdId"></param>
        Task Update(BookmarkArgs args,string userId);
        /// <summary>
        /// Retrieves all bookmarks that are in db for current user
        /// </summary>
        /// <returns></returns>
        Task<List<BookmarkData>> RetrieveAllByUserId(string userId);
        /// <summary>
        /// Retrieves the bookmark by id and then removes it
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        Task Delete(int bookmarkId);

        /// <summary>
        /// Returns bookmark by id for authenticate user 
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        Task<BookmarkData> RetrieveBookmarkByIdForBasicAuth(int bookmarkId);
        /// <summary>
        /// Gets the last five created bookmarks from database for specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<BookmarkData>> RetrieveLastFiveCreatedBookmarks(string userId);

    }
}
