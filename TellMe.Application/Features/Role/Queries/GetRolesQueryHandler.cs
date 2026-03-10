using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.DTOs;
using TellMe.Application.Features.Role.Queries;
using TellMe.Core.Common;
using TellMe.Core.Entities;
using TellMe.Data;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<List<RoleBriefDto>>>,
    IRequestHandler<GetRoleByIdQuery, Result<RoleDto>>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GetRolesQueryHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<List<RoleBriefDto>>> Handle(GetRolesQuery request, CancellationToken ct)
    {
        var query = _context.Roles.Where(r => !r.IsDeleted);

        var totalRecords = await query.CountAsync(ct);

        var roles = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = _mapper.Map<List<RoleBriefDto>>(roles);

        return new Result<List<RoleBriefDto>>
        {
            IsSuccess = true,
            Data = dtos,
            Total = totalRecords,
            Page = request.PageNumber,
            Limit = request.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize)
        };
    }
    public async Task<Result<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken ct)
    {
        var role = await _context.Roles
            .Include(x => x.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);

        if (role == null)
            return Result<RoleDto>.Failure("Không tìm thấy vai trò");

        var dto = _mapper.Map<RoleDto>(role);
        return Result<RoleDto>.Success(dto);
    }
  
    
}