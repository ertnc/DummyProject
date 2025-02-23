using AutoMapper;
using DummyProject.Entity;
using DummyProject.Model;

namespace DummyProject.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>();
        }
    }
}
