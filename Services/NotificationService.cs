using Plugin.LocalNotification;
using FitnessControl.Models;
using FitnessControl.Services;

namespace FitnessControl.Services;

public class NotificationService
{
    private const int DailyNotificationId = 1000;
    private const int WorkoutNotificationBaseId = 2000;

    public static async Task Initialize()
    {
        await RequestPermissions();
    }

    public static async Task RequestPermissions()
    {
        try
        {
            var enabled = await LocalNotificationCenter.Current.AreNotificationsEnabled();
            if (!enabled)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Permission error: {ex.Message}");
        }
    }

    public static async Task ScheduleDailyReminder(TimeSpan time)
    {
        await CancelAllNotifications();

        var request = new NotificationRequest
        {
            NotificationId = DailyNotificationId,
            Title = "Recordatorio de Entrenamiento",
            Description = "Es hora de entrenar! Vamos a quemar calories!",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = GetNextInstance(time),
                RepeatType = NotificationRepeat.Daily
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    public static async Task ScheduleWorkoutDaysReminder(TimeSpan time, List<int> workoutDays)
    {
        await CancelWorkoutNotifications();

        int notificationId = WorkoutNotificationBaseId;

        foreach (var day in workoutDays)
        {
            var request = new NotificationRequest
            {
                NotificationId = notificationId++,
                Title = "Dia de entrenamiento!",
                Description = "Hoy te toca entrenar. A bouger!",
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = GetNextInstanceForDayOfWeek(time, day),
                    RepeatType = NotificationRepeat.Weekly
                }
            };

            await LocalNotificationCenter.Current.Show(request);
        }
    }

    public static async Task CancelAllNotifications()
    {
        LocalNotificationCenter.Current.CancelAll();
    }

    public static async Task CancelWorkoutNotifications()
    {
        for (int i = WorkoutNotificationBaseId; i < WorkoutNotificationBaseId + 10; i++)
        {
            LocalNotificationCenter.Current.Cancel(i);
        }
    }

    public static async Task RescheduleNotifications()
    {
        var settings = await DatabaseService.GetNotificationSettingsAsync();
        
        if (settings == null || !settings.Enabled)
            return;

        if (settings.DailyReminder)
        {
            await ScheduleDailyReminder(settings.ReminderTime);
        }
        else if (settings.WorkoutDaysOnly)
        {
            var workoutDays = new List<int> { 1, 2, 3, 4, 6 };
            await ScheduleWorkoutDaysReminder(settings.ReminderTime, workoutDays);
        }
    }

    private static DateTime GetNextInstance(TimeSpan time)
    {
        var now = DateTime.Now;
        var next = new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, 0);

        if (next <= now)
            next = next.AddDays(1);

        return next;
    }

    private static DateTime GetNextInstanceForDayOfWeek(TimeSpan time, int dayOfWeek)
    {
        var now = DateTime.Now;
        var dayDiff = ((dayOfWeek - (int)now.DayOfWeek + 7) % 7);
        
        if (dayDiff == 0 && now.TimeOfDay > time)
            dayDiff = 7;

        var next = now.Date.AddDays(dayDiff).Add(time);

        return next;
    }
}
