using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TellMe.Application.Common.Interfaces;
using TellMe.Application.DTOs;
using TellMe.Core.Common;
using TellMe.Data;
using TellMe.Infrastructure.Repositories;
using TellMe.Application.Features.Permission.Commands;
using RoleEntity = TellMe.Core.Entities.Role;

namespace TellMe.Application.Features.Role.Commands
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
    {
        private readonly AppDbContext _context;
        private ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateRoleCommandHandler(
            AppDbContext context,
            IMapper mapper,
            IMediator mediator,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _mapper = mapper;
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        public async Task<Result<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            // validate early
            if (request?.RoleDto == null)
                return Result<RoleDto>.Failure("Dữ liệu vai trò không hợp lệ");

            if (string.IsNullOrWhiteSpace(request.RoleDto.Name))
                return Result<RoleDto>.Failure("Tên vai trò là bắt buộc");

            if (request.RoleDto.PermissionIds == null || request.RoleDto.PermissionIds.Count < 1)
                return Result<RoleDto>.Failure("Vai trò phải có ít nhất 1 quyền");

            // Use a transaction to ensure role creation and permission assignment atomicity
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var exists = await _context.Roles.AnyAsync(r => r.Name == request.RoleDto.Name && !r.IsDeleted, cancellationToken);
                if (exists)
                {
                    return Result<RoleDto>.Failure("Tên vai trò đã tồn tại");
                }

                var Permissions = _context.Permissions
                    .Where(p => request.RoleDto.PermissionIds.Contains(p.Id) && p.IsActive && !p.IsDeleted).Select(p => p.Name).ToList();

                var role = _mapper.Map<RoleEntity>(request.RoleDto);
                role.CreatedBy = _currentUserService.GetUserId();
                _context.Roles.Add(role);
                await _context.SaveChangesAsync(cancellationToken);

                // Assign permissions by dispatching the existing command (will use the same DbContext / transaction)
                var assignCmd = new AssignPermissionsToRoleCommand
                {
                    Dto = new AssignPermissionsToRoleDto
                    {
                        RoleId = role.Id,
                        PermissionIds = request.RoleDto.PermissionIds
                    }
                };

                // If the assign handler returns Result<T>, consider checking result; here we send and rely on exceptions for failure.
                await _mediator.Send(assignCmd, cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var result = _mapper.Map<RoleDto>(role);

                return Result<RoleDto>.SuccessResult(result, "Thêm mới vai trò thành công");
            }
            catch (Exception ex)
            {
                try
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                catch
                {
                    // ignore rollback errors, original exception is more important
                }

                // Optionally log ex here if ILogger is available
                return Result<RoleDto>.Failure($"Lỗi thực thi: {ex.Message} | Chi tiết: {ex.InnerException?.Message}");
            }
        }
    }
}
