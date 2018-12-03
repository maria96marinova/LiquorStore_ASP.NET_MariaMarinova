using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquorStore.Services.Liquors.Models
{
    public class LiquorBasicServiceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        [DisplayName("Category")]
        public string CategoryName { get; set; }

        public string ImageUrl { get; set; }
    }
}
