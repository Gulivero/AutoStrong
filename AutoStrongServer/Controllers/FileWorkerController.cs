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
    public async Task<IActionResult> SaveFile([FromBody] FileData file, CancellationToken ct)
    {
        try
        {
            var path = folderPath + file.Name;
            await System.IO.File.WriteAllBytesAsync(path, file.Data, ct);

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
    public async Task<IEnumerable<FileData>> GetAllImages(CancellationToken ct)
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
