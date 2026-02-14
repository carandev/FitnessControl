using SQLite;

namespace FitnessControl.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    public string Goal { get; set; } = "Perder grasa y ganar músculo";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class WorkoutSession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int RoutineDay { get; set; }
    public string RoutineName { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Now;
    public TimeSpan Duration { get; set; }
    public bool IsCompleted { get; set; }
}

public class Exercise
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MuscleGroup { get; set; } = string.Empty;
    public int DefaultSets { get; set; }
    public int DefaultReps { get; set; }
    public int DayNumber { get; set; }
}

public class ExerciseLog
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int ExerciseId { get; set; }
    public string ExerciseName { get; set; } = string.Empty;
    public int Sets { get; set; }
    public int Reps { get; set; }
    public double Weight { get; set; }
    public bool IsCompleted { get; set; }
}

public class BodyProgress
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public double Weight { get; set; }
    public double? Chest { get; set; }
    public double? Waist { get; set; }
    public double? Hips { get; set; }
    public double? LeftArm { get; set; }
    public double? RightArm { get; set; }
    public double? LeftThigh { get; set; }
    public double? RightThigh { get; set; }
}

public class NotificationSettings
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public bool Enabled { get; set; }
    public TimeSpan ReminderTime { get; set; } = new TimeSpan(7, 0, 0);
    public bool DailyReminder { get; set; } = true;
    public bool WorkoutDaysOnly { get; set; }
}
