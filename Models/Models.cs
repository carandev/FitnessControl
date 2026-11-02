namespace FitControl.Models;

public class Ejercicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string Instrucciones { get; set; } = "";
    public string VideoUrl { get; set; } = "";
    public string Categoria { get; set; } = "";
    public string GrupoMuscular { get; set; } = "";
    public string Dificultad { get; set; } = "Principiante";
    public int DuracionSegundos { get; set; }
    public int Repeticiones { get; set; }
    public bool EsPorTiempo => DuracionSegundos > 0 && Repeticiones == 0;
    public bool EsPorSeries => Repeticiones > 0;
}

public class RutinaDiaria
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public string Objetivo { get; set; } = "";
    public int DuracionMinutos { get; set; }
    public List<RutinaEjercicio> Ejercicios { get; set; } = new();
}

public class RutinaEjercicio
{
    public int EjercicioId { get; set; }
    public int Orden { get; set; }
    public int Series { get; set; } = 1;
    public int Repeticiones { get; set; }
    public int DuracionSegundos { get; set; }
    public int DescansoSegundos { get; set; } = 30;
    public bool EsPorTiempo => DuracionSegundos > 0 && Repeticiones == 0;
    public bool EsPorSeries => Repeticiones > 0;
}

public class Sesion
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string? RutinaNombre { get; set; }
    public TimeSpan Duracion { get; set; }
    public string Sensacion { get; set; } = "Moderado";
    public bool Completada { get; set; }
}

public class RegistroPeso
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public double PesoKg { get; set; }
}

public class UserProfile
{
    public string Nombre { get; set; } = "";
    public double AlturaCm { get; set; } = 177;
    public double PesoInicialKg { get; set; } = 87;
    public string Objetivo { get; set; } = "Bajar peso y tonificar";
}
