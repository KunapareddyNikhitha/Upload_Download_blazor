using Microsoft.AspNetCore.StaticFiles;
using Microsoft.JSInterop;

namespace BlazorRemainig.Services
{
    public interface IFileDownload
    {
        Task<List<String>> GetUploadFiles();
        Task DownloadFile(string url); 
    }
    public class FileDownload : IFileDownload
    {
        private IWebHostEnvironment _webHostEnvironment;

        private readonly IJSRuntime _js;

        public FileDownload(IWebHostEnvironment webHostEnvironment, IJSRuntime js)
		{
			_webHostEnvironment = webHostEnvironment;
			_js = js;
		}

		public async Task<List<string>> GetUploadFiles()
        {
            var base64Urls = new List<string>();
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var files = Directory.GetFiles(uploadPath);
            if(files is not null && files.Length > 0){
                foreach (var file in files)
                {
                    using(var fileInput=
                        new FileStream(file, FileMode.Open, FileAccess.Read))
                    {
                        var memorystream = new MemoryStream();
                        await fileInput.CopyToAsync(memorystream);
                        var buffer = memorystream.ToArray();
                        var fileType = GetMimeTypeForFileExtension(file);
                        base64Urls.Add($"data:{ fileType};base64,{Convert.ToBase64String(buffer)}");
                    }
                }
            }
            return base64Urls;
        }

        private string GetMimeTypeForFileExtension(string file)
        {
            const string DefaultContentType = "application/octet-stream";
            var provider = new FileExtensionContentTypeProvider();
            if(!provider.TryGetContentType(file, out string contentType))
            {
                contentType = DefaultContentType;
            }
            return contentType;
        }

        public async Task DownloadFile(string url)
        {
            await _js.InvokeVoidAsync("downloadFile", url);
        }
    }
}
