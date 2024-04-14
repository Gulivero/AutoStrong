using AutoStrongClient.Models;
using Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace AutoStrongClient.ViewModels;

internal class ImagesViewModel : INotifyPropertyChanged
{
    private ObservableCollection<ImageData> images;
    public ObservableCollection<ImageData> Images 
    { 
        get { return  images; }
        set 
        { 
            images = value;
            OnPropertyChanged();
        }
    }

    public ImagesViewModel()
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

    public void AddImageToList(string name, string? description, BitmapImage image)
    {
        var item = new ImageData
        {
            Name = name,
            Description = description,
            Image = image
        };

        if (Images.Any(i => i.Name == name))
        {
            return;
        }

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
