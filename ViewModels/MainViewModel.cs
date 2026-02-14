using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessControl.Models;
using FitnessControl.Services;
using System.Collections.ObjectModel;

namespace FitnessControl.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private int _currentDay;

    [ObservableProperty]
    private string _currentRoutineName = string.Empty;

    [ObservableProperty]
    private string _currentRoutineDescription = string.Empty;

    [ObservableProperty]
    private int _streak;

    [ObservableProperty]
    private int _totalWorkouts;

    [ObservableProperty]
    private bool _isLoading = true;

    public ObservableCollection<Exercise> TodayExercises { get; } = new();

    public MainViewModel()
    {
        LoadDataAsync();
    }

    private async void LoadDataAsync()
    {
        try
        {
            await DatabaseService.InitializeAsync();
            
            User = await DatabaseService.GetUserAsync();
            Streak = await DatabaseService.GetWorkoutStreakAsync();
            TotalWorkouts = await DatabaseService.GetTotalWorkoutsAsync();

            var schedule = DefaultRoutines.GetRoutineSchedule();
            int dayOfWeek = (int)DateTime.Now.DayOfWeek;
            if (dayOfWeek == 0) dayOfWeek = 7;
            
            CurrentDay = dayOfWeek;
            var todayRoutine = schedule.FirstOrDefault(r => r.Day == CurrentDay);
            CurrentRoutineName = todayRoutine.Name;
            CurrentRoutineDescription = todayRoutine.Description;

            var exercises = await DatabaseService.GetExercisesByDayAsync(CurrentDay);
            TodayExercises.Clear();
            foreach (var ex in exercises)
            {
                TodayExercises.Add(ex);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task RefreshData()
    {
        IsLoading = true;
        LoadDataAsync();
    }
}
