using AutoStrongClient.Commands;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace AutoStrongClient.ViewModels;

internal class UploadButtonViewModel(SelectButtonViewModel selectButtonViewModel, SelectTextViewModel selectTextViewModel, ImagesViewModel imagesViewModel)
{
    private readonly SelectButtonViewModel _selectButtonViewModel = selectButtonViewModel;
    private readonly SelectTextViewModel _selectTextViewModel = selectTextViewModel;
    private readonly ImagesViewModel _imagesViewModel = imagesViewModel;

    private ICommand uploadCommand;
    public ICommand UploadCommand => uploadCommand ??= new RelayCommand(async _ =>
    {
        if (_selectButtonViewModel.Path is null)
        {
            return;
        }

        await using var file = File.OpenRead(_selectButtonViewModel.Path);
        using var fileStreamContent = new StreamContent(file);
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var fileName = Path.GetFileName(_selectButtonViewModel.Path);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
        using var multipartFormContent = new MultipartFormDataContent
        {
            { fileStreamContent, "files", fileName },
            { new StringContent(_selectTextViewModel.Description), $"{fileNameWithoutExt}text" }
        };

        using var client = new HttpClient();
        using var response = await client.PostAsync("http://localhost:5244/FileWorker/SaveFile", multipartFormContent);
        if (response.IsSuccessStatusCode)
        {
            MessageBox.Show($"Файл успешно сохранен!");
            _imagesViewModel.AddImageToList(fileNameWithoutExt, _selectTextViewModel.Description, new BitmapImage(new Uri(_selectButtonViewModel.Path)));
        }
        else
        {
            var responseText = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Ошибка во время сохранения файла: {responseText}");
        }
    });

}
