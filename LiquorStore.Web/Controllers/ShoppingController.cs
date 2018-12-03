using AutoMapper;
using LiquorStore.Services.Liquors;
using LiquorStore.Services.Liquors.Models;
using LiquorStore.Services.Shopping;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LiquorStore.Web.Controllers
{
    [Authorize]
    public class ShoppingController:Controller
    {
        private readonly ILiquorService liquorService;
        private readonly IOrderService orderService;

        public ShoppingController(ILiquorService liquorService, IOrderService orderService)
        {
            this.liquorService = liquorService;
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult> Catalogue()
          => View(await this.liquorService.AllWithDetails());

        [HttpGet]
        public async Task<ActionResult> AddToCart(int id)
        {
            List<LiquorBasicServiceModel> itemsInCart = new List<LiquorBasicServiceModel>();
            var product = Mapper.Map<LiquorDetailsServiceModel,LiquorBasicServiceModel>(await this.liquorService.Details(id));

            if (Session["itemsInCart"] != null)
            {
                itemsInCart = (List<LiquorBasicServiceModel>)Session["itemsInCart"];
                if (!itemsInCart.Select(i => i.Id).Contains(product.Id))
                {
                    itemsInCart.Add(product);
                    Session["itemsInCart"] = itemsInCart;
                    TempData["success"] = "Successfully added item to cart";
                }
            }

            else
            {
                itemsInCart.Add(product);
                Session["itemsInCart"] = itemsInCart;
                TempData["success"] = "Successfully added item to cart";
            }

            return RedirectToAction(nameof(Catalogue));
        }

        [HttpGet]
        public ActionResult CartSummary()
        {
            List<LiquorBasicServiceModel> itemsInCart = new List<LiquorBasicServiceModel>();

            if (Session["itemsInCart"] != null)
            {
                itemsInCart = (List<LiquorBasicServiceModel>)Session["itemsInCart"];
            }

            return View(itemsInCart);
        }

        [HttpPost]
        public async Task<ActionResult> Order(FormCollection collection)
        {
            List<LiquorBasicServiceModel> itemsInCart = new List<LiquorBasicServiceModel>();

            if (Session["itemsInCart"] != null)
            {
                itemsInCart = (List<LiquorBasicServiceModel>)Session["itemsInCart"];
            }

            await this.orderService.SaveOrder(this.HttpContext.User.Identity.GetUserId(), itemsInCart, collection["address"]);
            TempData["successOrder"] = "Successfully submitted order";

            if (Session["itemsInCart"] != null)
            {
                Session["itemsInCart"] = null;
            }

            return RedirectToAction(nameof(Catalogue));
        }

        [HttpGet]
        public async Task<ActionResult> GetLucky()
        {
            IEnumerable<LiquorDetailsServiceModel> getLuckyItems = new List<LiquorDetailsServiceModel>();

            if (Session["getLuckyItems"] != null)
            {
                getLuckyItems = (List<LiquorDetailsServiceModel>)Session["getLuckyItems"];
            }

            else
            {
                Session["getLuckyItems"] = await this.liquorService.SelectItemsForPromotion();
                getLuckyItems = (List<LiquorDetailsServiceModel>)Session["getLuckyItems"];
            }

            return View(getLuckyItems);
        }

        [HttpGet]
        public async Task<ActionResult> PromotionCodeItems()
         => View(await this.liquorService.GetItemsWithPromotionCode());

        [HttpPost]
        public async Task<ActionResult> BuyWithPromotionCode(FormCollection collection)
        {
            if (!this.orderService.IsPromotionCodeCorrect(Convert.ToInt32(collection["productId"]), collection["code"]))
            {
                TempData["error"] = "Wrong Code";
            }
            else
            {
                List<LiquorBasicServiceModel> items = new List<LiquorBasicServiceModel>();
                var product = Mapper.Map<LiquorDetailsServiceModel, LiquorBasicServiceModel>(await this.liquorService.Details
                                                                                        (Convert.ToInt32(collection["productId"])));
                items.Add(product);

                TempData["successOrder"] = "Successfully submitted order";
                await this.orderService.SaveOrder(this.HttpContext.User.Identity.GetUserId(), items, collection["address"], collection["code"]);
            }

            return RedirectToAction(nameof(PromotionCodeItems));
        }

    }
}