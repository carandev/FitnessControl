using FitControl.Services;
using FitControl.ViewModels;

namespace FitControl;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        var storage = new StorageService();
        BindingContext = new MainViewModel(storage);
    }
}
