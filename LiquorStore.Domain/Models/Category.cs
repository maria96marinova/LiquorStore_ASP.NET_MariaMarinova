using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static LiquorStore.Domain.DataConstants;

namespace LiquorStore.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public IList<Liquor> Liquors { get; set; }
    }
}
