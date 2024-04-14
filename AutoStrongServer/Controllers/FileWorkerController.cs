using Microsoft.AspNetCore.Mvc;
using Models;

namespace AutoStrongServer.Controllers;

[ApiController]
[Route("[controller]")]
public class FileWorkerController : ControllerBase
{
    private const string folderPath = @"D:\images\";
    private readonly ILogger<FileWorkerController> _logger;
    public FileWorkerController(ILogger<FileWorkerController> logger)
    {
        _logger = logger;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveFile(CancellationToken ct)
    {
        try
        {
            var form = HttpContext.Request.Form;
            foreach(var file in form.Files)
            {
                var imagePath = folderPath + file.FileName;
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var textPath = folderPath + fileName + ".txt";

                await using var textStream = new StreamWriter(textPath, false);
                await using var imageStream = new FileStream(imagePath, FileMode.Create);
                await file.CopyToAsync(imageStream, ct);
                await textStream.WriteAsync(form[$"{fileName}text"]);
            }

            return Ok("Файл успешно сохранён.");
        }
        catch (Exception ex)
        {
            var message = "Ошибка во время сохранения файла.";
            _logger.LogError(ex, message);

            return BadRequest(message);
        }
    }

    [HttpGet("[action]")]
    public async Task<IEnumerable<FileData>> GetAllFiles(CancellationToken ct)
    {
        try
        {
            var result = new List<FileData>();
            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                var item = new FileData
                {
                    Name = Path.GetFileName(file),
                    Data = await System.IO.File.ReadAllBytesAsync(file, ct)
                };

                result.Add(item);
            }

            return result;
        }
        catch (Exception ex)
        {
            var message = "Ошибка во время получения файлов.";
            _logger.LogError(ex, message);

            throw;
        }
    }
}
