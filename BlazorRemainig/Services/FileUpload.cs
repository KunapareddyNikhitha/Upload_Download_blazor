using Microsoft.AspNetCore.Components.Forms;

namespace BlazorRemainig.Services
{
    public interface IFileUpload
    {
        Task<string> UploadFile(IBrowserFile file);
        Task<string> GeneratePreviewUr(IBrowserFile file);
    }
    public class FileUpload : IFileUpload
    {
        public IWebHostEnvironment _WebHostEnvironment;
        public readonly ILogger<FileUpload> _logger;

        public FileUpload(IWebHostEnvironment webHostEnvironment, ILogger<FileUpload> logger)
        {
            _WebHostEnvironment = webHostEnvironment;
            _logger = logger;
        }
    
        public async  Task<string> UploadFile(IBrowserFile file)
        {
           if(file is not null)
            {
                try
                {
                    var uploadPath = Path.Combine(_WebHostEnvironment.WebRootPath,"uploads",file.Name);
                    using(var stream = file.OpenReadStream())
                    {
                        var fileStream = File.Create(uploadPath);
                        await stream.CopyToAsync(fileStream);
                        fileStream.Close();
                    }
                    return "true";
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.ToString());
                    return "false";
                }
            }
            return "false";
        }

        public async Task<string> GeneratePreviewUr(IBrowserFile file)
        {
            if (!file.ContentType.Contains("image"))
            {
                if (file.ContentType.Contains("pdf"))
                {
                    return "images/pdf.svg";
                }
            }
            
            var resizedImage = await file.RequestImageFileAsync(file.ContentType, 100, 100);
            var buffer = new byte[resizedImage.Size];
            await resizedImage.OpenReadStream().ReadAsync(buffer);
            return $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";

        }
    }
}
