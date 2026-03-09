using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TellMe.Application.DTOs;
using TellMe.Core.Common;
using TellMe.Data;

namespace TellMe.Application.Features.User.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Where(r => !r.IsDeleted)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

            if (user == null)
            {
                return Result<UserDto>.Failure($"User có Id {request.Id} không được tìm thấy");
            }

            var dto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(dto);
        }
    }
}
