using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LiquorStore.Domain.DataConstants;

namespace LiquorStore.Domain.Models
{
    public class Liquor
    {
        public int Id { get; set; }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(DescriptionMinLength)]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        [Range(PriceMinValue,LiquorPriceMaxValue)]
        public decimal Price { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Range(0,15)]
        public double AlcoholByVolume { get; set; }

        public string PromotionCode { get; set; }

        public Category Category { get; set; }

        public int CategoryId { get; set; }

        public IList<CartLine> CartLines { get; set; }
    }
}
