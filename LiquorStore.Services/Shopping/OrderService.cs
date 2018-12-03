using LiquorStore.Domain;
using LiquorStore.Domain.Models;
using LiquorStore.Services.Liquors.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiquorStore.Services.Shopping
{
    public class OrderService:IOrderService
    {
        private readonly ApplicationDbContext dbContext;

        public OrderService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool IsPromotionCodeCorrect(int productId, string promotionCode)
         => this.dbContext.Liquors.Any(c => c.Id == productId && c.PromotionCode == promotionCode);

        public async Task SaveOrder(string customerId, List<LiquorBasicServiceModel> items, string address, string promotionCode = null)
        {
            var order = new Order();

            if (promotionCode == null)
            {
                List<CartLine> cartLineItems = items.Select(i => new CartLine
                {
                    ProductId = i.Id,
                    Price = i.Price
                }).ToList();

                order.Address = address;
                order.CustomerId = customerId;
                order.CartLines = cartLineItems;
            }

            else
            {
                var cartLineItem = this.SetReducedPrice(items.Select(c => c.Id).FirstOrDefault());
                order.CartLines = new List<CartLine>() { cartLineItem };
                order.Address = address;
                order.CustomerId = customerId;
            }

            this.dbContext.Orders.Add(order);

            await this.dbContext.SaveChangesAsync();
        }

        private CartLine SetReducedPrice(int productId)
        {
            decimal productInitialPrice = this.dbContext.Liquors.Where(c => c.Id == productId)
                                                        .Select(c => c.Price).FirstOrDefault();

            CartLine cartLine = new CartLine
            {
                ProductId = productId,
                Price = productInitialPrice - (decimal)0.5*productInitialPrice
            };

            return cartLine;
        }
    }
}
