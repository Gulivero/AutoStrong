using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace AutoStrongClient.Models;

internal class ImageData : INotifyPropertyChanged
{
    public string Name { get; set; }
    public BitmapImage Image { get; set; }

    private string? description;
    public string? Description
    {
        get => description;
        set
        {
            description = value;
            OnPropertyChanged();
        }
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
