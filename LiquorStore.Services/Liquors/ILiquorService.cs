using LiquorStore.Services.Liquors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquorStore.Services.Liquors
{
    public interface ILiquorService
    {
        Task<bool> Create(string name, string description, decimal price, string imageUrl, double alcoholByVolume,
                                int categoryId, string promotionCode);

        Task<IEnumerable<LiquorBasicServiceModel>> All();

        Task<IEnumerable<LiquorDetailsServiceModel>> AllWithDetails();

        Task<LiquorDetailsServiceModel> Details(int id);

        Task<IEnumerable<LiquorDetailsServiceModel>> SelectItemsForPromotion();

        Task<IEnumerable<LiquorDetailsServiceModel>> GetItemsWithPromotionCode();

        Task<bool> Edit(int id, string name, string description, decimal price, string imageUrl, double alcoholByVolume,
                                int categoryId, string promotionCode);

        Task<bool> Remove(int id);

    }
}
