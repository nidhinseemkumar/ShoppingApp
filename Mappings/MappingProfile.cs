using AutoMapper;
using ShoppingApp.Models;
using ShoppingApp.DTOs;

namespace ShoppingApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserRegisterDto, User>();
            
            CreateMap<Product, ProductDto>();
            // Add more mappings as needed
        }
    }
}
