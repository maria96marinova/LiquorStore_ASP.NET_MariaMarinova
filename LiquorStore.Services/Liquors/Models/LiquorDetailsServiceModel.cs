using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiquorStore.Services.Liquors.Models
{
    public class LiquorDetailsServiceModel:LiquorBasicServiceModel
    {
        public string Description { get; set; }

        [DisplayName("Alcohol")]
        public double AlcoholByVolume { get; set; }

        [DisplayName("Promotion Code")]
        public string PromotionCode { get; set; }

        public int CategoryId { get; set; }
    }
}
