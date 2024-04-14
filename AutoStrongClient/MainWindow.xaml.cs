using Microsoft.Win32;
using Models;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AutoStrongClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly HttpClient client = new();
        public MainWindow()
        {
            InitializeComponent();
            GetAllFiles();
        }

        private void GetAllFiles()
        {
            using var response = client.GetAsync("http://localhost:5244/FileWorker/GetAllFiles").Result;
            var content = response.Content.ReadFromJsonAsync<IEnumerable<FileData>>().Result;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files|*.png;*.jpg"
            };

            if (openFileDialog.ShowDialog() is true)
            {
                newImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var path = newImage.Source.ToString().Remove(0, 8);

            await using var file = File.OpenRead(path);
            using var fileStreamContent = new StreamContent(file);
            fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            var fileName = Path.GetFileName(path);
            using var multipartFormContent = new MultipartFormDataContent
            {
                { fileStreamContent, "files", fileName },
                { new StringContent(textImage.Text), $"{Path.GetFileNameWithoutExtension(fileName)}text" }
            };

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
        }
    }
}