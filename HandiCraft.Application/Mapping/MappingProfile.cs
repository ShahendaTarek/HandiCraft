using AutoMapper;
using HandiCraft.Application.DTOs.ProductList;
using HandiCraft.Application.DTOs.Social;
using HandiCraft.Application.DTOs.User;
using HandiCraft.Domain.Identity;
using HandiCraft.Domain.Posts;
using HandiCraft.Domain.ProductList;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile() 
        {
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments))
                .ForMember(dest => dest.Reactions, opt => opt.MapFrom(src => src.Reactions))
                .ForMember(dest => dest.MediaItems, opt => opt.MapFrom(src => src.MediaItems))
                .ForMember(dest => dest.TextContent, opt => opt.MapFrom(src => src.TextContent))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            CreateMap<CreatePostDto, Post>();

            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            
            CreateMap<AddCommentDto, Comment>();
            CreateMap<Reaction, ReactDto>();
            CreateMap<Media, MediaItemDto>();

            CreateMap<ApplicationUser, UserProfileDto>();
            CreateMap<AddProductDto, Product>();
            CreateMap<UpdateProductDto, Product>();
                 
            CreateMap<Product, ProductResponseDto>()
                 .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                 .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        }
        
    }
}