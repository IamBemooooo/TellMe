using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.DTOs;
using TellMe.Application.Features.Role;
using TellMe.Core.Common;
using TellMe.Data;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<List<RoleBriefDto>>>
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
        var roles = await _context.Roles
            .Where(r => !r.IsDeleted) 
            .ToListAsync(ct);

        var dtos = _mapper.Map<List<RoleBriefDto>>(roles);
        return Result<List<RoleBriefDto>>.Success(dtos);
    }
}