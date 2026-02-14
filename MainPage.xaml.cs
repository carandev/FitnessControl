using FitnessControl.Services;
using FitnessControl.Models;

namespace FitnessControl;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        LoadData();
    }

    private async void LoadData()
    {
        await DatabaseService.InitializeAsync();

        var schedule = DefaultRoutines.GetRoutineSchedule();
        int dayOfWeek = (int)DateTime.Now.DayOfWeek;
        if (dayOfWeek == 0) dayOfWeek = 7;
        
        var todayRoutine = schedule.FirstOrDefault(r => r.Day == dayOfWeek);
        RoutineNameLabel.Text = todayRoutine.Name;
        RoutineDescLabel.Text = todayRoutine.Description;

        var streak = await DatabaseService.GetWorkoutStreakAsync();
        var total = await DatabaseService.GetTotalWorkoutsAsync();

        StreakLabel.Text = streak.ToString();
        TotalLabel.Text = total.ToString();

        var exercises = await DatabaseService.GetExercisesByDayAsync(dayOfWeek);
        foreach (var ex in exercises)
        {
            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 12,
                Padding = 16,
                Margin = new Thickness(0, 0, 0, 8)
            };

            var stack = new VerticalStackLayout();
            stack.Children.Add(new Label
            {
                Text = ex.Name,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2D3436")
            });
            stack.Children.Add(new Label
            {
                Text = ex.MuscleGroup,
                FontSize = 12,
                TextColor = Color.FromArgb("#636E72")
            });
            stack.Children.Add(new Label
            {
                Text = $"{ex.DefaultSets} series x {ex.DefaultReps} repeticiones",
                FontSize = 12,
                TextColor = Color.FromArgb("#636E72")
            });

            frame.Content = stack;
            ExerciseList.Add(frame);
        }
    }
}
