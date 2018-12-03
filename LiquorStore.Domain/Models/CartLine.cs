using System.ComponentModel.DataAnnotations;
using static LiquorStore.Domain.DataConstants;

namespace LiquorStore.Domain.Models
{
    public class CartLine
    {
        public int Id { get; set; }

        [Range(PriceMinValue,CartLinePriceMaxValue)]
        public decimal Price { get; set; }

        public Liquor Product { get; set; }

        public int ProductId { get; set; }

        public Order Order { get; set; }

        public int OrderId { get; set; }
    }
}
