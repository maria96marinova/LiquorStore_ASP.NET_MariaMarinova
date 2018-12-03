using LiquorStore.Domain;
using LiquorStore.Domain.Models;
using LiquorStore.Services.Categories.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace LiquorStore.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext dbContext;

        public CategoryService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<CategoryServiceModel>> All()
         => await this.dbContext.Categories
                                .Select(c => new CategoryServiceModel
                                {
                                    Id = c.Id,
                                    Name = c.Name
                                }).ToListAsync();

        public async Task Create(string name)
        {
            var category = new Category
            {
                Name = name
            };

            this.dbContext.Categories.Add(category);
            await this.dbContext.SaveChangesAsync();
        }

        public async Task<bool> Remove(int id)
        {
            if (!this.dbContext.Categories.Any(c => c.Id == id))
            {
                return false;
            }

            this.dbContext.Categories.Remove(this.dbContext.Categories.FirstOrDefault(c => c.Id == id));
            await this.dbContext.SaveChangesAsync();
            return true;
        }
    }
}
