using AutoStrongClient.Commands;
using AutoStrongClient.Models;
using Microsoft.Win32;
using Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoStrongClient.ViewModels;

internal class MainViewModel : INotifyPropertyChanged
{
    private ObservableCollection<ImageData> images;
    public ObservableCollection<ImageData> Images 
    { 
        get => images;
        set 
        { 
            images = value;
            OnPropertyChanged();
        }
    }

    private string description = "Description";
    public string Description
    {
        get => description;
        set
        {
            description = value;
            OnPropertyChanged();
        }
    }

    private string? path;
    public string? Path
    {
        get => path;
        set
        {
            path = value;
            OnPropertyChanged();
        }
    }

    private ImageSource source;
    public ImageSource Source
    {
        get => source;
        set
        {
            source = value;
            OnPropertyChanged();
        }
    }

    private ICommand uploadCommand;
    public ICommand UploadCommand => uploadCommand ??= new RelayCommand(async _ =>
    {
        if (string.IsNullOrEmpty(Path))
        {
            return;
        }

        await using var file = File.OpenRead(Path);
        using var fileStreamContent = new StreamContent(file);
        fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        var fileName = System.IO.Path.GetFileName(Path);
        var fileNameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
        using var multipartFormContent = new MultipartFormDataContent
        {
            { fileStreamContent, "files", fileName },
            { new StringContent(Description), $"{fileNameWithoutExt}text" }
        };

        using var client = new HttpClient();
        using var response = await client.PostAsync("http://localhost:5244/FileWorker/SaveFile", multipartFormContent);
        if (response.IsSuccessStatusCode)
        {
            MessageBox.Show($"Файл успешно сохранен!");
            AddImageToList(fileNameWithoutExt, Description, new BitmapImage(new Uri(Path)));
        }
        else
        {
            var responseText = await response.Content.ReadAsStringAsync();
            MessageBox.Show($"Ошибка во время сохранения файла: {responseText}");
        }
    });

    private ICommand selectCommand;
    public ICommand SelectCommand => selectCommand ??= new RelayCommand(_ =>
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Image files|*.png;*.jpg"
        };

        if (openFileDialog.ShowDialog() is true)
        {
            Source = new BitmapImage(new Uri(openFileDialog.FileName));
        }

        Path = Source?.ToString().Remove(0, 8);
    });

    public MainViewModel()
    {
        GetAllFiles();
    }

    public void GetAllFiles()
    {
        using var client = new HttpClient();
        using var response = client.GetAsync("http://localhost:5244/FileWorker/GetAllFiles").Result;

        Images = [];
        var filesData = response.Content.ReadFromJsonAsync<IEnumerable<FileData>>().Result;
        foreach(var fileData in filesData)
        {
            AddImageToList(fileData.Name, fileData.Description, ToBitmap(fileData.Data));
        }
    }

    private void AddImageToList(string name, string? description, BitmapImage image)
    {
        var existingImage = Images.FirstOrDefault(i => i.Name == name);
        if (existingImage is not null)
        {
            existingImage.Description = description;
            existingImage.Image = image;

            return;
        }

        var item = new ImageData
        {
            Name = name,
            Description = description,
            Image = image
        };

        Images.Add(item);
    }

    private BitmapImage ToBitmap(byte[] array)
    {
        using var ms = new MemoryStream(array);
        var image = new BitmapImage();

        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = ms;
        image.EndInit();

        return image;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged is not null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
