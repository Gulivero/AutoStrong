using Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;

namespace AutoStrongClient.ViewModels;

internal class ImagesViewModel
{
    public ObservableCollection<FileData> Images { get; set; } = [];

    public ImagesViewModel()
    {
        GetAllFiles();
    }

    private void GetAllFiles()
    {
        using var client = new HttpClient();
        using var response = client.GetAsync("http://localhost:5244/FileWorker/GetAllFiles").Result;

        Images = response.Content.ReadFromJsonAsync<ObservableCollection<FileData>>().Result;
    }
}
