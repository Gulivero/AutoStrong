using BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Models;

namespace BLL;

public class FileWorkerBll : IFileWorkerBll
{
    private const string folderPath = @"D:\images\";

    public async Task SaveFile(HttpContext httpContext, CancellationToken ct)
    {
        var form = httpContext.Request.Form;
        foreach (var file in form.Files)
        {
            var imagePath = folderPath + file.FileName;
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var textPath = folderPath + fileName + ".txt";

            await using var textStream = new StreamWriter(textPath, false);
            await using var imageStream = new FileStream(imagePath, FileMode.Create);

            await file.CopyToAsync(imageStream, ct);
            await textStream.WriteAsync(form[$"{fileName}text"]);
        }
    }

    public async Task<IEnumerable<FileData>> GetAllFiles(CancellationToken ct)
    {
        var extensions = new[] { "*.jpg", "*.png" };
        var files = new List<string>();
        foreach (var extension in extensions)
        {
            files.AddRange(Directory.GetFiles(folderPath, extension));
        }

        var result = new List<FileData>();
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var textFile = folderPath + fileName + ".txt";
            using var reader = new StreamReader(textFile);

            var item = new FileData
            {
                Name = fileName,
                Description = await reader.ReadToEndAsync(ct),
                Data = await File.ReadAllBytesAsync(file, ct)
            };

            result.Add(item);
        }

        return result;
    }
}