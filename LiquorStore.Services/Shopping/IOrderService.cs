using LiquorStore.Services.Liquors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquorStore.Services.Shopping
{
    public interface IOrderService
    {
        Task SaveOrder(string customerId, List<LiquorBasicServiceModel> items, string address, string promotionCode = null);

        bool IsPromotionCodeCorrect(int productId, string promotionCode);
    }
}
