using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessControl.Models;

namespace FitnessControl.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _editName = string.Empty;

    [ObservableProperty]
    private int _editAge;

    [ObservableProperty]
    private double _editHeight;

    [ObservableProperty]
    private double _editWeight;

    public ProfileViewModel()
    {
        LoadUserAsync();
    }

    private async void LoadUserAsync()
    {
        try
        {
            await Services.DatabaseService.InitializeAsync();
            User = await Services.DatabaseService.GetUserAsync();

            if (User != null)
            {
                EditName = User.Name;
                EditAge = User.Age;
                EditHeight = User.Height;
                EditWeight = User.Weight;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void StartEditing()
    {
        if (User == null) return;
        EditName = User.Name;
        EditAge = User.Age;
        EditHeight = User.Height;
        EditWeight = User.Weight;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveProfile()
    {
        if (User == null) return;

        User.Name = EditName;
        User.Age = EditAge;
        User.Height = EditHeight;
        User.Weight = EditWeight;

        await Services.DatabaseService.UpdateUserAsync(User);
        IsEditing = false;
    }

    [RelayCommand]
    private void CancelEditing()
    {
        IsEditing = false;
    }
}
