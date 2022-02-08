using Entity;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Services._Exceptions;
using Services.Abstractions;

namespace ReadLater5.Controllers
{
    public class CategoriesController : Controller
    {
        ICategoryService _categoryService;
        private readonly ILoginManager _loginManager;
        public CategoriesController(ICategoryService categoryService, ILoginManager loginManager)
        {
            _categoryService = categoryService;
            _loginManager = loginManager;
        }
        // GET: Categories

        public async Task<IActionResult> Index()
        {
            List<Category> model = new List<Category>();
            try
            {
                var currentUserId = await _loginManager.GetCurrentUserIdWithOutBasicAuth();
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Redirect("Identity/Account/Login");
                }
                model = await _categoryService.GetCategoriesByUserId(currentUserId);

            }
            catch (Exception e)
            {
                RedirectToAction("Error", "Home");
            }
            return View(model);
        }

        // GET: Categories/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Category category = _categoryService.GetCategory((int)id);
            if (category == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
            }
            return View(category);

        }

        // GET: Categories/Create
        //[Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryService.CreateCategory(category);
                    return RedirectToAction("Index");
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

            return View(category);
        }

        // GET: Categories/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Category category = _categoryService.GetCategory((int)id);
            if (category == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryService.UpdateCategory(category);
                    return RedirectToAction("Index");
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

            return View(category);
        }

        // GET: Categories/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status400BadRequest);
            }
            Category category = _categoryService.GetCategory((int)id);
            if (category == null)
            {
                return new StatusCodeResult(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound);
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                Category category = _categoryService.GetCategory(id);
                _categoryService.DeleteCategory(category);

            }
            catch (Exception e)
            {
                RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

    }
}
