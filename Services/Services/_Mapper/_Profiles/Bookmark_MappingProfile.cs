using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entity;
using Services.Models.Data;

namespace Services._Mapper._Profiles
{
    public class Bookmark_MappingProfile : Profile
    {
        public Bookmark_MappingProfile()
        {
            //
            // Mapping from Bookmark to BookmarkData
            //
            CreateMap<Bookmark, BookmarkData>(MemberList.Destination)
                .ForMember(d => d.Id, m => m.MapFrom(s => s.ID))
                .ForMember(d => d.URL, m => m.MapFrom(s => s.URL))
                .ForMember(d => d.ShortDescription, m => m.MapFrom(s => s.ShortDescription))
                .ForMember(d => d.CategoryId, m => m.MapFrom(s => s.CategoryId))
                .ForMember(d => d.CategoryName, m => m.MapFrom(s => s.Category.Name))
                ;
        }
    }
}
