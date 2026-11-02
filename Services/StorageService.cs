using System.Text.Json;
using FitControl.Models;

namespace FitControl.Services;

public class StorageService
{
    private readonly string _dataPath;
    private AppData _data = new();

    public StorageService()
    {
        _dataPath = Path.Combine(FileSystem.AppDataDirectory, "fitcontrol_data.json");
        Load();
    }

    private void Load()
    {
        try
        {
            if (File.Exists(_dataPath))
            {
                var json = File.ReadAllText(_dataPath);
                _data = JsonSerializer.Deserialize<AppData>(json) ?? new AppData();
                EnsureEjerciciosLoaded();
            }
            else
            {
                InicializarDatos();
            }
        }
        catch
        {
            InicializarDatos();
        }
    }

    private void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_dataPath, json);
        }
        catch { }
    }

    private void InicializarDatos()
    {
        _data.Ejercicios = GetEjerciciosCompletos();
        _data.Pesos = new List<RegistroPeso> { new() { Id = 1, Fecha = DateTime.Today, PesoKg = 88 } };
        _data.Perfil = new UserProfile { AlturaCm = 177, PesoInicialKg = 88 };
        Save();
    }

    private void EnsureEjerciciosLoaded()
    {
        if (_data.Ejercicios == null || _data.Ejercicios.Count == 0 || string.IsNullOrEmpty(_data.Ejercicios[0].Instrucciones))
        {
            var ejerciciosCompletos = GetEjerciciosCompletos();
            foreach (var ej in ejerciciosCompletos)
            {
                var existing = _data.Ejercicios.FirstOrDefault(e => e.Id == ej.Id);
                if (existing != null)
                {
                    existing.Nombre = ej.Nombre;
                    existing.Descripcion = ej.Descripcion;
                    existing.Instrucciones = ej.Instrucciones;
                    existing.VideoUrl = ej.VideoUrl;
                    existing.Categoria = ej.Categoria;
                    existing.GrupoMuscular = ej.GrupoMuscular;
                    existing.Dificultad = ej.Dificultad;
                    existing.DuracionSegundos = ej.DuracionSegundos;
                    existing.Repeticiones = ej.Repeticiones;
                }
            }
            Save();
        }
    }

    public List<Ejercicio> GetEjercicios() => _data.Ejercicios;
    public Ejercicio? GetEjercicioById(int id) => _data.Ejercicios.FirstOrDefault(e => e.Id == id);
    public List<Sesion> GetSesiones() => _data.Sesiones;
    public List<RegistroPeso> GetPesos() => _data.Pesos;
    public UserProfile GetPerfil() => _data.Perfil;

    public RutinaDiaria GetRutinaDelDia()
    {
        var diaSemana = (int)DateTime.Today.DayOfWeek;
        return GetRutinaSemanal(diaSemana);
    }

    private RutinaDiaria GetRutinaSemanal(int diaSemana)
    {
        return diaSemana switch
        {
            0 => GetRutinaDomingo(),
            1 => GetRutinaLunes(),
            2 => GetRutinaMartes(),
            3 => GetRutinaMiercoles(),
            4 => GetRutinaJueves(),
            5 => GetRutinaViernes(),
            6 => GetRutinaSabado(),
            _ => GetRutinaLunes()
        };
    }

    public string GetNombreDiaSemana()
    {
        return DateTime.Today.DayOfWeek switch
        {
            DayOfWeek.Monday => "Lunes",
            DayOfWeek.Tuesday => "Martes",
            DayOfWeek.Wednesday => "Miércoles",
            DayOfWeek.Thursday => "Jueves",
            DayOfWeek.Friday => "Viernes",
            DayOfWeek.Saturday => "Sábado",
            DayOfWeek.Sunday => "Domingo",
            _ => "Lunes"
        };
    }

    private RutinaDiaria GetRutinaLunes()
    {
        return new RutinaDiaria
        {
            Id = 1,
            Nombre = "Lunes - Fuerza Básica (Full Body)",
            Descripcion = "Activa todo el cuerpo sin agotarte",
            Objetivo = "Fuerza",
            DuracionMinutos = 30,
            Ejercicios = new List<RutinaEjercicio>
            {
                new() { EjercicioId = 100, Orden = 1, DuracionSegundos = 300, DescansoSegundos = 0 },
                new() { EjercicioId = 1, Orden = 2, Series = 3, Repeticiones = 12, DescansoSegundos = 60 },
                new() { EjercicioId = 2, Orden = 3, Series = 3, Repeticiones = 8, DescansoSegundos = 60 },
                new() { EjercicioId = 6, Orden = 4, Series = 3, Repeticiones = 10, DescansoSegundos = 60 },
                new() { EjercicioId = 16, Orden = 5, Series = 3, Repeticiones = 12, DescansoSegundos = 60 },
                new() { EjercicioId = 17, Orden = 6, Series = 3, DuracionSegundos = 20, DescansoSegundos = 45 },
                new() { EjercicioId = 12, Orden = 7, Series = 2, DuracionSegundos = 30, DescansoSegundos = 0 }
            }
        };
    }

    private RutinaDiaria GetRutinaMartes()
    {
        return new RutinaDiaria
        {
            Id = 2,
            Nombre = "Martes - Cardio Moderado",
            Descripcion = "Cardio suave para proteger rodillas",
            Objetivo = "Cardio",
            DuracionMinutos = 30,
            Ejercicios = new List<RutinaEjercicio>
            {
                new() { EjercicioId = 100, Orden = 1, DuracionSegundos = 600, DescansoSegundos = 0 },
                new() { EjercicioId = 13, Orden = 2, Series = 3, Repeticiones = 20, DescansoSegundos = 45 },
                new() { EjercicioId = 5, Orden = 3, Series = 3, Repeticiones = 15, DescansoSegundos = 45 },
                new() { EjercicioId = 19, Orden = 4, Series = 3, Repeticiones = 12, DescansoSegundos = 45 },
                new() { EjercicioId = 100, Orden = 5, DuracionSegundos = 600, DescansoSegundos = 0 }
            }
        };
    }

    private RutinaDiaria GetRutinaMiercoles()
    {
        return new RutinaDiaria
        {
            Id = 3,
            Nombre = "Miércoles - Core y Movilidad",
            Descripcion = "Fortalece abdomen y mejora postura",
            Objetivo = "Core",
            DuracionMinutos = 25,
            Ejercicios = new List<RutinaEjercicio>
            {
                new() { EjercicioId = 13, Orden = 1, Series = 2, Repeticiones = 15, DescansoSegundos = 30 },
                new() { EjercicioId = 7, Orden = 2, Series = 3, Repeticiones = 15, DescansoSegundos = 45 },
                new() { EjercicioId = 10, Orden = 3, Series = 3, Repeticiones = 20, DescansoSegundos = 45 },
                new() { EjercicioId = 9, Orden = 4, Series = 3, Repeticiones = 12, DescansoSegundos = 45 },
                new() { EjercicioId = 17, Orden = 5, Series = 3, DuracionSegundos = 25, DescansoSegundos = 45 },
                new() { EjercicioId = 16, Orden = 6, Series = 3, Repeticiones = 10, DescansoSegundos = 45 },
                new() { EjercicioId = 12, Orden = 7, Series = 2, DuracionSegundos = 30, DescansoSegundos = 0 }
            }
        };
    }

    private RutinaDiaria GetRutinaJueves()
    {
        return new RutinaDiaria
        {
            Id = 4,
            Nombre = "Jueves - Descanso Activo",
            Descripcion = "Camina o estira, el cuerpo necesita recuperar",
            Objetivo = "Recuperación",
            DuracionMinutos = 30,
            Ejercicios = new List<RutinaEjercicio>
            {
                new() { EjercicioId = 100, Orden = 1, DuracionSegundos = 1800, DescansoSegundos = 0 }
            }
        };
    }

    private RutinaDiaria GetRutinaViernes()
    {
        return new RutinaDiaria
        {
            Id = 5,
            Nombre = "Viernes - Fuerza (Piernas + Torso)",
            Descripcion = "Segunda sesión de fuerza de la semana",
            Objetivo = "Fuerza",
            DuracionMinutos = 30,
            Ejercicios = new List<RutinaEjercicio>
            {
                new() { EjercicioId = 100, Orden = 1, DuracionSegundos = 300, DescansoSegundos = 0 },
                new() { EjercicioId = 1, Orden = 2, Series = 4, Repeticiones = 12, DescansoSegundos = 60 },
                new() { EjercicioId = 6, Orden = 3, Series = 3, Repeticiones = 10, DescansoSegundos = 60 },
                new() { EjercicioId = 2, Orden = 4, Series = 3, Repeticiones = 10, DescansoSegundos = 60 },
                new() { EjercicioId = 16, Orden = 5, Series = 3, Repeticiones = 12, DescansoSegundos = 60 },
                new() { EjercicioId = 17, Orden = 6, Series = 3, DuracionSegundos = 30, DescansoSegundos = 45 },
                new() { EjercicioId = 12, Orden = 7, Series = 2, DuracionSegundos = 30, DescansoSegundos = 0 }
            }
        };
    }

    private RutinaDiaria GetRutinaSabado()
    {
        return new RutinaDiaria
        {
            Id = 6,
            Nombre = "Sábado - Cardio Largo",
            Descripcion = "El día clave para quemar grasa",
            Objetivo = "Cardio",
            DuracionMinutos = 40,
            Ejercicios = new List<RutinaEjercicio>
            {
                new() { EjercicioId = 100, Orden = 1, DuracionSegundos = 300, DescansoSegundos = 0 },
                new() { EjercicioId = 100, Orden = 2, DuracionSegundos = 2100, DescansoSegundos = 0 },
                new() { EjercicioId = 12, Orden = 3, Series = 2, DuracionSegundos = 60, DescansoSegundos = 0 }
            }
        };
    }

    private RutinaDiaria GetRutinaDomingo()
    {
        return new RutinaDiaria
        {
            Id = 7,
            Nombre = "Domingo - Recuperación",
            Descripcion = "Estiramientos y movilidad",
            Objetivo = "Flexibilidad",
            DuracionMinutos = 20,
            Ejercicios = new List<RutinaEjercicio>
            {
                new() { EjercicioId = 12, Orden = 1, DuracionSegundos = 60, DescansoSegundos = 15 },
                new() { EjercicioId = 14, Orden = 2, DuracionSegundos = 60, DescansoSegundos = 15 },
                new() { EjercicioId = 15, Orden = 3, DuracionSegundos = 60, DescansoSegundos = 15 },
                new() { EjercicioId = 11, Orden = 4, DuracionSegundos = 60, DescansoSegundos = 15 },
                new() { EjercicioId = 100, Orden = 5, DuracionSegundos = 600, DescansoSegundos = 0 }
            }
        };
    }

    public void AddSesion(Sesion sesion)
    {
        sesion.Id = _data.Sesiones.Count > 0 ? _data.Sesiones.Max(s => s.Id) + 1 : 1;
        _data.Sesiones.Add(sesion);
        Save();
    }

    public void AddPeso(RegistroPeso peso)
    {
        peso.Id = _data.Pesos.Count > 0 ? _data.Pesos.Max(p => p.Id) + 1 : 1;
        _data.Pesos.Add(peso);
        Save();
    }

    public void UpdatePerfil(UserProfile perfil)
    {
        _data.Perfil = perfil;
        Save();
    }

    public int GetRacha()
    {
        var sesiones = _data.Sesiones.Where(s => s.Completada).OrderByDescending(s => s.Fecha).ToList();
        if (sesiones.Count == 0) return 0;
        int racha = 0;
        var fecha = DateTime.Today;
        foreach (var s in sesiones)
        {
            var fs = s.Fecha.Date;
            if (fs == fecha || fs == fecha.AddDays(-1)) { racha++; fecha = fs.AddDays(-1); }
            else break;
        }
        return racha;
    }

    public bool YaHizoEjercicioHoy()
    {
        return _data.Sesiones.Any(s => s.Completada && s.Fecha.Date == DateTime.Today);
    }

    private List<Ejercicio> GetEjerciciosCompletos()
    {
        return new List<Ejercicio>
        {
            new() { Id = 1, Nombre = "Sentadillas", Descripcion = "Flexión de rodillas desde pie", Categoria = "Fuerza", GrupoMuscular = "Piernas", Dificultad = "Principiante", Repeticiones = 12, 
                VideoUrl = "https://www.youtube.com/watch?v=KjcNUJ6Yt0g",
                Instrucciones = "1. Párate con pies al ancho de los hombros\n2. Baja como si te sentaras en una silla invisible\n3. Mantén tu peso en los talones\n4. Baja hasta que tus muslos estén paralelos al suelo\n5. Vuelve a subir apretando los glúteos" },

            new() { Id = 2, Nombre = "Flexiones de Rodillas", Descripcion = "Flexiones con apoyo en rodillas", Categoria = "Fuerza", GrupoMuscular = "Pecho", Dificultad = "Principiante", Repeticiones = 8,
                VideoUrl = "https://www.youtube.com/shorts/CiGIDQB4UWc",
                Instrucciones = "1. Colócate en el suelo boca abajo\n2. Apóyate en las rodillas y las manos\n3. Mantén el cuerpo recto\n4. Baja el pecho hacia el suelo\n5. Empuja hacia arriba" },

            new() { Id = 6, Nombre = "Zancadas Alternadas", Descripcion = "Adelantar una pierna alternando", Categoria = "Fuerza", GrupoMuscular = "Piernas", Dificultad = "Principiante", Repeticiones = 10,
                VideoUrl = "https://www.youtube.com/watch?v=cCp7C9dOa3Y",
                Instrucciones = "1. Párate derecho\n2. Da un paso adelante con una pierna\n3. Baja hasta que ambas rodillas estén a 90 grados\n4. Regresa y repite con la otra pierna" },

            new() { Id = 16, Nombre = "Superman", Descripcion = "Elevar brazos y piernas desde el suelo", Categoria = "Core", GrupoMuscular = "Espalda baja", Dificultad = "Principiante", Repeticiones = 12,
                VideoUrl = "https://www.youtube.com/watch?v=vxaPBQdVaQg",
                Instrucciones = "1. Acuéstate boca abajo con brazos extendidos\n2. Simultáneamente levanta brazos, pecho y piernas\n3. Mantén 2 segundos\n4. Baja lentamente y repite" },

            new() { Id = 17, Nombre = "Plancha Frontal", Descripcion = "Mantén el cuerpo recto sobre los codos", Categoria = "Fuerza", GrupoMuscular = "Core", Dificultad = "Principiante", DuracionSegundos = 30,
                VideoUrl = "https://www.youtube.com/watch?v=pSHjTRCQxIw",
                Instrucciones = "1. Apóyate sobre los codos y las puntas de los pies\n2. Mantén tu cuerpo en línea recta\n3. Aprieta el abdomen y los glúteos\n4. No dejes que la cadera suba o baje" },

            new() { Id = 12, Nombre = "Estiramiento de Piernas", Descripcion = "Estirar isquiotibiales", Categoria = "Estiramiento", GrupoMuscular = "Piernas", Dificultad = "Principiante", DuracionSegundos = 30,
                VideoUrl = "https://www.youtube.com/watch?v=ID9R2BrInRo",
                Instrucciones = "1. Siéntate con piernas extendidas\n2. Dobla una rodilla y apoya el pie contra el muslo\n3. Inclínate hacia adelante intentando tocar los toes\n4. Mantén 15 segundos y repite" },

            new() { Id = 13, Nombre = "Jumping Jacks", Descripcion = "Salto con apertura de brazos y piernas", Categoria = "Cardio", GrupoMuscular = "Cuerpo completo", Dificultad = "Principiante", Repeticiones = 20,
                VideoUrl = "https://www.youtube.com/watch?v=Omk6XKk-BZg",
                Instrucciones = "1. Párate con pies juntos y brazos a los lados\n2. Salta separando piernas mientras levantas brazos\n3. Vuelve a la posición inicial\n4. Mantén el ritmo constante" },

            new() { Id = 5, Nombre = "Mountain Climbers", Descripcion = "Escaladores - lleva rodillas al pecho", Categoria = "Cardio", GrupoMuscular = "Cuerpo completo", Dificultad = "Principiante", Repeticiones = 15,
                VideoUrl = "https://www.youtube.com/watch?v=A-3ouxT4mHA",
                Instrucciones = "1. Colócate en posición de plancha alta\n2. Lleva una rodilla hacia el pecho\n3. Alterna las piernas rápidamente\n4. Mantén las caderas niveladas" },

            new() { Id = 19, Nombre = "Skater Step", Descripcion = "Saltos laterales estilo patinador", Categoria = "Cardio", GrupoMuscular = "Piernas", Dificultad = "Principiante", Repeticiones = 12,
                VideoUrl = "https://www.youtube.com/watch?v=QlZqRKEV1i8",
                Instrucciones = "1. Párate sobre una pierna\n2. Salta lateralmente hacia un lado\n3. Cae sobre la otra pierna\n4. Alterna lateralmente" },

            new() { Id = 7, Nombre = "Crunches", Descripcion = "Elevación de torso desde el suelo", Categoria = "Core", GrupoMuscular = "Abdomen", Dificultad = "Principiante", Repeticiones = 15,
                VideoUrl = "https://www.youtube.com/watch?v=N2qDD8fHr98",
                Instrucciones = "1. Acuéstate boca arriba con rodillas flexionadas\n2. Coloca las manos detrás de la cabeza\n3. Eleva los hombros usando solo abdominales\n4. Baja lentamente sin tocar el suelo" },

            new() { Id = 10, Nombre = "Bicicleta Abdominal", Descripcion = "Tocar codo con rodilla alternando", Categoria = "Core", GrupoMuscular = "Oblicuos", Dificultad = "Intermedio", Repeticiones = 20,
                VideoUrl = "https://www.youtube.com/watch?v=ibGRT6byKHI",
                Instrucciones = "1. Acuéstate con manos detrás de la cabeza\n2. Levanta las piernas con rodillas a 90 grados\n3. Lleva el codo derecho hacia la rodilla izquierda\n4. Alterna manteniendo el movimiento continuo" },

            new() { Id = 9, Nombre = "Elevación de Piernas", Descripcion = "Elevar piernas acostado", Categoria = "Core", GrupoMuscular = "Abdomen inferior", Dificultad = "Intermedio", Repeticiones = 12,
                VideoUrl = "https://www.youtube.com/watch?v=iD471ZFsNp0",
                Instrucciones = "1. Acuéstate boca arriba con piernas extendidas\n2. Coloca las manos debajo de los glúteos\n3. Levanta las piernas perpendiculares al suelo\n4. Baja lentamente sin tocar el suelo" },

            new() { Id = 14, Nombre = "Estiramiento de Cadera", Descripcion = "Posición de mariposa", Categoria = "Estiramiento", GrupoMuscular = "Cadera", Dificultad = "Principiante", DuracionSegundos = 30,
                VideoUrl = "https://www.youtube.com/watch?v=VBx2k2mtqiM",
                Instrucciones = "1. Siéntate en el suelo\n2. Une las plantas de los pies\n3. Empuja suavemente las rodillas hacia el suelo\n4. Mantén la espalda recta" },

            new() { Id = 15, Nombre = "Estiramiento de Cuello", Descripcion = "Movilidad de cuello", Categoria = "Estiramiento", GrupoMuscular = "Cuello", Dificultad = "Principiante", DuracionSegundos = 30,
                VideoUrl = "https://www.youtube.com/watch?v=w1rR0a585jU",
                Instrucciones = "1. Siéntate o párate derecho\n2. Inclina la cabeza hacia un lado\n3. Mantén 15 segundos\n4. Repite del otro lado" },

            new() { Id = 11, Nombre = "Estiramiento de Espalda", Descripcion = "Movilidad de columna", Categoria = "Estiramiento", GrupoMuscular = "Espalda", Dificultad = "Principiante", DuracionSegundos = 30,
                VideoUrl = "https://www.youtube.com/watch?v=w1rR0a585jU",
                Instrucciones = "1. Arrodíllate con manos en el suelo\n2. Arquea la espalda hacia arriba\n3. Mantén 5 segundos\n4. Arquea hacia abajo y repite" },

            new() { Id = 100, Nombre = "Caminata", Descripcion = "Caminar rápido o en el lugar", Categoria = "Cardio", GrupoMuscular = "Cuerpo completo", Dificultad = "Principiante", DuracionSegundos = 300,
                VideoUrl = "",
                Instrucciones = "1. Camina a paso ligero\n2. Mantén una postura erguida\n3. Balancea los brazos naturalmente\n4. Respira profundamente" }
        };
    }

    private class AppData
    {
        public List<Ejercicio> Ejercicios { get; set; } = new();
        public List<Sesion> Sesiones { get; set; } = new();
        public List<RegistroPeso> Pesos { get; set; } = new();
        public UserProfile Perfil { get; set; } = new();
    }
}
