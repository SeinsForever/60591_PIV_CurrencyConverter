using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flurl.Http;

namespace CurrencyConverter;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        GetCurrencyInfoFromApi();
    }

    [RelayCommand]
    void GetCurrencyInfoFromApi()
    {
        listOfCurrency.Clear();
        listOfValue.Clear();
        listOfCurrency.Add("Российский рубль");
        ListOfValue.Add(1);

        while (true)
        {
            var url = $"https://www.cbr-xml-daily.ru/archive/" +
                      $"{userDateTime.Year}/{userDateTime.Month:D2}" +
                      $"/{userDateTime.Day:D2}" +
                      $"/daily_json.js";
            try
            {
                var result = url.GetJsonAsync<Root>().Result;
                findableDateTime = result.Date;

                foreach (var valutes in result.Valute.ListOfValuteItem)
                {
                    listOfCurrency.Add(valutes.Name);
                    listOfValue.Add(valutes.Value / valutes.Nominal);
                }

                break;
            }
            catch
            {
                userDateTime = userDateTime.AddDays(-1);
            }
        }


        OnPropertyChanged(nameof(UserDateTime));
        OnPropertyChanged(nameof(FindableDateTime));
        OnPropertyChanged(nameof(rateValueRubIn));
        OnPropertyChanged(nameof(rateValueRubOut));
        OnPropertyChanged(nameof(rateValue));
        OnPropertyChanged(nameof(convertedCurrencyValue));
    }

    [ObservableProperty]
    public List<String> listOfCurrency = new List<String>();

    [ObservableProperty]
    public List<double> listOfValue = new List<double>();



    [ObservableProperty]
    DateTime todayDate = DateTime.Now;

    [ObservableProperty]
    DateTime userDateTime = DateTime.Now;

    [ObservableProperty]
    DateTime findableDateTime;


    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(convertedCurrencyValue))]
    double userCurrencyValue = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(rateValueRubIn))]
    [NotifyPropertyChangedFor(nameof(nameOfCurrencyIn))]
    [NotifyPropertyChangedFor(nameof(convertedCurrencyValue))]
    [NotifyPropertyChangedFor(nameof(rateValue))]
    int selectedCurrencyIn = 0;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(rateValueRubOut))]
    [NotifyPropertyChangedFor(nameof(nameOfCurrencyOut))]
    [NotifyPropertyChangedFor(nameof(convertedCurrencyValue))]
    [NotifyPropertyChangedFor(nameof(rateValue))]
    int selectedCurrencyOut = 11;

    public double rateValueRubIn => listOfValue[selectedCurrencyIn];
    public double rateValueRubOut => listOfValue[selectedCurrencyOut];
    public string nameOfCurrencyIn => listOfCurrency[selectedCurrencyIn];
    public string nameOfCurrencyOut => listOfCurrency[selectedCurrencyOut];

    public double convertedCurrencyValue => userCurrencyValue * rateValueRubIn / rateValueRubOut;
    public double rateValue => rateValueRubIn / rateValueRubOut;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    public bool IsNotBusy => !IsBusy;

    [RelayCommand]
    void Change()
    {
        IsBusy = true;
        (selectedCurrencyIn, selectedCurrencyOut) = (selectedCurrencyOut, selectedCurrencyIn);
        OnPropertyChanged(nameof(rateValueRubIn));
        OnPropertyChanged(nameof(rateValueRubOut));
        OnPropertyChanged(nameof(nameOfCurrencyIn));
        OnPropertyChanged(nameof(nameOfCurrencyOut));
        OnPropertyChanged(nameof(rateValue));
        OnPropertyChanged(nameof(convertedCurrencyValue));
        OnPropertyChanged(nameof(SelectedCurrencyIn));
        OnPropertyChanged(nameof(SelectedCurrencyOut));
        IsBusy = false;
    }


}