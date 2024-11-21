namespace TicketAPI.Services.Scoped;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile imageFile);
    void DeleteFile(string fileNameWithExtension);
}

/// <summary>
/// Manages File interaction.
/// </summary>
public class FileService(IWebHostEnvironment environment, IConfiguration configuration) : IFileService
{
    /// <summary>
    /// Save ImageFile in Upload
    /// </summary>
    /// <param name="imageFile">FileData from Form</param>
    /// <returns>Image File Name</returns>
    public async Task<string> SaveFileAsync(IFormFile imageFile)
    {
        if (imageFile is null)
        {
            throw new ArgumentNullException(nameof(imageFile));
        }
        
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);   
        }

        string[] allowedFileExtensions = configuration.GetSection("ImageUpload:AllowedFileExtentions").Get<string[]>();
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

    /// <summary>
    /// Delete ImageFile in Upload
    /// </summary>
    /// <param name="fileNameWithExtension">FileName of Image with Extension</param>
    public void DeleteFile(string fileNameWithExtension)
    {
        if (string.IsNullOrEmpty(fileNameWithExtension))
        {
            throw new ArgumentNullException(nameof(fileNameWithExtension));
        }
        
        var contentPath = environment.ContentRootPath;
        var path = Path.Combine(contentPath, "Uploads", fileNameWithExtension);

        if (!File.Exists(path))
        {
            throw new ArgumentException($"File {fileNameWithExtension} does not exist.");
        }
        File.Delete(path);
    }
}