using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.DTOs;
using TellMe.Core.Common;
using TellMe.Data;

namespace TellMe.Application.Features.Permission.Queries
{
    internal class GetAllRolePermissionQueryHandler : IRequestHandler<GetALlRolePermissionQuery, Result<List<PermissionDto>>>
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public GetAllRolePermissionQueryHandler(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Result<List<PermissionDto>>> Handle(GetALlRolePermissionQuery request, CancellationToken ct)
        {
            var query = _context.Permissions.AsQueryable(); 
            var totalRecords = await query.CountAsync(ct);

            var permissions = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            var dtos = _mapper.Map<List<PermissionDto>>(permissions);

            return new Result<List<PermissionDto>>
            {
                IsSuccess = true,
                Data = dtos,
                Total = totalRecords,
                Page = request.PageNumber,
                Limit = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize)
            };
        }
    }
}
