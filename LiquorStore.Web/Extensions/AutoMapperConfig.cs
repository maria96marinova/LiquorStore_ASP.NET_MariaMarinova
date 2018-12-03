using AutoMapper;
using LiquorStore.Domain.Models;
using LiquorStore.Services.Liquors.Models;
using LiquorStore.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LiquorStore.Web.Extensions
{
    public static class AutoMapperConfig
    {
        public static void ConfigureMappings()
        {
            Mapper.Initialize(c =>
            {
                c.CreateMap<LiquorBasicServiceModel, LiquorDetailsServiceModel>().ReverseMap();
                c.CreateMap<UserViewModel, ApplicationUser>().ReverseMap();

            }
           );
        }

    }


}