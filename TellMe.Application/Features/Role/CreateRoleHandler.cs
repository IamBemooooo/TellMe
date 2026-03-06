using MediatR;
using TellMe.Infrastructure.Repositories;
using System.Threading;
using System.Threading.Tasks;
using System;
using RoleEntity = TellMe.Core.Entities.Role;

namespace TellMe.Application.Features.Role
{
    public class CreateRoleHandler : IRequestHandler<CreateRoleCommand, Guid>
    {
        private readonly IRepository<RoleEntity> _roleRepository;

        public CreateRoleHandler(IRepository<RoleEntity> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<Guid> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            // Check if role with same name exists
            var exists = await _roleRepository.ExistsAsync(r => r.Name == request.Name, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException($"Role with name '{request.Name}' already exists.");
            }

            var role = new RoleEntity
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true
            };

            var added = await _roleRepository.AddAsync(role, cancellationToken);
            return added.Id;
        }
    }
}
