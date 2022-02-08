using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using AutoMapper;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using ReadLater5.Models.Requests;
using ReadLater5.Models.Responses;
using Services;
using Services._Exceptions;
using Services.Abstractions;
using Services.Models.Args;
using Services.Models.Data;

namespace ReadLater5.Controllers
{
    [Route("Bookmark")]
    [ApiController]
    public class BookmarkController : Controller
    {
        private readonly IBookmarkService _bookmarkService;
        private readonly ILoginManager _loginManager;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookmarkController> _logger;
        public BookmarkController(IBookmarkService bookmarkService, ILoginManager loginManager, ICategoryService categoryService, IMapper mapper, ILogger<BookmarkController> logger)
        {
            _bookmarkService = bookmarkService;
            _loginManager = loginManager;
            _categoryService = categoryService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Updates the bookmark with new data
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateBookmark")]
        [Route("Update")]
        [Authorize]
        public async Task<IActionResult> Update([FromForm] BookmarkRequest input)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var currentUserId = await _loginManager.GetCurrentUserIdWithOutBasicAuth();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    throw new AuthErrorException("User not valid");
                }

                var args = new BookmarkArgs
                {
                    Id = input.Id,
                    URL = input.URL,
                    ShortDescription = input.ShortDescription,
                    CategoryId = input.CategoryId
                };

                await _bookmarkService.Update(args, currentUserId);

            }
            catch (AuthErrorException authError)
            {
                RedirectToAction("Error", "Home");
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Returns the Edit view.
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        [Route("Edit")]
        public async Task<IActionResult> Edit([FromQuery] int bookmarkId)
        {
            if (bookmarkId == 0)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            var currentUserId = await _loginManager.GetCurrentUserIdWithOutBasicAuth();
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Redirect("Identity/Account/Login");
            }

            var bookmark = await _bookmarkService.RetrieveById(bookmarkId);
            if (bookmark == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
            }
            var categoriesForUser = await _categoryService.GetCategoriesByUserId(currentUserId);

            ViewBag.Categories = _mapper.Map<List<Category>, List<SelectListItem>>(categoriesForUser);

            return View(bookmark);
        }
        /// <summary>
        /// Returns the create view.
        /// </summary>
        /// <returns></returns>
        //[ActionName("Create")]
        //[Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Creates bookmark with url string and short description
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Ok if result is valid,BadRequest in other case</returns>
        [Route("CreateBookmark")]
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBookmark([FromForm] BookmarkRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var currentUserId = await _loginManager.GetCurrentUserIdWithOutBasicAuth();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    throw new AuthErrorException("User not valid");
                }

                var args = new BookmarkArgs
                {
                    URL = request.URL,
                    ShortDescription = request.ShortDescription
                };

                var result = await _bookmarkService.Create(args, currentUserId);

