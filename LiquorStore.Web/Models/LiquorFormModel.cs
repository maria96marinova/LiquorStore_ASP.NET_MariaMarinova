using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using static LiquorStore.Domain.DataConstants;

namespace LiquorStore.Web.Models
{
    public class LiquorFormModel
    {
        public LiquorFormModel()
        {
            this.Categories = new List<SelectListItem>();
        }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        [Range(PriceMinValue, LiquorPriceMaxValue)]
        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        [Range(0, 15)]
        [DisplayName("Alcohol")]
        public double AlcoholByVolume { get; set; }

        [DisplayName("Promotion Code")]
        public string PromotionCode { get; set; }

        [Display(Name="Category")]
        public int CategoryId { get; set; }

    }
}