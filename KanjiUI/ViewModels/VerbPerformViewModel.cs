using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoushiKatsu;
using KanjiUI.SpecialModels;
using KanjiUI.Utils;
using KBE.Components.Utils;
using KBE.Models;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.System;

namespace KanjiUI.ViewModels;

internal partial class VerbPerformViewModel : MasterDetailViewModel<VerbConjure>
{

    [ObservableProperty]
    private string furigana;

    [ObservableProperty]
    private string input;

    [ObservableProperty]
    private string display;

    private readonly Brush yellow = new SolidColorBrush(Microsoft.UI.Colors.Yellow);
    private readonly Brush green = new SolidColorBrush(Microsoft.UI.Colors.Green);
    private readonly Brush red = new SolidColorBrush(Microsoft.UI.Colors.Red);

    [ObservableProperty]
    private Brush displayColor = new SolidColorBrush(Microsoft.UI.Colors.Yellow);
    public ObservableCollection<VerbResult> VerbResults { get; set; } = [];

    public VerbPerformViewModel()
    {
        VerbConjure.GenerateConjuredList();
        Generate();
    }

    public ICommand SubmitCommand => new AsyncRelayCommand(Submit);
    public ICommand ContinueCommand => new RelayCommand(Generate);


    public void GenerateInfo()
    {
        VerbResults.Clear();

        VerbResult form = new()
        {
            CorrectAnswer = Current.FormType.ToDisplay(),
            VerbType = "Form",
        };

        VerbResult conjure = new()
        {
            CorrectAnswer = Current.ConjureType.ToDisplay(),
            VerbType = "Conjugation",
        };


        VerbResult polite = new()
        {
            CorrectAnswer = Current.IsPolite ? "✓" : "X",
            VerbType = "Polite",
        };

        VerbResult past = new()
        {
            CorrectAnswer = Current.IsPast ? "✓" : "X",
            VerbType = "Past",
        };

        VerbResult negative = new()
        {
            CorrectAnswer = Current.IsNegative ? "✓" : "X",
            VerbType = "Negative",
        };

        VerbResults.Add(form);
        VerbResults.Add(conjure);
        VerbResults.Add(polite);
        VerbResults.Add(past);
        VerbResults.Add(negative);
    }

    public void Generate()
    {
        Current = VerbConjure.ConjuredList.PickRandom().CloneWithoutSubmit();
        Current.Randomized();
        Display = Current.DictionaryForm;
        DisplayColor = yellow;
        Input = string.Empty;
        GenerateInfo();
        OnPropertyChanged(nameof(IsEnable));
        _ = GetFurigana();
    }

    public async Task Submit()
    {
        Current.Submitted = true;
        OnPropertyChanged(nameof(IsEnable));
        await GenerateResult();
    }

    public async Task GenerateResult() 
    {
        Display = Current.Content;
        await GetFurigana();

        if (Input == Display || Input == Furigana)
        {
            DisplayColor = green;
        }
        else
        {
            DisplayColor = red;
        }
        
    }

    public bool IsEnable()
    {
        return !Current.Submitted;
    }

    public async Task GetFurigana()
    {
        Furigana = await FuriganaHelpers.ToFuriganaKata(Display);
    }

    public override bool ApplyFilter(VerbConjure item, string filter)
    {
        throw new NotImplementedException();
    }
}
