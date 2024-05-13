using AutoMapper;
using Movies.Models;
using Movies.Models.DTO;

namespace Movies;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<Item, ItemDTO>().ReverseMap();
        CreateMap<Item, ItemCreateDTO>().ReverseMap();
        CreateMap<Item, ItemUpdateDTO>().ReverseMap();
        CreateMap<User, UserDTO>().ReverseMap();
        CreateMap<User, UserUpdateDTO>().ReverseMap();

        CreateMap<Comment, CommentDTO>().ReverseMap();
        CreateMap<Comment, CommentCreateDTO>().ReverseMap();
        CreateMap<Comment, CommentUpdateDTO>().ReverseMap();
        CreateMap<Review, ReviewDTO>().ReverseMap();
        CreateMap<Review, ReviewCreateDTO>().ReverseMap();
        CreateMap<Review, ReviewUpdateDTO>().ReverseMap();
    }
}