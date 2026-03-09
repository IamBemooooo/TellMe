using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.Common.Interfaces;
using TellMe.Core.Common;
using TellMe.Data;

namespace TellMe.Application.Features.User.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result<Unit>>
    {
        private readonly AppDbContext _context;

        public DeleteUserCommandHandler(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(new object[] { request.Id }, cancellationToken);

            if (user == null)
            {
                return Result<Unit>.Failure("Người dùng không tồn tại");
            }

            user.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.SuccessResult(Unit.Value, "Xóa tài khoản thành công");
        }
    }
}
