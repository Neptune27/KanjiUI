using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.EMMA;
using KBE.Components.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Models;


public partial class ConjureSettingModel : ObservableObject
{
    private static readonly Setting setting = Setting.Instance;

    [ObservableProperty]
    private bool none = setting.VerbConjureSetting.Conjure.None;
    partial void OnNoneChanged(bool value)
    {
        None = value;
        setting.VerbConjureSetting.Conjure.None = value;
    }

    [ObservableProperty]
    private bool ba = setting.VerbConjureSetting.Conjure.Ba;
    partial void OnBaChanged(bool value)
    {
        Ba = value;
        setting.VerbConjureSetting.Conjure.Ba = value;
    }

    [ObservableProperty]
    private bool tara = setting.VerbConjureSetting.Conjure.Tara;
    partial void OnTaraChanged(bool value)
    {
        Tara = value;
        setting.VerbConjureSetting.Conjure.Tara = value;
    }

    [ObservableProperty]
    private bool imperative = setting.VerbConjureSetting.Conjure.Imperative;
    partial void OnImperativeChanged(bool value)
    {
        Imperative = value;
        setting.VerbConjureSetting.Conjure.Imperative = value;
    }

    [ObservableProperty]
    private bool volitional = setting.VerbConjureSetting.Conjure.Volitional;
    partial void OnVolitionalChanged(bool value)
    {
        Volitional = value;
        setting.VerbConjureSetting.Conjure.Volitional = value;
    }

    [ObservableProperty]
    private bool conjunctive = setting.VerbConjureSetting.Conjure.Conjunctive;
    partial void OnConjunctiveChanged(bool value)
    {
        Conjunctive = value;
        setting.VerbConjureSetting.Conjure.Conjunctive = value;
    }

    [ObservableProperty]
    private bool te = setting.VerbConjureSetting.Conjure.Te;
    partial void OnTeChanged(bool value)
    {
        Te = value;
        setting.VerbConjureSetting.Conjure.Te = value;
    }

    [ObservableProperty]
    private bool sou = setting.VerbConjureSetting.Conjure.Sou;
    partial void OnSouChanged(bool value)
    {
        Sou = value;
        setting.VerbConjureSetting.Conjure.Sou = value;
    }

    [ObservableProperty]
    private bool tai = setting.VerbConjureSetting.Conjure.Tai;
    partial void OnTaiChanged(bool value)
    {
        Tai = value;
        setting.VerbConjureSetting.Conjure.Tai = value;
    }

    [ObservableProperty]
    private bool zu = setting.VerbConjureSetting.Conjure.Zu;
    partial void OnZuChanged(bool value)
    {
        Zu = value;
        setting.VerbConjureSetting.Conjure.Zu = value;
    }

    [ObservableProperty]
    private bool tari = setting.VerbConjureSetting.Conjure.Tari;
    partial void OnTariChanged(bool value)
    {
        Tari = value;
        setting.VerbConjureSetting.Conjure.Tari = value;
    }


}

public partial class FormSettingModel : ObservableObject
{
    private static readonly Setting setting = Setting.Instance;

    [ObservableProperty]
    private bool dictionary = setting.VerbConjureSetting.Form.Dictionary;
    partial void OnDictionaryChanged(bool value)
    {
        this.Dictionary = value;
        setting.VerbConjureSetting.Form.Dictionary = value;
    }

    [ObservableProperty]
    private bool potential = setting.VerbConjureSetting.Form.Potential;
    partial void OnPotentialChanged(bool value)
    {
        Potential = value;
        setting.VerbConjureSetting.Form.Potential = value;
    }

    [ObservableProperty]
    private bool potentialShort = setting.VerbConjureSetting.Form.PotentialShort;
    partial void OnPotentialShortChanged(bool value)
    {
        PotentialShort = value;
        setting.VerbConjureSetting.Form.PotentialShort = value;
    }

    [ObservableProperty]
    private bool passive = setting.VerbConjureSetting.Form.Passive;
    partial void OnPassiveChanged(bool value)
    {
        Passive = value;
        setting.VerbConjureSetting.Form.Passive = value;
    }

    [ObservableProperty]
    private bool causative = setting.VerbConjureSetting.Form.Causative;
    partial void OnCausativeChanged(bool value)
    {
        Causative = value;
        setting.VerbConjureSetting.Form.Causative = value;
    }

    [ObservableProperty]
    private bool causativeShort = setting.VerbConjureSetting.Form.CausativeShort;
    partial void OnCausativeShortChanged(bool value)
    {
        CausativeShort = value;
        setting.VerbConjureSetting.Form.CausativeShort = value;
    }

    [ObservableProperty]
    private bool causativePassive = setting.VerbConjureSetting.Form.CausativePassive;
    partial void OnCausativePassiveChanged(bool value)
    {
        CausativePassive = value;
        setting.VerbConjureSetting.Form.CausativePassive = value;
    }

    [ObservableProperty]
    private bool causativePassiveShort = setting.VerbConjureSetting.Form.CausativePassiveShort;
    partial void OnCausativePassiveShortChanged(bool value)
    {
        CausativePassiveShort = value;
        setting.VerbConjureSetting.Form.CausativePassiveShort = value;
    }

}

public partial class ModifierSettingModel : ObservableObject
{
    private static readonly Setting setting = Setting.Instance;

    [ObservableProperty]
    private bool none = setting.VerbConjureSetting.Modifier.None;
    partial void OnNoneChanged(bool value)
    {
        None = value;
        setting.VerbConjureSetting.Modifier.None = value;
    }

    [ObservableProperty]
    private bool polite = setting.VerbConjureSetting.Modifier.Polite;
    partial void OnPoliteChanged(bool value)
    {
        Polite = value;
        setting.VerbConjureSetting.Modifier.Polite = value;
    }

    [ObservableProperty]
    private bool casual = setting.VerbConjureSetting.Modifier.Casual;
    partial void OnCasualChanged(bool value)
    {
        Casual = value;
        setting.VerbConjureSetting.Modifier.Casual = value;
    }

    [ObservableProperty]
    private bool past = setting.VerbConjureSetting.Modifier.Past;
    partial void OnPastChanged(bool value)
    {
        Past = value;
        setting.VerbConjureSetting.Modifier.Past = value;
    }

    [ObservableProperty]
    private bool nonPast = setting.VerbConjureSetting.Modifier.NonPast;
    partial void OnNonPastChanged(bool value)
    {
        NonPast = value;
        setting.VerbConjureSetting.Modifier.NonPast = value;
    }

    [ObservableProperty]
    private bool negative = setting.VerbConjureSetting.Modifier.Negative;
    partial void OnNegativeChanged(bool value)
    {
        Negative = value;
        setting.VerbConjureSetting.Modifier.Negative = value;
    }

    [ObservableProperty]
    private bool nonNegative = setting.VerbConjureSetting.Modifier.NonNegative;
    partial void OnNonNegativeChanged(bool value)
    {
        NonNegative = value;
        setting.VerbConjureSetting.Modifier.NonNegative = value;
    }


}

public partial class VerbConjureSettingModel : ObservableObject
{
    [ObservableProperty]
    private ModifierSettingModel modifier = new();
    
    [ObservableProperty]
    private FormSettingModel form = new();

    [ObservableProperty]
    private ConjureSettingModel conjure = new();
}
