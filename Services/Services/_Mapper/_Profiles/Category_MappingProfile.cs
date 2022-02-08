using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Services._Mapper._Profiles
{
    public class Category_MappingProfile : Profile
    {
        public Category_MappingProfile()
        {
            CreateMap<Category, SelectListItem>()
                .ForMember(d => d.Value, m => m.MapFrom(s => s.ID))
                .ForMember(d => d.Text, m => m.MapFrom(s => s.Name))
                ;


        }
    }
}
