using FitnessControl.Services;
using FitnessControl.Models;

namespace FitnessControl;

public partial class ProfilePage : ContentPage
{
    private User? _user;

    public ProfilePage()
    {
        InitializeComponent();
        LoadProfile();
    }

    private async void LoadProfile()
    {
        await DatabaseService.InitializeAsync();
        _user = await DatabaseService.GetUserAsync();

        if (_user != null)
        {
            NameEntry.Text = _user.Name;
            AgeEntry.Text = _user.Age.ToString();
            HeightEntry.Text = _user.Height.ToString();
            WeightEntry.Text = _user.Weight.ToString();
            GoalLabel.Text = _user.Goal;
        }

        var settings = await DatabaseService.GetNotificationSettingsAsync();
        if (settings != null)
        {
            NotificationSwitch.IsToggled = settings.Enabled;
            NotificationTimePicker.Time = settings.ReminderTime;
            DailyRadio.IsChecked = settings.DailyReminder;
            WorkoutDaysRadio.IsChecked = settings.WorkoutDaysOnly;
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        if (_user == null) return;

        _user.Name = NameEntry.Text ?? "";
        if (int.TryParse(AgeEntry.Text, out int age)) _user.Age = age;
        if (double.TryParse(HeightEntry.Text, out double height)) _user.Height = height;
        if (double.TryParse(WeightEntry.Text, out double weight)) _user.Weight = weight;

        await DatabaseService.UpdateUserAsync(_user);
        
        await DisplayAlert("Guardado", "Perfil actualizado correctamente", "OK");
    }

    private async void NotificationSwitch_Toggled(object sender, ToggledEventArgs e)
    {
        if (NotificationSwitch.IsToggled)
        {
            await NotificationService.Initialize();
        }
    }

    private void ReminderType_Changed(object sender, CheckedChangedEventArgs e)
    {
    }

    private async void SaveNotifications_Clicked(object sender, EventArgs e)
    {
        var settings = await DatabaseService.GetNotificationSettingsAsync() ?? new NotificationSettings();
        
        settings.Enabled = NotificationSwitch.IsToggled;
        settings.ReminderTime = NotificationTimePicker.Time ?? new TimeSpan(7, 0, 0);
        settings.DailyReminder = DailyRadio.IsChecked;
        settings.WorkoutDaysOnly = WorkoutDaysRadio.IsChecked;

        await DatabaseService.SaveNotificationSettingsAsync(settings);

        if (settings.Enabled)
        {
            await NotificationService.RescheduleNotifications();
        }
        else
        {
            await NotificationService.CancelAllNotifications();
        }

        await DisplayAlert("Guardado", "Configuración de notificaciones guardada", "OK");
    }
}
