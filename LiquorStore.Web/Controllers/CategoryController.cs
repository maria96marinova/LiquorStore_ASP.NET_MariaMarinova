using LiquorStore.Domain.Models;
using LiquorStore.Services.Categories;
using LiquorStore.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LiquorStore.Web.Controllers
{
    [Authorize(Roles ="Administrator")]
    public class CategoryController:Controller
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult> All()
         => View(await this.categoryService.All());

        [HttpGet]
        public ActionResult Create()
         => View();

        [HttpPost]
        public async Task<ActionResult> Create(CategoryFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
         
            await this.categoryService.Create(model.Name);
            TempData["success"] = "Successfully added category";
            return RedirectToAction(nameof(All));
        }


        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            bool result = await this.categoryService.Remove(id);

            if (!result)
            {
                ViewData["error"] = "This category does not exist";
                return View();
            }

            return RedirectToAction(nameof(All));
        }
    }
}