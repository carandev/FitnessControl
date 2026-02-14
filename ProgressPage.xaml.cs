using FitnessControl.Services;
using FitnessControl.Models;

namespace FitnessControl;

public partial class ProgressPage : ContentPage
{
    private User? _user;

    public ProgressPage()
    {
        InitializeComponent();
        LoadData();
    }

    private async void LoadData()
    {
        await DatabaseService.InitializeAsync();
        _user = await DatabaseService.GetUserAsync();

        if (_user != null)
        {
            UpdateBMI();
            await LoadHistory();
        }
    }

    private void UpdateBMI()
    {
        if (_user == null || _user.Height <= 0) return;

        double heightInMeters = _user.Height / 100;
        double bmi = Math.Round(_user.Weight / (heightInMeters * heightInMeters), 1);

        BMILabel.Text = bmi.ToString();
        WeightLabel.Text = $"{_user.Weight} kg";

        BMICategory.Text = bmi switch
        {
            < 18.5 => "Bajo peso",
            < 25 => "Normal",
            < 30 => "Sobrepeso",
            _ => "Obesidad"
        };
    }

    private async Task LoadHistory()
    {
        var records = await DatabaseService.GetBodyProgressAsync();
        
        WeightHistoryList.Children.Clear();
        foreach (var record in records.Take(10))
        {
            var frame = new Frame
            {
                BackgroundColor = Colors.White,
                CornerRadius = 12,
                Padding = 12,
                Margin = new Thickness(0, 0, 0, 8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            var dateLabel = new Label
            {
                Text = record.Date.ToString("dd/MM/yyyy"),
                FontSize = 14,
                TextColor = Color.FromArgb("#636E72")
            };
            Grid.SetColumn(dateLabel, 0);

            var weightLabel = new Label
            {
                Text = $"{record.Weight} kg",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#2D3436")
            };
            Grid.SetColumn(weightLabel, 1);

            grid.Children.Add(dateLabel);
            grid.Children.Add(weightLabel);
            frame.Content = grid;

            WeightHistoryList.Add(frame);
        }
    }

    private async void SaveWeight_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(WeightEntry.Text)) return;
        if (!double.TryParse(WeightEntry.Text, out double weight)) return;

        var progress = new BodyProgress
        {
            Weight = weight,
            Date = DateTime.Now
        };

        await DatabaseService.AddBodyProgressAsync(progress);

        if (_user != null)
        {
            _user.Weight = weight;
            await DatabaseService.UpdateUserAsync(_user);
            UpdateBMI();
        }

        WeightEntry.Text = "";
        await LoadHistory();
        
        await DisplayAlert("Guardado", "Peso registrado correctamente", "OK");
    }
}
