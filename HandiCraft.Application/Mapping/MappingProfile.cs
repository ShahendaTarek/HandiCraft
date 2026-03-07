using AutoMapper;
using HandiCraft.Application.DTOs.Chat;
using HandiCraft.Application.DTOs.Notification;
using HandiCraft.Application.DTOs.Orders;
using HandiCraft.Application.DTOs.ProductList;
using HandiCraft.Application.DTOs.Settings;
using HandiCraft.Application.DTOs.Social;
using HandiCraft.Application.DTOs.User;
using HandiCraft.Application.DTOs.UserConnection;
using HandiCraft.Domain.Identity;
using HandiCraft.Domain.Notifications;
using HandiCraft.Domain.Orders;
using HandiCraft.Domain.Posts;
using HandiCraft.Domain.ProductList;
using HandiCraft.Domain.UserAccount;
using HandiCraft.Domain.UserConnections;

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

            CreateMap<UpdateSettingsDto, UserSettings>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateProfileDto, ApplicationUser>()
                    .ForAllMembers(opt =>
                        opt.Condition((src, dest, srcMember) =>
                        {
                            if (srcMember is string str)
                                return !string.IsNullOrWhiteSpace(str);

                            return srcMember != null;
                        }));


            CreateMap<Favorite, FavoriteDto>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.ProductId != null ? src.Product : null));

            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.Data, opt => opt.Ignore());

            CreateMap<Chat, ChatDto>()
                .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants));

            CreateMap<ChatParticipant, ChatParticipantDto>();

            CreateMap<ChatMessage, ChatMessageDto>();


            CreateMap<Cart, CartDto>().ReverseMap();

            CreateMap<CartItem, CartItemDto>().ReverseMap();



            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.PictureUrl))
                .ForMember(d => d.Price, o => o.MapFrom(src => src.Price))
                .ForMember(d => d.Quantity, o => o.MapFrom(src => src.Quantity));

            CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.DeliveryMethod, opt => opt.MapFrom(src => src.DeliveryMethod.Name))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.SubTotal))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total()))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items));


            CreateMap<AddressDto, ShippingAddress>();

            CreateMap<DeliveryMethod, DeliveryMethodDto>().ReverseMap();

            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => PaymentStatus.Pending))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status.ToString())
            ); 
                
            CreateMap<PaymobCallbackDto, Payment>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
        
    }
}