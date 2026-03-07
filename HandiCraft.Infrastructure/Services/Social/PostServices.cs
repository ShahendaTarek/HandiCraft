using AutoMapper;
using HandiCraft.Application.DTOs.Social;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.Identity;
using HandiCraft.Domain.Posts;
using HandiCraft.Presistance.context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Infrastructure.Services.Social
{
    public class PostServices : IPostServices
    {
        private readonly IMapper _mapper;
        private readonly HandiCraftDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFavoriteServices _favoriteServices;

        public PostServices(HandiCraftDbContext context, IMapper mapper, IWebHostEnvironment env, UserManager<ApplicationUser> userManager, IFavoriteServices favoriteServices)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
            _userManager = userManager;
            _favoriteServices = favoriteServices;
        }
        public async Task<PostDto> CreatePostAsync(string userId, CreatePostDto dto)
        {
            if ((dto.MediaItems == null || !dto.MediaItems.Any()) && string.IsNullOrWhiteSpace(dto.TextContent))
            {
                throw new Exception("Can't create empty post");
            }

            var post = new Post
            {
                TextContent = dto.TextContent,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            if (dto.MediaItems != null && dto.MediaItems.Any())
            {
                var uploadFolder = Path.Combine(_env.WebRootPath, "uploads");

                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                foreach (var file in dto.MediaItems)
                {
                    var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadFolder, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    var media = new Media
                    {
                        Url = $"/uploads/{fileName}",
                        MediaType = file.ContentType.StartsWith("image") ? MediaType.Image : MediaType.Video
                    };

                    post.MediaItems.Add(media);
                }
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return _mapper.Map<PostDto>(post);
        }
        public async Task<CommentDto> AddCommentAsync(string userId, AddCommentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                throw new Exception("Comment content cannot be empty.");
            }

            var postExists = await _context.Posts.AnyAsync(p => p.Id == dto.PostId);
            if (!postExists)
            {
                throw new Exception("Post not found.");
            }
            var comments = await _context.Comments
                    .Where(c => c.PostId == dto.PostId && c.ParentCommentId == null) 
                    .Include(c => c.Replies)
                    .ToListAsync();

            if (dto.ParentCommentId.HasValue)
            {
                var parentExists = await _context.Comments.AnyAsync(c => c.Id == dto.ParentCommentId.Value);
                if (!parentExists)
                {
                    throw new Exception("Parent comment not found.");
                }
            }

            var comment = new Comment
            {
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                PostId = dto.PostId,
                ParentCommentId = dto.ParentCommentId
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return _mapper.Map<CommentDto>(comment);
        }

        public async Task<ReactDto> AddReactionAsync(string userId, AddReactDto dto)
        {
            var post = await _context.Posts
               .Include(p => p.Reactions)
               .FirstOrDefaultAsync(p => p.Id == dto.PostId);

            if (post == null)
            {
                throw new Exception("Post not found.");
            }

            var existingReaction = post.Reactions
                .FirstOrDefault(r => r.UserId == userId);

            if (existingReaction != null)
            {
                _context.Reactions.Remove(existingReaction);
                await _context.SaveChangesAsync();


                return null; 
            }

            var reaction = new Reaction
            {
                PostId = dto.PostId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reactions.Add(reaction);
            await _context.SaveChangesAsync();


            return _mapper.Map<ReactDto>(reaction);
        }
        
        public async Task<string> DeletePostAsync(string userId, Guid postId)
        {
            var post = await _context.Posts
                   .Include(p => p.MediaItems)
                   .Include(p => p.Comments)
                   .ThenInclude(c => c.Replies)
                   .Include(p => p.Reactions)
                   .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return "Post not found";
            }

            if (post.UserId != userId)
            {
                return "You are not authorized to delete this post";
            }

            

            if (!string.IsNullOrEmpty(post.MediaUrl))
            {
                foreach (var media in post.MediaItems)
                {
                    var mediaPath = Path.Combine(_env.WebRootPath, media.Url.TrimStart('/'));
                    if (File.Exists(mediaPath))
                    {
                        File.Delete(mediaPath);
                    }
                }
            }

           
            _context.Media.RemoveRange(post.MediaItems);
            _context.Reactions.RemoveRange(post.Reactions);
            foreach (var comment in post.Comments)
            {
                DeleteCommentWithReplies(comment);
            }
            _context.Comments.RemoveRange(post.Comments);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return "Post deleted successfully";
        }
        private void DeleteCommentWithReplies(Comment comment)
        {
            foreach (var reply in comment.Replies.ToList())
            {
                DeleteCommentWithReplies(reply); 
            }

            _context.Comments.Remove(comment);
        }
        public async Task<string> DeleteCommentAsync(string userId, int commentId)
        {
            var comment = await _context.Comments
         .Include(c => c.Replies)
         .FirstOrDefaultAsync(c => c.Id == commentId);

            if (comment == null)
            {
                return "Comment not found";
            }

            if (comment.UserId != userId)
            {
                return "You are not authorized to delete this comment";
            }

            
            DeleteCommentWithReplies(comment);

            await _context.SaveChangesAsync();

            return "Comment and replies deleted successfully";
        }

       

        public async Task<string> DeleteReactionAsync(string userId, int reactionId)
        {
            var reaction = await _context.Reactions.FindAsync(reactionId);

            if (reaction == null)
            {
                return "Reaction not found";
            }

            if (reaction.UserId != userId)
            {
                return "You are not allowed to delete this reaction";
            }

            var postId = await _context.Reactions
                   .Where(r => r.Id == reactionId)
                   .Select(r => r.PostId)
                   .FirstOrDefaultAsync();

            _context.Reactions.Remove(reaction);
            await _context.SaveChangesAsync();
            


            return "Reaction deleted successfully";
        }

        public async Task<string> EditPostAsync(string userId, Guid postId, string? newTextContent)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
                return "Post not found";

            if (post.UserId != userId)
                return "You are not allowed to edit this post";

            if (string.IsNullOrWhiteSpace(newTextContent) && string.IsNullOrWhiteSpace(post.MediaUrl))
                return "Post cannot be empty";

            post.TextContent = newTextContent;

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();

            return "Post updated successfully";
        }

        public async Task<string> EditCommentAsync(string userId, int commentId, string newText)
        {
            var comment = await _context.Comments.FindAsync(commentId);

            if (comment == null)
                return "Comment not found";

            if (comment.UserId != userId)
                return "You are not allowed to edit this comment";

            if (string.IsNullOrWhiteSpace(newText))
                return "Comment text cannot be empty";

            comment.Content = newText;

            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return "Comment updated successfully";
        }

        public async Task<List<PostDto>> GetAllPostsByUserIdAsync(string userId)
        {
            var posts = await _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.Comments)
                .Include(p => p.Reactions)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            var result = _mapper.Map<List<PostDto>>(posts);
            return result;
        }

        public async Task<PostDto> GetPostByIdAsync(Guid postId)
        {
            var post = await _context.Posts
               .Include(p => p.Comments)
               .Include(p => p.Reactions)
               .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                throw new Exception("Post not found.");

            return _mapper.Map<PostDto>(post);
        }

        public async Task<List<CommentDto>> GetAllCommentsAsync(Guid postId)
        {
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
                throw new Exception("Post not found.");

            var comments = await _context.Comments
                .Where(c => c.PostId == postId && c.ParentCommentId == null)
                .Include(c => c.Replies)
                .ToListAsync();

            return _mapper.Map<List<CommentDto>>(comments);
        }

        public async Task<List<ReactDto>> GetAllReactsAsync(Guid postId)
        {
            var postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
                throw new Exception("Post not found.");

            var reactions = await _context.Reactions
                .Where(r => r.PostId == postId)
                .ToListAsync();

            return _mapper.Map<List<ReactDto>>(reactions);
        }
    }
}
