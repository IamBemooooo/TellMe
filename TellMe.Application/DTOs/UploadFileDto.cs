using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TellMe.Application.DTOs
{
    public class UploadAvatarDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
    }

    public class UploadAvatarRequest
    {
        public IFormFile file { get; set; } = null!;
    }
}
