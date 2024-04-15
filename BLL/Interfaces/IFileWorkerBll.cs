using Microsoft.AspNetCore.Http;
using Models;

namespace BLL.Interfaces;

public interface IFileWorkerBll
{
    Task SaveFile(HttpContext httpContext, CancellationToken ct);
    Task<IEnumerable<FileData>> GetAllFiles(CancellationToken ct);
}