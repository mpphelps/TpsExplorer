using TpsExplorer.Core;

namespace TpsExplorer.MVVM.ViewModel;

public class MainViewModel : ObservableObject
{

    public RelayCommand BoxViewCommand { get; set; }
    public RelayCommand HgBoxViewCommand { get; set; }


    public BoxViewModel BoxVM { get; set; }
    public HgViewModel HgVM { get; set; }
    private object _currentView;

    public object CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            OnPropertyChanged();
        }
    }
    public MainViewModel()
    {
        BoxVM = new BoxViewModel();
        HgVM = new HgViewModel();
        CurrentView = BoxVM;

        BoxViewCommand = new RelayCommand(o =>
        {
            CurrentView = BoxVM;
        });
        HgBoxViewCommand = new RelayCommand(o =>
        {
            CurrentView = BoxVM;
        });
    }
}
