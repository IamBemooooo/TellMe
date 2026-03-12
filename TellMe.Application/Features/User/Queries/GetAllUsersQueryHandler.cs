using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.DTOs;
using TellMe.Core.Common;
using TellMe.Data;

namespace TellMe.Application.Features.User.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<List<UserBriefDto>>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<UserBriefDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                IQueryable<TellMe.Core.Entities.User> query = _context.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission);

                if (!string.IsNullOrWhiteSpace(request.Username))
                {
                    var keyword = request.Username.Trim().ToLower();
                    query = query.Where(u =>
                        u.Username.ToLower().Contains(keyword) ||
                        (u.Name != null && u.Name.ToLower().Contains(keyword)));
                }

                if (request.RoleId.HasValue)
                {
                    query = query.Where(u =>
                        u.UserRoles.Any(ur => ur.RoleId == request.RoleId));
                }

                query = query.OrderBy(u => u.CreatedAt);

                var totalCount = await query.CountAsync(cancellationToken);

                var users = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                var userDtos = users.Select(u => new UserBriefDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Name = u.Name,
                    Email = u.Email,
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt,
                    CreatedAt = u.CreatedAt,
                    ProfileImageUrl = u.ProfileImageUrl,
                    Roles = u.UserRoles.Select(ur => new RoleBriefDto
                    {
                        Id = ur.Role.Id,
                        Name = ur.Role.Name,
                        IsActive = ur.Role.IsActive,
                        Permissions = ur.Role.RolePermissions
                        .Select(rp => new PermissionBriefDto
                        {
                            Id = rp.Permission.Id,
                            Name = rp.Permission.Name,
                            Description = rp.Permission.Description
                        }).ToList()
                    }).ToList()
                }).ToList();

                return Result<List<UserBriefDto>>.PaginatedResult(userDtos, totalCount, request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                return Result<List<UserBriefDto>>.Failure(ex.Message);
            }
        }
    }
}
