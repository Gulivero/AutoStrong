using Microsoft.Win32;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AutoStrongClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

            using var client = new HttpClient();
            using var response = await client.PostAsync("http://localhost:5244/FileWorker/SaveFile", multipartFormContent);

            var responseText = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                MessageBox.Show($"Файл успешно сохранен!");
            }
            else
            {
                MessageBox.Show($"Ошибка во время сохранения файла: {responseText}");
            }
        }
    }
}