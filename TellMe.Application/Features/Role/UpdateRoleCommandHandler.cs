using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.DTOs;
using TellMe.Application.Features.Role;
using TellMe.Core.Common;
using TellMe.Core.Entities;
using TellMe.Data;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UpdateRoleCommandHandler(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken ct)
    {
        var role = await _context.Roles
            .Include(r => r.RolePermissions)
            .FirstOrDefaultAsync(r => r.Id == request.RoleDto.Id, ct);

        if (role == null) return Result<RoleDto>.Failure("Không tìm thấy vai trò");

        role.Name = request.RoleDto.Name;
        role.Description = request.RoleDto.Description;

        var removedPermissions = role.RolePermissions
            .Where(rp => !request.RoleDto.PermissionIds.Contains(rp.PermissionId)).ToList();
        _context.RolePermissions.RemoveRange(removedPermissions);

        // Thêm những quyền mới chưa có trong DB
        var existingPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList();
        var newPermissionIds = request.RoleDto.PermissionIds
            .Where(pId => !existingPermissionIds.Contains(pId))
            .Select(pId => new RolePermission { RoleId = role.Id, PermissionId = pId });

        await _context.RolePermissions.AddRangeAsync(newPermissionIds, ct);

        // 4. Lưu thay đổi
        await _context.SaveChangesAsync(ct);

        return Result<RoleDto>.Success(_mapper.Map<RoleDto>(role));
    }
}