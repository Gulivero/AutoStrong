using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace AutoStrongServer.Controllers;

[ApiController]
[Route("[controller]")]
public class FileWorkerController : ControllerBase
{
    private readonly ILogger<FileWorkerController> _logger;
    private readonly IFileWorkerBll _fileWorkerBll;
    public FileWorkerController(ILogger<FileWorkerController> logger, IFileWorkerBll fileWorkerBll)
    {
        _logger = logger;
        _fileWorkerBll = fileWorkerBll;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SaveFile(CancellationToken ct)
    {
        try
        {
            await _fileWorkerBll.SaveFile(HttpContext, ct);

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
            var result = await _fileWorkerBll.GetAllFiles(ct);

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