                if (!result)
                {
                    return BadRequest();
                }

            }
            catch (AuthErrorException authError)
            {
                RedirectToAction("Error", "Home");
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
            catch (Exception e)
            {
                RedirectToAction("Index");
            }
           
            return Ok();

        }
        /// <summary>
        /// It lists all the bookmarks for the current user and shows them on index view
        /// </summary>
        /// <returns></returns>
        [Route("RetrieveAllByUserId")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var result = new List<BookmarkData>();
            try
            {
                var currentUserId = await _loginManager.GetCurrentUserIdWithOutBasicAuth();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Redirect("Identity/Account/Login");
                }
                result = await _bookmarkService.RetrieveAllByUserId(currentUserId);

            }
            catch (Exception e)
            {
                RedirectToAction("Error", "Home");
            }
            return View(result);
        }

        /// <summary>
        /// Details about bookmark selected by id
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        [Route("Retrieve")]
        [HttpGet]
        public async Task<IActionResult> Details([FromQuery] int bookmarkId)
        {
            var result = await _bookmarkService.RetrieveById(bookmarkId);

            if (result == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }

            return View(result);
        }
        /// <summary>
        /// It returns the view for Delete 
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        [Route("Delete")]
        public async Task<IActionResult> Delete([FromQuery] int bookmarkId)
        {
            if (bookmarkId == 0)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            var bookmark = await _bookmarkService.RetrieveById(bookmarkId);
            if (bookmark == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
            }
            return View(bookmark);
        }

        /// <summary>
        /// Delete function for bookmark, it deletes it by id
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        [ActionName("DeleteBookmark")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int bookmarkId)
        {
            try
            {
                await _bookmarkService.Delete(bookmarkId);
            }
            catch (Exception e)
            {
                RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        /// <summary>
        /// Returns bookmarks for users that have basic authentication (username and password)
        /// </summary>
        /// <returns></returns>
        [Route("RetrieveBookmarks")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = "BasicAuth")]
        [ProducesResponseType(typeof(BookmarkResponse<List<BookmarkData>>), 200)]
        public async Task<ActionResult<BookmarkResponse<List<BookmarkData>>>> RetrieveBookmarks()
        {
            try
            {
                var userId = await _loginManager.GetCurrentUserIdWithBasicAuth();
                
                var result = await _bookmarkService.RetrieveAllByUserId(userId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(RetrieveBookmarks)} failed with error msg - {e.Message}");
                return BadRequest();
            }
            
        }
        /// <summary>
        /// Returns bookmarks for users that have basic authentication (username and password)
        /// </summary>
        /// <returns></returns>
        [Route("RetrieveBookmarkById")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = "BasicAuth")]
        [ProducesResponseType(typeof(BookmarkResponse<BookmarkData>), 200)]
        public async Task<ActionResult<BookmarkResponse<BookmarkData>>> RetrieveBookmarkById([FromQuery] int bookmarkId)
        {
            try
            {
                var result = await _bookmarkService.RetrieveBookmarkByIdForBasicAuth(bookmarkId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(RetrieveBookmarkById)} failed with error msg - {e.Message}");
                return BadRequest();
            }
         
        }

        /// <summary>
        /// Returns bookmarks for users that have basic authentication (username and password)
        /// </summary>
        /// <returns></returns>
        [Route("UpdateBookmarkWithBasicAuth")]
        [HttpPatch]
        [Authorize(AuthenticationSchemes = "BasicAuth")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> UpdateBookmarkWithBasicAuth([FromBody] BookmarkRequest bookmarkRequest)
        {
            try
            {
                var userId = await _loginManager.GetCurrentUserIdWithBasicAuth();
                if (string.IsNullOrEmpty(userId))
                {
                    throw new AuthenticationException();
                }

                var args = new BookmarkArgs
                {
                    Id = bookmarkRequest.Id,
                    URL = bookmarkRequest.URL,
                    ShortDescription = bookmarkRequest.ShortDescription,
                    CategoryId = bookmarkRequest.CategoryId
                };
                await _bookmarkService.Update(args,userId);
                return Ok(true);
            }
            catch (AuthErrorException authError)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(UpdateBookmarkWithBasicAuth)} failed with error msg - {e.Message}");
                return false;
            }

        }
        /// <summary>
        /// Deletes bookmark with authentication
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        [Route("DeleteBookmark")]
        [HttpDelete]
        [Authorize(AuthenticationSchemes = "BasicAuth")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> DeleteBookmark([FromQuery] int bookmarkId)
        {
            try
            {
                await _bookmarkService.Delete(bookmarkId);
                return Ok(true);
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(DeleteBookmark)} failed with error msg - {e.Message}");
                return false;
            }

        }
        /// <summary>
        /// Deletes bookmark with authentication
        /// </summary>
        /// <param name="bookmarkId"></param>
        /// <returns></returns>
        [Route("CreateNewBookmark")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = "BasicAuth")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<ActionResult<bool>> CreateNewBookmark([FromBody] BookmarkRequest bookmarkRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var currentUserId = await _loginManager.GetCurrentUserIdWithBasicAuth();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    throw new AuthErrorException("User not valid");
                }

                var args = new BookmarkArgs
                {
                    URL = bookmarkRequest.URL,
                    ShortDescription = bookmarkRequest.ShortDescription
                };

                var result = await _bookmarkService.Create(args, currentUserId);

                return Ok(result);
            }
            catch (AuthErrorException authError)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(CreateNewBookmark)} failed with error msg - {e.Message}");
                return false;
            }

        }
        /// <summary>
        /// Retrieves the last five added bookmarks for user 
        /// </summary>
        /// <param name="bookmarkRequest"></param>
        /// <returns></returns>
        [Route("RetrieveLastFiveCreatedBookmarks")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = "BasicAuth")]
        [ProducesResponseType(typeof(BookmarkResponse<List<BookmarkData>>), 200)]
        public async Task<ActionResult<BookmarkResponse<List<BookmarkData>>>> RetrieveLastFiveCreatedBookmarks()
        {
            try
            {
                var currentUserId = await _loginManager.GetCurrentUserIdWithBasicAuth();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    throw new AuthErrorException("User not valid");
                }

                var result = await _bookmarkService.RetrieveLastFiveCreatedBookmarks(currentUserId);

                return Ok(result);
            }
            catch (AuthErrorException authError)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError($"{nameof(CreateNewBookmark)} failed with error msg - {e.Message}");
                return false;
            }

        }
    }
}
