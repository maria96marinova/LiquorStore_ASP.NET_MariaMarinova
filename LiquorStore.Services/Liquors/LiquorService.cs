using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiquorStore.Domain;
using LiquorStore.Domain.Models;
using LiquorStore.Services.Extensions;
using LiquorStore.Services.Liquors.Models;

namespace LiquorStore.Services.Liquors
{
    public class LiquorService : ILiquorService
    {
        private readonly ApplicationDbContext dbContext;

        public LiquorService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<LiquorBasicServiceModel>> All()
         => await this.dbContext.Liquors
                        .Select(c => new LiquorBasicServiceModel
                        {
                            Id=c.Id,
                            Name = c.Name,
                            Price = c.Price,
                            CategoryName = c.Category.Name,
                            ImageUrl = c.ImageUrl
                        }).ToListAsync();

        public async Task<IEnumerable<LiquorDetailsServiceModel>> AllWithDetails()
            => await this.dbContext.Liquors
                                    .Select(c => new LiquorDetailsServiceModel
                                    {
                                        Id = c.Id,
                                        Name = c.Name,
                                        Description = c.Description,
                                        Price = c.Price,
                                        ImageUrl = c.ImageUrl,
                                        CategoryName = c.Category.Name,
                                        CategoryId = c.CategoryId,
                                        AlcoholByVolume = c.AlcoholByVolume,
                                        PromotionCode = c.PromotionCode
                                    }).ToListAsync();
        

        public async Task<bool> Create(string name, string description, decimal price, string imageUrl, double alcoholByVolume, int categoryId, string promotionCode)
        {
            if (!this.dbContext.Categories.Any(c => c.Id == categoryId))
            {
                return false;
            }

            var liquor = new Liquor();
            liquor.Name = name;
            liquor.Description = description;
            liquor.Price = price;
            liquor.ImageUrl = imageUrl;
            liquor.AlcoholByVolume = alcoholByVolume;
            liquor.CategoryId = categoryId;
            liquor.PromotionCode = promotionCode;

            this.dbContext.Liquors.Add(liquor);
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<LiquorDetailsServiceModel> Details(int id)
         => await this.dbContext.Liquors.Where(l => l.Id == id)
                         .Select(c => new LiquorDetailsServiceModel
                         {
                             Id = c.Id,
                             Name = c.Name,
                             Description = c.Description,
                             Price = c.Price,
                             ImageUrl = c.ImageUrl,
                             CategoryName = c.Category.Name,
                             CategoryId = c.CategoryId,
                             AlcoholByVolume = c.AlcoholByVolume,
                             PromotionCode = c.PromotionCode
                         })
                        .FirstOrDefaultAsync();
            

        public async Task<bool> Edit(int id, string name, string description, decimal price, string imageUrl, double alcoholByVolume, int categoryId, string promotionCode)
        {
            if (!this.dbContext.Liquors.Any(l => l.Id == id) || !this.dbContext.Categories.Any(c => c.Id == categoryId))
            {
                return false;
            }

            var liquor = this.dbContext.Liquors.FirstOrDefault(l => l.Id == id);
            liquor.Name = name;
            liquor.Description = description;
            liquor.Price = price;
            liquor.ImageUrl = imageUrl;
            liquor.AlcoholByVolume = alcoholByVolume;
            liquor.CategoryId = categoryId;
            liquor.PromotionCode = promotionCode;

            await this.dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LiquorDetailsServiceModel>> GetItemsWithPromotionCode()
         => await this.dbContext.Liquors.Where(c => c.PromotionCode != null)
                                        .Select(c => new LiquorDetailsServiceModel
                                        {
                                            Id = c.Id,
                                            Name = c.Name,
                                            Description = c.Description,
                                            Price = c.Price,
                                            ImageUrl = c.ImageUrl,
                                            CategoryName = c.Category.Name,
                                            CategoryId = c.CategoryId,
                                            AlcoholByVolume = c.AlcoholByVolume,
                                            PromotionCode = c.PromotionCode
                                        }).ToListAsync();

        public async Task<bool> Remove(int id)
        {
            if (!this.dbContext.Liquors.Any(l => l.Id == id))
            {
                return false;
            }

            this.dbContext.Liquors.Remove(this.dbContext.Liquors.FirstOrDefault(l => l.Id == id));
            await this.dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LiquorDetailsServiceModel>> SelectItemsForPromotion()
        {
            List<LiquorDetailsServiceModel> itemsForPromotion = new List<LiquorDetailsServiceModel>();

            List<Liquor> allLiquors = await this.dbContext.Liquors.Include(c => c.Category).ToListAsync();

            Random r = new Random();
            int itemsNumber = r.Next(2, allLiquors.Count());

            allLiquors = ListExtensions.GetRandomElements<Liquor>(allLiquors, itemsNumber);

            double discount = r.Next(5, 20)/100.0;

            allLiquors = allLiquors.Select(i => { i.Price = i.Price - i.Price * (decimal)discount; return i; }).ToList();

            await this.dbContext.SaveChangesAsync();

            itemsForPromotion = allLiquors.Select(c => new LiquorDetailsServiceModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Price = c.Price,
                ImageUrl = c.ImageUrl,
                CategoryName = c.Category.Name,
                CategoryId = c.CategoryId,
                AlcoholByVolume = c.AlcoholByVolume,
                PromotionCode = c.PromotionCode
            }).ToList();

            return itemsForPromotion;                                                                      
        }
    }
}
