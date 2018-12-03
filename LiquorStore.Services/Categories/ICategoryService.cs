using LiquorStore.Services.Categories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquorStore.Services.Categories
{
    public interface ICategoryService
    {
        Task Create(string name);

        Task<IEnumerable<CategoryServiceModel>> All();

        Task<bool> Remove(int id);
    }
}
