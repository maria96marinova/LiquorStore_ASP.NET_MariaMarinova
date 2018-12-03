using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LiquorStore.Domain.DataConstants;

namespace LiquorStore.Domain.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [MinLength(AddressMinLength)]
        [MaxLength(AddressMaxLength)]
        public string Address { get; set; }

        public bool Shipped { get; set; }

        public IList<CartLine> CartLines { get; set; }

        public ApplicationUser Customer { get; set; }

        public string CustomerId { get; set; }
    }
}
