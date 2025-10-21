namespace PROG6212POE.Services
{
    public class ClaimFileService
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

        public ClaimFileService()
        {
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        public async Task<string?> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("No file selected for upload.");

            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(ext))
                throw new Exception("Invalid file type. Only .pdf, .docx, and .xlsx are allowed.");

            if (file.Length > 5 * 1024 * 1024) // 5MB limit
                throw new Exception("File too large. Maximum 5MB allowed.");

            var fileName = $"{Guid.NewGuid()}{ext}";
            var path = Path.Combine(_uploadPath, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }
    }
}
