using FitnessControl.Services;
using FitnessControl.Models;

namespace FitnessControl;

public partial class WorkoutPage : ContentPage
{
    private int _selectedDay = 1;
    private WorkoutSession? _currentSession;

    public WorkoutPage()
    {
        InitializeComponent();
        LoadSchedule();
    }

    private async void LoadSchedule()
    {
        await DatabaseService.InitializeAsync();

        var schedule = DefaultRoutines.GetRoutineSchedule();
        
        DayButtons.Children.Clear();
        foreach (var day in schedule)
        {
            var btn = new Button
            {
                Text = day.Day.ToString(),
                WidthRequest = 40,
                HeightRequest = 40,
                CornerRadius = 20,
                BackgroundColor = Color.FromArgb("#A8D8EA"),
                TextColor = Color.FromArgb("#2D3436"),
                FontAttributes = FontAttributes.Bold,
                Padding = 0
            };
            var dayNum = day.Day;
            btn.Clicked += (s, e) => SelectDay(dayNum);
            DayButtons.Add(btn);
        }

        SelectDay(1);
    }

    private async void SelectDay(int day)
    {
        _selectedDay = day;
        var schedule = DefaultRoutines.GetRoutineSchedule();
        var routine = schedule.FirstOrDefault(r => r.Day == day);

        RoutineNameLabel.Text = routine.Name;
        RoutineDescLabel.Text = routine.Description;

        var exercises = await DatabaseService.GetExercisesByDayAsync(day);
        
        ExerciseList.Children.Clear();
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
                Text = $"{ex.DefaultSets} series x {ex.DefaultReps} rep",
                FontSize = 14,
                TextColor = Color.FromArgb("#636E72"),
                Margin = new Thickness(0, 8, 0, 0)
            });
            stack.Children.Add(new Label
            {
                Text = ex.Description,
                FontSize = 12,
                TextColor = Color.FromArgb("#B2BEC3"),
                FontAttributes = FontAttributes.Italic,
                Margin = new Thickness(0, 4, 0, 0)
            });

            frame.Content = stack;
            ExerciseList.Add(frame);
        }

        StartButton.IsVisible = day != 5 && day != 7;
        FinishButton.IsVisible = _currentSession != null;
    }

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        if (_selectedDay == 5 || _selectedDay == 7) return;

        _currentSession = await DatabaseService.CreateSessionAsync(_selectedDay, RoutineNameLabel.Text);
        FinishButton.IsVisible = true;
        StartButton.IsVisible = false;
        
        await DisplayAlert("Entrenamiento Started", $"Comenzaste {RoutineNameLabel.Text}. ¡Buena suerte!", "OK");
    }

    private async void FinishButton_Clicked(object sender, EventArgs e)
    {
        if (_currentSession == null) return;

        _currentSession.IsCompleted = true;
        _currentSession.Duration = DateTime.Now - _currentSession.Date;
        await DatabaseService.UpdateSessionAsync(_currentSession);
        
        _currentSession = null;
        FinishButton.IsVisible = false;
        StartButton.IsVisible = true;
        
        await DisplayAlert("¡Entrenamiento Completado!", "Great job! Keep up the good work.", "OK");
    }
}
