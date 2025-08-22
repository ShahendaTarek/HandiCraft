using HandiCraft.Application.DTOs.Social;
using HandiCraft.Domain.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Application.Interfaces
{
    public interface IPostServices
    {
        Task<PostDto> CreatePostAsync(string userId, CreatePostDto dto);
        Task<CommentDto> AddCommentAsync(string userId, AddCommentDto dto);
        Task<ReactDto> AddReactionAsync(string userId,AddReactDto dto);
        Task<string>DeletePostAsync(string userId,Guid postId);
        Task<string> DeleteCommentAsync(string userId,int commentId);
        Task<string> DeleteReactionAsync(string userId, int reactionId);
        Task<string> EditPostAsync(string userId, Guid postId, string? newTextContent);
        Task<string> EditCommentAsync(string userId, int commentId, string newText);
        Task<List<PostDto>> GetAllPostsByUserIdAsync(string userId);
        Task<PostDto> GetPostByIdAsync(Guid PostId);
        Task<List<CommentDto>> GetAllCommentsAsync(Guid postId);
        Task<List<ReactDto>> GetAllReactsAsync(Guid postId);
    }
}
