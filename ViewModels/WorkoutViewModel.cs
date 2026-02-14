using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessControl.Models;
using FitnessControl.Services;
using System.Collections.ObjectModel;

namespace FitnessControl.ViewModels;

public partial class WorkoutViewModel : ObservableObject
{
    [ObservableProperty]
    private int _selectedDay;

    [ObservableProperty]
    private string _routineName = string.Empty;

    [ObservableProperty]
    private string _routineDescription = string.Empty;

    [ObservableProperty]
    private WorkoutSession? _currentSession;

    [ObservableProperty]
    private bool _isLoading = true;

    [ObservableProperty]
    private bool _sessionStarted;

    public ObservableCollection<Exercise> Exercises { get; } = new();
    public ObservableCollection<ExerciseLog> ExerciseLogs { get; } = new();
    public ObservableCollection<(int Day, string Name, string Description)> Schedule { get; } = new();

    public WorkoutViewModel()
    {
        LoadScheduleAsync();
    }

    private async void LoadScheduleAsync()
    {
        try
        {
            await DatabaseService.InitializeAsync();

            var schedule = DefaultRoutines.GetRoutineSchedule();
            foreach (var item in schedule)
            {
                Schedule.Add(item);
            }

            if (Schedule.Count > 0)
            {
                await SelectDay(Schedule[0].Day);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task SelectDay(int day)
    {
        SelectedDay = day;
        var routine = Schedule.FirstOrDefault(r => r.Day == day);
        RoutineName = routine.Name;
        RoutineDescription = routine.Description;

        var exercises = await DatabaseService.GetExercisesByDayAsync(day);
        Exercises.Clear();
        foreach (var ex in exercises)
        {
            Exercises.Add(ex);
        }

        if (CurrentSession != null && CurrentSession.RoutineDay == day)
        {
            var logs = await DatabaseService.GetLogsBySessionAsync(CurrentSession.Id);
            ExerciseLogs.Clear();
            foreach (var log in logs)
            {
                ExerciseLogs.Add(log);
            }
        }
        else
        {
            ExerciseLogs.Clear();
        }
    }

    [RelayCommand]
    private async Task StartWorkout()
    {
        if (SelectedDay == 5 || SelectedDay == 7)
        {
            return;
        }

        CurrentSession = await DatabaseService.CreateSessionAsync(SelectedDay, RoutineName);
        SessionStarted = true;
    }

    [RelayCommand]
    private async Task CompleteExercise(Exercise exercise)
    {
        if (CurrentSession == null) return;

        var log = new ExerciseLog
        {
            SessionId = CurrentSession.Id,
            ExerciseId = exercise.Id,
            ExerciseName = exercise.Name,
            Sets = exercise.DefaultSets,
            Reps = exercise.DefaultReps,
            Weight = 0,
            IsCompleted = true
        };

        await DatabaseService.SaveExerciseLogAsync(log);
    }

    [RelayCommand]
    private async Task FinishWorkout()
    {
        if (CurrentSession == null) return;

        CurrentSession.IsCompleted = true;
        CurrentSession.Duration = DateTime.Now - CurrentSession.Date;
        await DatabaseService.UpdateSessionAsync(CurrentSession);
        SessionStarted = false;
        CurrentSession = null;
    }
}
