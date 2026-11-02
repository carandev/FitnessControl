using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitControl.Models;
using FitControl.Services;
using System.Collections.ObjectModel;

namespace FitControl.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly StorageService _storage;
    private DateTime _inicioSesion;
    private System.Timers.Timer? _timer;
    private System.Timers.Timer? _timerDescanso;
    private bool _enDescanso;
    private int _tiempoRestante;

    [ObservableProperty] private int rachaDias;
    [ObservableProperty] private bool ejercicioCompletadoHoy;
    [ObservableProperty] private double ultimoPeso;
    [ObservableProperty] private double imc;
    [ObservableProperty] private string imcCategoria = "";
    [ObservableProperty] private string nuevoPesoTexto = "";
    [ObservableProperty] private string nombreDiaSemana = "";
    [ObservableProperty] private string nombre = "";
    [ObservableProperty] private double alturaCm = 177;
    [ObservableProperty] private double pesoInicialKg = 87;
    [ObservableProperty] private string objetivo = "Bajar peso y tonificar";

    [ObservableProperty] private bool mostrandoRutina;
    [ObservableProperty] private RutinaDiaria? rutinaDiaria;
    [ObservableProperty] private int ejercicioActualIndex;
    [ObservableProperty] private int serieActual = 1;
    [ObservableProperty] private bool sesionEnCurso;
    [ObservableProperty] private string tiempoTranscurrido = "00:00";
    [ObservableProperty] private string tiempoEjercicio = "";
    [ObservableProperty] private string textoDescanso = "";
    [ObservableProperty] private bool enDescanso;
    [ObservableProperty] private string sensacionSeleccionada = "Moderado";
    [ObservableProperty] private bool rutinaCompletada;

    [ObservableProperty] private string ejercicioNombre = "";
    [ObservableProperty] private string ejercicioDescripcion = "";
    [ObservableProperty] private string ejercicioInstrucciones = "";
    [ObservableProperty] private string ejercicioMeta = "";
    [ObservableProperty] private string ejercicioSerieInfo = "";
    [ObservableProperty] private bool esPorTiempo;
    [ObservableProperty] private bool esPorSeries;

    [ObservableProperty] private bool mostrandoHistorial;
    [ObservableProperty] private bool mostrandoPerfil;
    [ObservableProperty] private ObservableCollection<Sesion> ultimasSesiones = new();

    public MainViewModel(StorageService storage)
    {
        _storage = storage;
        CargarDatos();
    }

    [RelayCommand]
    public void CargarDatos()
    {
        var perfil = _storage.GetPerfil();
        Nombre = perfil.Nombre;
        AlturaCm = perfil.AlturaCm;
        PesoInicialKg = perfil.PesoInicialKg;
        Objetivo = perfil.Objetivo;

        var pesos = _storage.GetPesos();
        UltimoPeso = pesos.OrderByDescending(p => p.Fecha).First().PesoKg;
        NuevoPesoTexto = UltimoPeso.ToString("F1");
        CalcularImc(perfil.AlturaCm);

        RachaDias = _storage.GetRacha();
        EjercicioCompletadoHoy = _storage.YaHizoEjercicioHoy();

        RutinaDiaria = _storage.GetRutinaDelDia();
        NombreDiaSemana = _storage.GetNombreDiaSemana();

        var sesiones = _storage.GetSesiones().Where(s => s.Completada).OrderByDescending(s => s.Fecha).ToList();
        UltimasSesiones.Clear();
        foreach (var s in sesiones.Take(10))
            UltimasSesiones.Add(s);
    }

    private void CalcularImc(double altura)
    {
        if (UltimoPeso > 0 && altura > 0)
        {
            Imc = UltimoPeso / ((altura / 100) * (altura / 100));
            ImcCategoria = Imc switch { < 18.5 => "Bajo peso", < 25 => "Normal", < 30 => "Sobrepeso", _ => "Obesidad" };
        }
    }

    [RelayCommand]
    private void MostrarRutina()
    {
        MostrandoRutina = true;
        MostrandoHistorial = false;
        MostrandoPerfil = false;
    }

    [RelayCommand]
    private void MostrarInicio()
    {
        MostrandoRutina = false;
        MostrandoHistorial = false;
        MostrandoPerfil = false;
        CargarDatos();
    }

    [RelayCommand]
    private void MostrarHistorial()
    {
        MostrandoRutina = false;
        MostrandoHistorial = true;
        MostrandoPerfil = false;
    }

    [RelayCommand]
    private void MostrarPerfilSeccion()
    {
        MostrandoRutina = false;
        MostrandoHistorial = false;
        MostrandoPerfil = true;
    }

    [RelayCommand]
    private void IniciarRutina()
    {
        if (RutinaDiaria == null || RutinaDiaria.Ejercicios.Count == 0) return;
        
        RutinaDiaria = _storage.GetRutinaDelDia();
        if (RutinaDiaria == null || RutinaDiaria.Ejercicios.Count == 0) return;
        
        EjercicioActualIndex = 0;
        SerieActual = 1;
        SesionEnCurso = true;
        RutinaCompletada = false;
        _enDescanso = false;
        _inicioSesion = DateTime.Now;
        
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (s, e) =>
        {
            var diff = DateTime.Now - _inicioSesion;
            TiempoTranscurrido = $"{(int)diff.TotalMinutes:00}:{diff.Seconds:00}";
            
            if (!EnDescanso && EsPorTiempo)
            {
                _tiempoRestante--;
                TiempoEjercicio = _tiempoRestante.ToString();
                if (_tiempoRestante <= 0)
                {
                    App.Current.Dispatcher.Dispatch(() => CompletarEjercicio());
                }
            }
        };
        _timer.Start();
        
        MostrarEjercicioActual();
    }

    private void MostrarEjercicioActual()
    {
        if (RutinaDiaria == null) return;
        
        var rutinaEjercicio = RutinaDiaria.Ejercicios[EjercicioActualIndex];
        var ejercicio = _storage.GetEjercicioById(rutinaEjercicio.EjercicioId);
        
        if (ejercicio == null) return;

        EjercicioNombre = ejercicio.Nombre;
        EjercicioDescripcion = ejercicio.Descripcion;
        EjercicioInstrucciones = ejercicio.Instrucciones;
        EsPorTiempo = rutinaEjercicio.EsPorTiempo;
        EsPorSeries = rutinaEjercicio.EsPorSeries;
        SerieActual = 1;

        if (EsPorTiempo)
        {
            _tiempoRestante = rutinaEjercicio.DuracionSegundos;
            TiempoEjercicio = _tiempoRestante.ToString();
            EjercicioMeta = $"Mantén durante {_tiempoRestante} segundos";
            EjercicioSerieInfo = "";
        }
        else
        {
            TiempoEjercicio = "";
            EjercicioMeta = $"{rutinaEjercicio.Series} series x {rutinaEjercicio.Repeticiones} repeticiones";
            EjercicioSerieInfo = $"Serie {SerieActual} de {rutinaEjercicio.Series}";
        }
    }

    [RelayCommand]
    private void CompletarEjercicio()
    {
        if (RutinaDiaria == null) return;
        
        var rutinaEjercicio = RutinaDiaria.Ejercicios[EjercicioActualIndex];
        
        if (EsPorSeries)
        {
            if (SerieActual < rutinaEjercicio.Series)
            {
                SerieActual++;
                EjercicioSerieInfo = $"Serie {SerieActual} de {rutinaEjercicio.Series}";
                return;
            }
        }
        
        if (EjercicioActualIndex < RutinaDiaria.Ejercicios.Count - 1)
        {
            IniciarDescanso(rutinaEjercicio.DescansoSegundos);
        }
        else
        {
            FinalizarSesion();
        }
    }

    private void IniciarDescanso(int segundos)
    {
        if (segundos <= 0)
        {
            SiguienteEjercicio();
            return;
        }

        EnDescanso = true;
        _tiempoRestante = segundos;
        TextoDescanso = $"Descanso: {_tiempoRestante} segundos";
        
        _timerDescanso?.Stop();
        _timerDescanso = new System.Timers.Timer(1000);
        _timerDescanso.Elapsed += (s, e) =>
        {
            _tiempoRestante--;
            TextoDescanso = $"Descanso: {_tiempoRestante} segundos";
            if (_tiempoRestante <= 0)
            {
                App.Current.Dispatcher.Dispatch(() =>
                {
                    _timerDescanso?.Stop();
                    EnDescanso = false;
                    SiguienteEjercicio();
                });
            }
        };
        _timerDescanso.Start();
    }

    [RelayCommand]
    private void SaltarDescanso()
    {
        _timerDescanso?.Stop();
        EnDescanso = false;
        SiguienteEjercicio();
    }

    [RelayCommand]
    private void SiguienteEjercicio()
    {
        if (RutinaDiaria == null) return;
        
        if (EjercicioActualIndex < RutinaDiaria.Ejercicios.Count - 1)
        {
            EjercicioActualIndex++;
            MostrarEjercicioActual();
        }
        else
        {
            FinalizarSesion();
        }
    }

    [RelayCommand]
    private void FinalizarSesion()
    {
        DetenerTimers();
        
        var duracion = DateTime.Now - _inicioSesion;
        _storage.AddSesion(new Sesion
        {
            Fecha = DateTime.Now,
            RutinaNombre = RutinaDiaria?.Nombre,
            Duracion = duracion,
            Sensacion = SensacionSeleccionada,
            Completada = true
        });

        SesionEnCurso = false;
        RutinaCompletada = true;
    }

    [RelayCommand]
    private void CancelarSesion()
    {
        DetenerTimers();
        SesionEnCurso = false;
        MostrarRutina();
    }

    private void DetenerTimers()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        _timerDescanso?.Stop();
        _timerDescanso?.Dispose();
        _timerDescanso = null;
        EnDescanso = false;
    }

    [RelayCommand]
    private void AgregarPeso()
    {
        if (double.TryParse(NuevoPesoTexto, out double p) && p > 0)
        {
            _storage.AddPeso(new RegistroPeso { Fecha = DateTime.Today, PesoKg = p });
            UltimoPeso = p;
            var perfil = _storage.GetPerfil();
            CalcularImc(perfil.AlturaCm);
        }
    }

    [RelayCommand]
    private void GuardarPerfil()
    {
        _storage.UpdatePerfil(new UserProfile { Nombre = Nombre, AlturaCm = AlturaCm, PesoInicialKg = PesoInicialKg, Objetivo = Objetivo });
    }
}
