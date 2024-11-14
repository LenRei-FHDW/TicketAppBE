namespace TicketAPI.Services.Scoped;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions);
    void DeleteFile(string fileNameWithExtension);
}

public class FileService(IWebHostEnvironment environment) : IFileService
{
    public async Task<string> SaveFileAsync(IFormFile imageFile, string[] allowedFileExtensions)
    {
        if (imageFile is null)
        {
            throw new ArgumentNullException(nameof(imageFile));
        }
        
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Images");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);   
        }
        
        var ext = Path.GetExtension(imageFile.FileName);
        if (!allowedFileExtensions.Contains(ext))
        {
            throw new ArgumentException($"Only {string.Join(",", allowedFileExtensions)} are allowed.");
        }
        
        var fileName = $"{Guid.NewGuid().ToString()}{ext}";
        var fileNameWithPath = Path.Combine(path, fileName);
        using var stream = new FileStream(fileNameWithPath, FileMode.Create);
        await imageFile.CopyToAsync(stream);
        return fileName;
    }

    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }
        
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Images", fileNameWithExtension);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"File {fileNameWithExtension} does not exist.");
        }
        File.Delete(path);
    }
}