using AutoStrongClient.ViewModels;
using System.Windows;

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

            var imagesViewModel = new ImagesViewModel();
            var selectTextViewModel = new SelectTextViewModel();
            var selectButtonViewModel = new SelectButtonViewModel();
            var uploadButtonViewModel = new UploadButtonViewModel(selectButtonViewModel, selectTextViewModel, imagesViewModel);

            images.DataContext = imagesViewModel;
            textImage.DataContext = selectTextViewModel;
            newImage.DataContext = selectButtonViewModel;
            selectButton.DataContext = selectButtonViewModel;
            uploadButton.DataContext = uploadButtonViewModel;
        }
    }
}