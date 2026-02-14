using FitnessControl.Services;
using FitnessControl.Models;

namespace FitnessControl;

public partial class HistoryPage : ContentPage
{
    public HistoryPage()
    {
        InitializeComponent();
        LoadSessions();
    }

    private async void LoadSessions()
    {
        await DatabaseService.InitializeAsync();
        var sessions = await DatabaseService.GetAllSessionsAsync();
        
        SessionsList.Children.Clear();
        
        if (sessions.Count == 0)
        {
            SessionsList.Add(new Label
            {
                Text = "No hay entrenamientos registrados",
                FontSize = 16,
                TextColor = Color.FromArgb("#636E72"),
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(0, 40, 0, 0)
            });
            return;
        }

        foreach (var session in sessions)
        {
            var bgColor = session.IsCompleted ? "#A8E6CF" : "#FFEAA7";
            
            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 12,
                Padding = 16,
                Margin = new Thickness(0, 0, 0, 8)
            };

            var stack = new VerticalStackLayout();
            
            var headerGrid = new Grid();
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            var nameLabel = new Label
            {
                Text = session.RoutineName,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2D3436")
            };
            Grid.SetColumn(nameLabel, 0);

            var statusLabel = new Label
            {
                Text = session.IsCompleted ? "Completado" : "Iniciado",
                FontSize = 12,
                TextColor = session.IsCompleted ? Color.FromArgb("#27AE60") : Color.FromArgb("#F39C12")
            };
            Grid.SetColumn(statusLabel, 1);

            headerGrid.Children.Add(nameLabel);
            headerGrid.Children.Add(statusLabel);

            stack.Children.Add(headerGrid);
            stack.Children.Add(new Label
            {
                Text = session.Date.ToString("dd/MM/yyyy - HH:mm"),
                FontSize = 12,
                TextColor = Color.FromArgb("#636E72"),
                Margin = new Thickness(0, 4, 0, 0)
            });

            if (session.IsCompleted && session.Duration.TotalMinutes > 0)
            {
                stack.Children.Add(new Label
                {
                    Text = $"Duración: {(int)session.Duration.TotalMinutes} min",
                    FontSize = 12,
                    TextColor = Color.FromArgb("#636E72")
                });
            }

            frame.Content = stack;
            SessionsList.Add(frame);
        }
    }
}
