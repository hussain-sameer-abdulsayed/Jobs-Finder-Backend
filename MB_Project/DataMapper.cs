using AutoMapper;
using MB_Project.Models;
using MB_Project.Models.DTOS.CategoryDto;
using MB_Project.Models.DTOS.OrderDto;
using MB_Project.Models.DTOS.PostDto;
using MB_Project.Models.DTOS.PostFeatureDto;
using MB_Project.Models.DTOS.PostImageDto;
using MB_Project.Models.DTOS.ReviewDto;
using MB_Project.Models.DTOS.UserDto;

namespace MB_Project
{
    public class DataMapper : Profile
    {
        public DataMapper() 
        {
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, ViewUserDto>().ReverseMap();
            CreateMap<User, UpdateUserDto>().ReverseMap();

            CreateMap<Order, CreateOrderDto>().ReverseMap();
            CreateMap<Order, UpdateOrderDto>().ReverseMap();
            CreateMap<Order, ViewOrderDto>().ReverseMap();

            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, ViewCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();

            CreateMap<Post, CreatePostDto>().ReverseMap();
            CreateMap<Post, ViewPostDto>().ReverseMap();
            CreateMap<Post, UpdatePostDto>().ReverseMap();

            CreateMap<Review, CreateReviewDto>().ReverseMap();
            CreateMap<Review, ViewReviewDto>().ReverseMap();
            CreateMap<Review, UpdateReviewDto>().ReverseMap();

            CreateMap<PostImage, CreatePostImageDto>().ReverseMap();
            CreateMap<PostImage, ViewPostImageDto>().ReverseMap();
            CreateMap<PostImage, UpdatePostImageDto>().ReverseMap();

            CreateMap<PostFeature, CreatePostFeatureDto>().ReverseMap();
            CreateMap<PostFeature, ViewPostFeatureDto>().ReverseMap();
            CreateMap<PostFeature, UpdatePostFeatureDto>().ReverseMap();

            

        }

    }
}
