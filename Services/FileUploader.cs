using System;
namespace acme.Services
{
    public interface IFileUploadService
    {
        Task<bool> UploadFile(IFormFile file);
    }
    public class FileUploadLocalService : IFileUploadService
    {
        public async Task<bool> UploadFile(IFormFile file)
        {
            string path = "";
            string pathname = "";
            try
            {
                if (file.Length > 0)
                {
                    path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "UploadedFiles"));
                    pathname = Path.Combine(path, file.FileName);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (var fileStream = new FileStream(pathname, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("File Copy Failed", ex);
            }
        }
    }
}

