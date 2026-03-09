using System;
using System.Collections.Generic;

namespace TellMe.Application.DTOs
{
    public class UpdateRoleDto : CreateRoleDto
    {
        public Guid Id { get; set; }
    }
}