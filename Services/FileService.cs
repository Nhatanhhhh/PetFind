using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace PetStore.Services
{
    public interface IFileService
    {
        Task<string?> SaveImageAsync(IFormFile? file, string folder);
        Task<string?> SaveImageFromUrlAsync(string? url, string folder);
        Task<string?> ProcessImageInputAsync(IFormFile? file, string? url, string folder);
        bool IsValidImageUrl(string url);
        bool IsValidImageFile(IFormFile file);
    }

    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly HttpClient _httpClient;

        public FileService(IWebHostEnvironment environment, HttpClient httpClient)
        {
            _environment = environment;
            _httpClient = httpClient;
        }

        public async Task<string?> SaveImageAsync(IFormFile? file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            if (!IsValidImageFile(file))
                throw new ArgumentException("Invalid image file format");

            var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{folder}/{fileName}";
        }

        public async Task<string?> SaveImageFromUrlAsync(string? url, string folder)
        {
            if (string.IsNullOrEmpty(url) || !IsValidImageUrl(url))
                return null;

            try
            {
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    return null;

                var contentType = response.Content.Headers.ContentType?.ToString();
                if (string.IsNullOrEmpty(contentType) || !contentType.StartsWith("image/"))
                    return null;

                var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var extension = GetExtensionFromContentType(contentType);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }

                return $"/uploads/{folder}/{fileName}";
            }
            catch
            {
                return null;
            }
        }

        public async Task<string?> ProcessImageInputAsync(
            IFormFile? file,
            string? url,
            string folder
        )
        {
            // Priority: File upload > URL
            if (file != null && file.Length > 0)
            {
                return await SaveImageAsync(file, folder);
            }
            else if (!string.IsNullOrEmpty(url))
            {
                return await SaveImageFromUrlAsync(url, folder);
            }

            return null;
        }

        public bool IsValidImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            // Basic URL validation
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;

            // Check if it's HTTP or HTTPS
            if (uri.Scheme != "http" && uri.Scheme != "https")
                return false;

            // Check for common image extensions
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var path = uri.AbsolutePath.ToLower();
            return imageExtensions.Any(ext => path.EndsWith(ext));
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file size (max 10MB)
            if (file.Length > 10 * 1024 * 1024)
                return false;

            // Check content type
            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/gif",
                "image/bmp",
                "image/webp",
            };
            return allowedTypes.Contains(file.ContentType.ToLower());
        }

        private string GetExtensionFromContentType(string contentType)
        {
            return contentType switch
            {
                "image/jpeg" => ".jpg",
                "image/jpg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                "image/webp" => ".webp",
                _ => ".jpg",
            };
        }
    }
}
