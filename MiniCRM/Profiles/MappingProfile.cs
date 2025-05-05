using AutoMapper;
using MiniCRM.Models;
using MiniCRM.Dtos;

namespace MiniCRM.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Customer, CustomerDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateCustomerDto, Customer>();
            CreateMap<UpdateOrderDto, Order>();
            CreateMap<CreateCustomerDto, Customer>();
        }
    }
}
