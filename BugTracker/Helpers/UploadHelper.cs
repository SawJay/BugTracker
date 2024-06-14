using BugTracker.Model;
using SawyersCodeBlog.Helpers;
using System.Text.RegularExpressions;

namespace BugTracker.Helpers
{
    public static class UploadHelper
    {
        public static readonly string DefaultProfilePicture = "/images/DefaultProfilePicture.png.png";
        public static readonly int MaxFileSize = ImageHelper.MaxFileSize;

        public static async Task<FileUpload> GetImageUploadAsync(IFormFile file)
        {
            using var ms = new MemoryStream();

            await file.CopyToAsync(ms);

            byte[] data = ms.ToArray();

            if (ms.Length > 5 * 1024 * 1024)
            {
                throw new IOException("Images must be less than 5MB");
            }

            FileUpload upload = new FileUpload()
            {
                Id = Guid.NewGuid(),
                Data = data,
                Type = file.ContentType
            };

            return upload;
        }

        public static FileUpload GetImageUpload(string dataUrl)
        {
            GroupCollection matchGroups = Regex.Match(dataUrl, @"data:(?<type>.+?);base64,(?<data>.+)").Groups;

            if (matchGroups.ContainsKey("type") && matchGroups.ContainsKey("data"))
            {
                string contentType = matchGroups["type"].Value;
                byte[] data = Convert.FromBase64String(matchGroups["data"].Value);

                if (data.Length <= MaxFileSize)
                {
                    FileUpload upload = new FileUpload()
                    {
                        Id = Guid.NewGuid(),
                        Data = data,
                        Type = contentType
                    };

                    return upload;
                }
            }

            throw new IOException("Data URL was either invalid or too large");
        }
    }
}
