using AutoStrongClient.Commands;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Input;

namespace AutoStrongClient.ViewModels;

internal class UploadButtonViewModel(SelectButtonViewModel selectButtonViewModel, SelectTextViewModel selectTextViewModel)
{
    private readonly SelectButtonViewModel _selectButtonViewModel = selectButtonViewModel;
    private readonly SelectTextViewModel _selectTextViewModel = selectTextViewModel;

    private ICommand uploadCommand;
    public ICommand UploadCommand => uploadCommand ??= new RelayCommand(async _ =>
    {
        await using var file = File.OpenRead(_selectButtonViewModel.Path);
        using var fileStreamContent = new StreamContent(file);
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var fileName = Path.GetFileName(_selectButtonViewModel.Path);
        using var multipartFormContent = new MultipartFormDataContent
        {
            { fileStreamContent, "files", fileName },
            { new StringContent(_selectTextViewModel.Description), $"{Path.GetFileNameWithoutExtension(fileName)}text" }
        };

        using var client = new HttpClient();
        using var response = await client.PostAsync("http://localhost:5244/FileWorker/SaveFile", multipartFormContent);
        if (response.IsSuccessStatusCode)
        {
            MessageBox.Show($"Файл успешно сохранен!");
        }
        else
        {
            var responseText = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Ошибка во время сохранения файла: {responseText}");
        }
    });

}
