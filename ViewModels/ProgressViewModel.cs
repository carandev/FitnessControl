using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FitnessControl.Models;
using FitnessControl.Services;
using System.Collections.ObjectModel;

namespace FitnessControl.ViewModels;

public partial class ProgressViewModel : ObservableObject
{
    [ObservableProperty]
    private User? _user;

    [ObservableProperty]
    private double _bmi;

    [ObservableProperty]
    private string _bmiCategory = string.Empty;

    [ObservableProperty]
    private bool _isLoading = true;

    public ObservableCollection<BodyProgress> ProgressRecords { get; } = new();

    public ProgressViewModel()
    {
        LoadDataAsync();
    }

    private async void LoadDataAsync()
    {
        try
        {
            await DatabaseService.InitializeAsync();

            User = await DatabaseService.GetUserAsync();
            if (User != null)
            {
                CalculateBMI();
            }

            var records = await DatabaseService.GetBodyProgressAsync();
            ProgressRecords.Clear();
            foreach (var record in records)
            {
                ProgressRecords.Add(record);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void CalculateBMI()
    {
        if (User == null || User.Height <= 0) return;

        double heightInMeters = User.Height / 100;
        Bmi = Math.Round(User.Weight / (heightInMeters * heightInMeters), 1);

        BmiCategory = Bmi switch
        {
            < 18.5 => "Bajo peso",
            < 25 => "Normal",
            < 30 => "Sobrepeso",
            _ => "Obesidad"
        };
    }

    [RelayCommand]
    private async Task AddProgress(double weight)
    {
        var progress = new BodyProgress
        {
            Weight = weight,
            Date = DateTime.Now
        };

        await DatabaseService.AddBodyProgressAsync(progress);

        if (User != null)
        {
            User.Weight = weight;
            await DatabaseService.UpdateUserAsync(User);
            CalculateBMI();
        }

        ProgressRecords.Insert(0, progress);
    }

    public async Task SaveMeasurementsAsync(
        double? chest, double? waist, double? hips,
        double? leftArm, double? rightArm, double? leftThigh, double? rightThigh)
    {
        var progress = new BodyProgress
        {
            Weight = User?.Weight ?? 0,
            Chest = chest,
            Waist = waist,
            Hips = hips,
            LeftArm = leftArm,
            RightArm = rightArm,
            LeftThigh = leftThigh,
            RightThigh = rightThigh,
            Date = DateTime.Now
        };

        await DatabaseService.AddBodyProgressAsync(progress);
        ProgressRecords.Insert(0, progress);
    }
}
