using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiquorStore.Web.Models
{
    public class UserEditFormModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }
    }
}