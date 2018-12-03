using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static LiquorStore.Domain.DataConstants;

namespace LiquorStore.Web.Models
{
    public class CategoryFormModel
    {
        [Required]
        [MinLength(NameMinLength)]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }
    }
}