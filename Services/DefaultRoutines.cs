namespace FitnessControl.Services;

public static class DefaultRoutines
{
    public static List<Models.Exercise> GetExercises()
    {
        return new List<Models.Exercise>
        {
            new() { Name = "Sentadillas con peso corporal", Description = "Pies al ancho de hombros, baja hasta que los muslos estén paralelos al suelo", MuscleGroup = "Pierna", DefaultSets = 3, DefaultReps = 12, DayNumber = 1 },
            new() { Name = "Zancadas caminatas", Description = "Da pasos adelante alternando piernas, manteniendo el torso erguido", MuscleGroup = "Pierna", DefaultSets = 3, DefaultReps = 10, DayNumber = 1 },
            new() { Name = "Elevación de talones", Description = "De pie, eleva los talones lo máximo posible", MuscleGroup = "Pierna", DefaultSets = 3, DefaultReps = 15, DayNumber = 1 },
            new() { Name = "Puente de glúteos", Description = "Acostado boca arriba, eleva las caderas tightened los glúteos", MuscleGroup = "Glúteo", DefaultSets = 3, DefaultReps = 12, DayNumber = 1 },
            new() { Name = "Peso muerto con una pierna", Description = "De pie, inclínate hacia adelante manteniendo una pierna elevada", MuscleGroup = "Pierna", DefaultSets = 3, DefaultReps = 8, DayNumber = 1 },

            new() { Name = "Flexiones de pared", Description = "Manos en pared, baja el pecho hacia la pared", MuscleGroup = "Pecho", DefaultSets = 3, DefaultReps = 12, DayNumber = 2 },
            new() { Name = "Flexiones de rodillas", Description = "En el suelo, rodillas apoyadas, haz flexiones", MuscleGroup = "Pecho", DefaultSets = 3, DefaultReps = 10, DayNumber = 2 },
            new() { Name = "Press con mancuernas", Description = "Acostado, pushes mancuernas hacia arriba", MuscleGroup = "Pecho", DefaultSets = 3, DefaultReps = 10, DayNumber = 2 },
            new() { Name = "Fondos de tríceps en silla", Description = "Manos en silla, baja el cuerpo flexionando los codos", MuscleGroup = "Tríceps", DefaultSets = 3, DefaultReps = 8, DayNumber = 2 },
            new() { Name = "Extensiones de tríceps", Description = "Mancuerna con ambas manos, extiende hacia arriba", MuscleGroup = "Tríceps", DefaultSets = 3, DefaultReps = 12, DayNumber = 2 },

            new() { Name = "Remo con mancuerna", Description = "Una mano y rodilla apoiados, tira la mancuerna hacia el costado", MuscleGroup = "Espalda", DefaultSets = 3, DefaultReps = 10, DayNumber = 3 },
            new() { Name = "Polea gravitatoria", Description = "Con una banda o toalla, tira hacia abajo simulando polea", MuscleGroup = "Espalda", DefaultSets = 3, DefaultReps = 12, DayNumber = 3 },
            new() { Name = "Superman", Description = "Acostado boca abajo, eleva brazos y piernas", MuscleGroup = "Espalda", DefaultSets = 3, DefaultReps = 10, DayNumber = 3 },
            new() { Name = "Curl de bíceps con mancuernas", Description = "De pie, dobla los codos bringing las mancuernas a los hombros", MuscleGroup = "Bíceps", DefaultSets = 3, DefaultReps = 12, DayNumber = 3 },
            new() { Name = "Curl martillo", Description = "Mancuernas con palma hacia adentro, eleva hacia hombros", MuscleGroup = "Bíceps", DefaultSets = 3, DefaultReps = 12, DayNumber = 3 },

            new() { Name = "Crunches", Description = "Acostado, eleva los hombros del suelo contrayendo abdominales", MuscleGroup = "Abdomen", DefaultSets = 3, DefaultReps = 15, DayNumber = 4 },
            new() { Name = "Elevación de piernas", Description = "Acostado, levanta las piernas manteniendo rectas", MuscleGroup = "Abdomen", DefaultSets = 3, DefaultReps = 12, DayNumber = 4 },
            new() { Name = "Plancha", Description = "Apoyado en antebrazos y puntas de pies, mantén el cuerpo recto", MuscleGroup = "Core", DefaultSets = 3, DefaultReps = 30, DayNumber = 4 },
            new() { Name = "Crunches oblícuos", Description = "Eleva el torso rotando hacia un lado", MuscleGroup = "Oblícuos", DefaultSets = 3, DefaultReps = 12, DayNumber = 4 },
            new() { Name = "Saltos en estrella", Description = "Saltos con brazos y piernas extendidos en cruz", MuscleGroup = "Cardio", DefaultSets = 3, DefaultReps = 10, DayNumber = 4 },
        };
    }

    public static List<(int Day, string Name, string Description)> GetRoutineSchedule()
    {
        return new List<(int, string, string)>
        {
            (1, "Pierna y Glúteos", "Fortalecimiento de tren inferior"),
            (2, "Pecho y Tríceps", "Empuje y tren superior"),
            (3, "Espalda y Bíceps", "Tracción y espalda"),
            (4, "Cardio y Core", "Abdomen y resistencia"),
            (5, "Descanso Activo", "Caminata ligera o estiramientos"),
            (6, "Pierna y Glúteos", "Fortalecimiento de tren inferior"),
            (7, "Descanso", "Recuperación total"),
        };
    }
}
