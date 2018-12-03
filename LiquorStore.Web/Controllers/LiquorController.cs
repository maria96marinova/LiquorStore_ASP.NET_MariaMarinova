using LiquorStore.Services.Categories;
using LiquorStore.Services.Liquors;
using LiquorStore.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LiquorStore.Web.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LiquorController:Controller
    {
        private readonly ILiquorService liquorService;
        private readonly ICategoryService categoryService;

        public LiquorController(ILiquorService liquorService, ICategoryService categoryService)
        {
            this.liquorService = liquorService;
            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult> All()
          => View(await this.liquorService.All());

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var categories = await this.categoryService.All();

            LiquorFormModel model = new LiquorFormModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(LiquorFormModel model, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                var categories = await this.categoryService.All();
                model.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                return View(model);
            }

            file.SaveAs(HttpContext.Server.MapPath("~/Content/Images/")+ file.FileName);
            model.ImageUrl = file.FileName;
            var success = await this.liquorService.Create(model.Name, model.Description, model.Price, model.ImageUrl,
                                                model.AlcoholByVolume, model.CategoryId, model.PromotionCode);

            if (!success)
            {
                TempData["error"] = "This category does not exist";
            }
            else
            {
                TempData["success"] = "Successfully added liquor";
            }
            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var categories = await this.categoryService.All();

            var liquorForEdit = await this.liquorService.Details(id);
            LiquorEditFormModel model = new LiquorEditFormModel
            {
                FormModel = new LiquorFormModel
                {
                    Name = liquorForEdit.Name,
                    Description = liquorForEdit.Description,
                    CategoryId = liquorForEdit.CategoryId,
                    Price = liquorForEdit.Price,
                    ImageUrl = liquorForEdit.ImageUrl,
                    AlcoholByVolume = liquorForEdit.AlcoholByVolume,
                    PromotionCode = liquorForEdit.PromotionCode,
                    Categories = categories.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                },

                Id = id
               
            };

            return View(model);

        }

        [HttpPost]
        public async Task<ActionResult> Edit(int id, LiquorEditFormModel model)
        {
            
            var success = await this.liquorService.Edit(id, model.FormModel.Name, model.FormModel.Description, model.FormModel.Price, model.FormModel.ImageUrl,
                                                model.FormModel.AlcoholByVolume, model.FormModel.CategoryId, model.FormModel.PromotionCode);

            if (!success)
            {
                TempData["error"] = "This liquor does not exist";
            }
            else
            {
                TempData["success"] = "Successfully edited liquor";
            }

            return RedirectToAction(nameof(All));

        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await this.liquorService.Remove(id);

            if (!success)
            {
                TempData["error"] = "This liquor does not exist";
                return View();
            }
            TempData["deleted"] = "Deleted liquor";

            return RedirectToAction(nameof(All));
        }

    }
}