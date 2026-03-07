using AutoMapper;
using AutoMapper.QueryableExtensions;
using HandiCraft.Application.DTOs.User;
using HandiCraft.Application.Interfaces;
using HandiCraft.Domain.UserConnections;
using HandiCraft.Presentation;
using HandiCraft.Presistance.context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandiCraft.Infrastructure.Services.UserConnections
{
    public class UserconnectionServices:IUserConnectionsServices
    {
        private readonly HandiCraftDbContext _context;
        private readonly IMapper _mapper;

        public UserconnectionServices(HandiCraftDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> FollowUserAsync(string currentUserId, string userId)
        {
            var existingFollow = await _context.UserFollows
               .FirstOrDefaultAsync(x => x.FollowerId == currentUserId && x.FollowedId == userId);

            if (existingFollow != null)
                throw new InvalidOperationException("You already follow this user.");

            var follow = new UserFollow
            {
                FollowerId = currentUserId,
                FollowedId = userId
            };

            _context.UserFollows.Add(follow);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<List<UserProfileDto>> GetFollowersAsync(string userId)
        {
            var followers = await _context.UserFollows
                    .Where(x => x.FollowedId == userId)
                    .Select(x => x.Follower)  
                    .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider) 
                    .ToListAsync();

            return followers;
        }
        public async Task<List<UserProfileDto>> GetFollowingAsync(string userId)
        {
            var following = await _context.UserFollows
                   .Where(x => x.FollowerId == userId)
                   .Select(x => x.Followed)  
                   .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider)
                   .ToListAsync();

            return following;
        }
        public async Task<bool> UnfollowUserAsync(string currentUserId, string userId)
        {
            var follow = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FollowedId == userId);

            if (follow == null)
                throw new KeyNotFoundException("You are not following this user.");

            _context.UserFollows.Remove(follow);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> BlockUserAsync(string currentUserId, string userId)
        {
            if (currentUserId == userId)
                throw new InvalidOperationException("You cannot block yourself.");

            var alreadyBlocked = await _context.UserBlocks
                .AnyAsync(x => x.BlockerId == currentUserId && x.BlockedId == userId);

            if (alreadyBlocked)
                throw new InvalidOperationException("You already blocked this user.");

            var followExists = await _context.UserFollows
                .AnyAsync(x =>
                    (x.FollowerId == currentUserId && x.FollowedId == userId) ||
                    (x.FollowerId == userId && x.FollowedId == currentUserId));

            if (!followExists)
                throw new InvalidOperationException("You can only block users you are connected with (following/followers).");

            _context.UserBlocks.Add(new UserBlock
            {
                BlockerId = currentUserId,
                BlockedId = userId
            });

            
            var follow = await _context.UserFollows
                .FirstOrDefaultAsync(x =>
                    (x.FollowerId == currentUserId && x.FollowedId == userId) ||
                    (x.FollowerId == userId && x.FollowedId == currentUserId));

            if (follow != null)
                _context.UserFollows.Remove(follow);

            await _context.SaveChangesAsync();

            return true;
        }
       
    }

}

