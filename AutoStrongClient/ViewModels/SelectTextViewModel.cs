using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AutoStrongClient.ViewModels;

internal class SelectTextViewModel : INotifyPropertyChanged
{
    private string description = "Description";
    public string Description 
    { 
        get { return description; }
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
