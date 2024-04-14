using AutoStrongClient.Commands;
using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoStrongClient.ViewModels;

internal class SelectButtonViewModel : INotifyPropertyChanged
{
    private string path;
    public string Path
    {
        get { return path; }
        set
        {
            path = value;
            OnPropertyChanged();
        }
    }

    private ImageSource source;
    public ImageSource Source
    {
        get { return source; }
        set
        {
            source = value;
            OnPropertyChanged();
        }
    }

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

        Path = Source.ToString().Remove(0, 8);
    });

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string prop = "")
    {
        if (PropertyChanged is not null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
