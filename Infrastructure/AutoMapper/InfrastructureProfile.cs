using AutoMapper;
using Domain.DTOs.CartItem;
using Domain.DTOs.Category;
using Domain.DTOs.Order;
using Domain.DTOs.OrderItem;
using Domain.DTOs.Product;
using Domain.DTOs.Review;
using Domain.Entities;

namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        CreateMap<CreateProductDto, Product>();
        CreateMap<Product, ProductDto>();

        CreateMap<CreateCategoryDto, Category>();
        CreateMap<Category, CategoryDto>();

        CreateMap<CreateOrderDto, Order>();
        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>();

        CreateMap<CreateCartItemDto, CartItem>();
        CreateMap<CartItem, CartItemDto>()
    .ForMember(dest => dest.ProductName,
               opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<CreateReviewDto, Review>();
        CreateMap<Review, ReviewDto>();
        CreateMap<UpdateReviewDto, Review>();

    }

}
