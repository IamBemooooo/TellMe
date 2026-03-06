using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.Common.Behaviors;
using TellMe.Core.Common;
using TellMe.Data;

namespace TellMe.Application.Features.User.Commands
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<Unit>>
    {
        private readonly AppDbContext _context;

        public LogoutCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var refreshTokenHash = JwtTokenHelper.HashRefreshToken(request.RefreshToken);

            var session = await _context.UserSessions
                .FirstOrDefaultAsync(x =>
                    x.Token == refreshTokenHash &&
                    !x.IsRevoked,
                    cancellationToken);

            if (session == null)
                return Result<Unit>.SuccessResult(Unit.Value);

            session.IsRevoked = true;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<Unit>.SuccessResult(Unit.Value, "Đăng xuất thành công");
        }
    }
}
