using Microsoft.AspNetCore.Mvc;
using TellMe.Core.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using TellMe.Application.DTOs;

namespace TellMe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private static readonly string[] ImageExtensions =
        {
            // images
            ".jpg", ".jpeg", ".png", ".tif", ".tiff", ".heic",
        };

        private const long MaxFileSize = 15 * 1024 * 1024; // 15MB

        [HttpPost("upload-avatar")]
        public async Task<ActionResult<Result<UploadAvatarDto>>> UploadAvatar(
            [FromForm] UploadAvatarRequest request)
        {
            if (request.file == null || request.file.Length == 0)
                return BadRequest("Không có file upload");

            if (request.file.Length > MaxFileSize)
                return BadRequest("Avatar vượt quá 15MB");

            if (!request.file.ContentType.StartsWith("image/"))
                return BadRequest("File không phải hình ảnh");

            var extension = Path.GetExtension(request.file.FileName).ToLowerInvariant();
            if (!ImageExtensions.Contains(extension))
                return BadRequest("Định dạng ảnh không được hỗ trợ");

            var uploadRoot = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                "avatars");

            Directory.CreateDirectory(uploadRoot);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadRoot, fileName);

            if (extension == ".heic")
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await request.file.CopyToAsync(stream);
            }
            else
            {
                using var image = await SixLabors.ImageSharp.Image.LoadAsync(request.file.OpenReadStream());
                await image.SaveAsync(filePath, GetEncoder(extension, 85));
            }

            return Result<UploadAvatarDto>.SuccessResult(new UploadAvatarDto
            {
                FileName = fileName,
                FileUrl = $"/uploads/avatars/{fileName}"
            }, "tải ảnh thành công");
        }

        private static IImageEncoder GetEncoder(string ext, int quality)
        {
            return ext switch
            {
                ".png" => new PngEncoder(),
                ".tif" or ".tiff" => new TiffEncoder(),
                ".webp" => new WebpEncoder { Quality = quality },
                _ => new JpegEncoder { Quality = quality } // jpg, jpeg
            };
        }
    }
}
