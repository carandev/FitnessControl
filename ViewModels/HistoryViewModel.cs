using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessControl.Models;
using System.Collections.ObjectModel;

namespace FitnessControl.ViewModels;

public partial class HistoryViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isLoading = true;

    public ObservableCollection<WorkoutSession> Sessions { get; } = new();

    public HistoryViewModel()
    {
        LoadSessionsAsync();
    }

    private async void LoadSessionsAsync()
    {
        try
        {
            await Services.DatabaseService.InitializeAsync();

            var sessions = await Services.DatabaseService.GetAllSessionsAsync();
            Sessions.Clear();
            foreach (var session in sessions)
            {
                Sessions.Add(session);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}
