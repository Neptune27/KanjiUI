using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using DoushiKatsu;
using KanjiUI.SpecialModels;
using KBE.Components.Utils;
using KBE.Models;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KanjiUI.ViewModels;

internal partial class VerbRecognizeViewModel : MasterDetailViewModel<VerbConjure>
{

    [ObservableProperty]
    private VerbConjure showVerb;

    public ObservableCollection<VerbResult> VerbResults { get; set; } = [];

    public VerbRecognizeViewModel()
    {
        VerbConjure.GenerateConjuredList();
        Generate();
    }

    public bool IsFormChecked(string option)
    {
        return Current.FormType.ToString() == option;
    }

    public bool IsConjureChecked(string option)
    {
        return Current.ConjureType.ToString() == option;
    }

    public ICommand SelectFormCommand => new RelayCommand<string>(SelectForm);
    public ICommand SubmitCommand => new RelayCommand(Submit);
    public ICommand ContinueCommand => new RelayCommand(Generate);
    public ICommand SelectConjureCommand => new RelayCommand<string>(SelectConjure);

    //public Command SelectOptionCommand => new RelayCommand<string>(SelectForm);

    public void Submit()
    {
        Current.Submitted = true;
        OnPropertyChanged(nameof(IsEnable));
        GenerateResult();
    }

    public void GenerateResult()
    {
        VerbResults.Clear();

        VerbResult verb = new()
        {
            CorrectAnswer = ShowVerb.Content,
            Answer = Current.Content,
            VerbType = "Verb",
        };
        verb.AutoColor();


        VerbResult form = new()
        {
            CorrectAnswer = ShowVerb.FormType.ToDisplay(),
            Answer = Current.FormType.ToDisplay(),
            VerbType = "Form",
        };
        form.AutoColor();

        VerbResult conjure = new()
        {
            CorrectAnswer = ShowVerb.ConjureType.ToDisplay(),
            Answer = Current.ConjureType.ToDisplay(),
            VerbType = "Conjugation",
        };
        conjure.AutoColor();


        VerbResult polite = new()
        {
            CorrectAnswer = ShowVerb.IsPolite ? "✓" : "X",
            Answer = Current.IsPolite ? "✓" : "X",
            VerbType = "Polite",
        };
        polite.AutoColor();

        VerbResult past = new()
        {
            CorrectAnswer = ShowVerb.IsPast ? "✓" : "X",
            Answer = Current.IsPast ? "✓" : "X",
            VerbType = "Past",
        };
        past.AutoColor();

        VerbResult negative = new()
        {
            CorrectAnswer = ShowVerb.IsNegative ? "✓" : "X",
            Answer = Current.IsNegative ? "✓" : "X",
            VerbType = "Negative",
        };
        negative.AutoColor();

        VerbResults.Add(verb);
        VerbResults.Add(form);
        VerbResults.Add(conjure);
        VerbResults.Add(polite);
        VerbResults.Add(past);
        VerbResults.Add(negative);

    }

    public void Generate()
    {
        Current = VerbConjure.ConjuredList.PickRandom().CloneWithoutSubmit();
        ShowVerb = Current.CloneWithoutSubmit();
        ShowVerb.Randomized();
        RequestUpdate();
        OnPropertyChanged(nameof(IsEnable));

    }



    public void SelectForm(string option)
    {
        if (Enum.TryParse(option, out FormType result))
        {
            Current.FormType = result;
        }
        RequestUpdate();
    }

    public void SelectConjure(string option)
    {
        if (Enum.TryParse(option, out ConjureType result))
        {
            Current.ConjureType = result;
        }

        RequestUpdate();
    }

    public bool IsEnable()
    {
        return !Current.Submitted;
    }

    // Method to manually trigger a refresh
    private void RequestUpdate()
    {
        OnPropertyChanged(nameof(IsFormChecked));  // Refresh the function-bound property
        OnPropertyChanged(nameof(IsConjureChecked));  // Refresh the function-bound property
    }
    public override bool ApplyFilter(VerbConjure item, string filter)
    {
        throw new NotImplementedException();
    }
}
