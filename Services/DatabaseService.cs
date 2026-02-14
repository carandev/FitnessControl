using SQLite;
using FitnessControl.Models;
using FitnessControl.Services;

namespace FitnessControl.Services;

public class DatabaseService
{
    private static SQLiteAsyncConnection? _database;

    public static async Task InitializeAsync()
    {
        if (_database != null) return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "fitnesscontrol.db");
        _database = new SQLiteAsyncConnection(dbPath);

        await _database.CreateTableAsync<User>();
        await _database.CreateTableAsync<WorkoutSession>();
        await _database.CreateTableAsync<Exercise>();
        await _database.CreateTableAsync<ExerciseLog>();
        await _database.CreateTableAsync<BodyProgress>();
        await _database.CreateTableAsync<NotificationSettings>();

        await SeedExercisesIfEmpty();
        await SeedDefaultUserIfEmpty();
    }

    private static async Task SeedExercisesIfEmpty()
    {
        if (_database == null) return;
        
        var count = await _database.Table<Exercise>().CountAsync();
        if (count == 0)
        {
            var exercises = DefaultRoutines.GetExercises();
            foreach (var exercise in exercises)
            {
                await _database.InsertAsync(exercise);
            }
        }
    }

    private static async Task SeedDefaultUserIfEmpty()
    {
        if (_database == null) return;

        var count = await _database.Table<User>().CountAsync();
        if (count == 0)
        {
            await _database.InsertAsync(new User
            {
                Name = "Usuario",
                Age = 30,
                Height = 175,
                Weight = 80,
                Goal = "Perder grasa y ganar músculo"
            });
        }
    }

    public static async Task<User?> GetUserAsync()
    {
        if (_database == null) await InitializeAsync();
        return await _database!.Table<User>().FirstOrDefaultAsync();
    }

    public static async Task UpdateUserAsync(User user)
    {
        if (_database == null) await InitializeAsync();
        await _database!.UpdateAsync(user);
    }

    public static async Task<List<Exercise>> GetExercisesByDayAsync(int day)
    {
        if (_database == null) await InitializeAsync();
        return await _database!.Table<Exercise>()
            .Where(e => e.DayNumber == day)
            .ToListAsync();
    }

    public static async Task<List<Exercise>> GetAllExercisesAsync()
    {
        if (_database == null) await InitializeAsync();
        return await _database!.Table<Exercise>().ToListAsync();
    }

    public static async Task<WorkoutSession> CreateSessionAsync(int day, string routineName)
    {
        if (_database == null) await InitializeAsync();
        
        var session = new WorkoutSession
        {
            RoutineDay = day,
            RoutineName = routineName,
            Date = DateTime.Now,
            IsCompleted = false
        };
        
        await _database!.InsertAsync(session);
        return session;
    }

    public static async Task UpdateSessionAsync(WorkoutSession session)
    {
        if (_database == null) await InitializeAsync();
        await _database!.UpdateAsync(session);
    }

    public static async Task<List<WorkoutSession>> GetAllSessionsAsync()
    {
        if (_database == null) await InitializeAsync();
        return await _database!.Table<WorkoutSession>()
            .OrderByDescending(s => s.Date)
            .ToListAsync();
    }

    public static async Task<List<ExerciseLog>> GetLogsBySessionAsync(int sessionId)
    {
        if (_database == null) await InitializeAsync();
        return await _database!.Table<ExerciseLog>()
            .Where(l => l.SessionId == sessionId)
            .ToListAsync();
    }

    public static async Task SaveExerciseLogAsync(ExerciseLog log)
    {
        if (_database == null) await InitializeAsync();
        
        var existing = await _database!.Table<ExerciseLog>()
            .Where(l => l.SessionId == log.SessionId && l.ExerciseId == log.ExerciseId)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
            existing.Sets = log.Sets;
            existing.Reps = log.Reps;
            existing.Weight = log.Weight;
            existing.IsCompleted = log.IsCompleted;
            await _database.UpdateAsync(existing);
        }
        else
        {
            await _database.InsertAsync(log);
        }
    }

    public static async Task AddBodyProgressAsync(BodyProgress progress)
    {
        if (_database == null) await InitializeAsync();
        await _database!.InsertAsync(progress);
    }

    public static async Task<List<BodyProgress>> GetBodyProgressAsync()
    {
        if (_database == null) await InitializeAsync();
        return await _database!.Table<BodyProgress>()
            .OrderByDescending(p => p.Date)
            .ToListAsync();
    }

    public static async Task<int> GetWorkoutStreakAsync()
    {
        if (_database == null) await InitializeAsync();
        
        var sessions = await _database!.Table<WorkoutSession>()
            .Where(s => s.IsCompleted)
            .OrderByDescending(s => s.Date)
            .ToListAsync();

        if (sessions.Count == 0) return 0;

        int streak = 0;
        var today = DateTime.Today;

        foreach (var session in sessions)
        {
            var sessionDate = session.Date.Date;
            var expectedDate = today.AddDays(-streak);

            if (sessionDate == expectedDate || sessionDate == expectedDate.AddDays(-1))
            {
                streak++;
            }
            else
            {
                break;
            }
        }

        return streak;
    }

    public static async Task<int> GetTotalWorkoutsAsync()
    {
        if (_database == null) await InitializeAsync();
        return await _database!.Table<WorkoutSession>()
            .Where(s => s.IsCompleted)
            .CountAsync();
    }

    public static async Task<NotificationSettings?> GetNotificationSettingsAsync()
    {
        if (_database == null) await InitializeAsync();
        return await _database!.Table<NotificationSettings>().FirstOrDefaultAsync();
    }

    public static async Task SaveNotificationSettingsAsync(NotificationSettings settings)
    {
        if (_database == null) await InitializeAsync();
        
        var existing = await _database!.Table<NotificationSettings>().FirstOrDefaultAsync();
        if (existing != null)
        {
            existing.Enabled = settings.Enabled;
            existing.ReminderTime = settings.ReminderTime;
            existing.DailyReminder = settings.DailyReminder;
            existing.WorkoutDaysOnly = settings.WorkoutDaysOnly;
            await _database.UpdateAsync(existing);
        }
        else
        {
            await _database.InsertAsync(settings);
        }
    }
}
