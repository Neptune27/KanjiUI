using CommunityToolkit.Mvvm.ComponentModel;
using DoushiKatsu;
using KBE.Components.Settings;
using KBE.Components.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Models;

public partial class VerbConjure(JapaneseVerb baseVerb) : ObservableObject
{

    public static readonly ImmutableList<JapaneseVerb> CuratedSpecial = 
        [new JapaneseVerb("する", VerbType.SURU), new JapaneseVerb("来る", VerbType.KURU)];

    public static readonly ImmutableList<string> CuratedGodanString = [
    "死ぬ",    "買う",    "合う",    "使う",    "歌う",    "言う",    "洗う",    "吸う",    "違う",    "行く",
    "泳ぐ",    "急ぐ",    "防ぐ",    "注ぐ",    "脱ぐ",    "繋ぐ",    "担ぐ",    "騒ぐ",    "外す",
    "学ぶ",    "飛ぶ",    "呼ぶ",    "遊ぶ",    "並ぶ",    "喜ぶ",    "運ぶ",    "転ぶ",
    "読む",    "飲む",    "休む",    "住む",    "生む",    "頼む",    "望む",    "囲む",
    "痛む",    "聞く",    "書く",    "歩く",    "働く",    "開く",    "着く",    "引く",
    "履く",    "持つ",    "立つ",    "育つ",    "待つ",    "経つ",    "打つ",    "役立つ",
    "撃つ",    "走る",    "作る",    "帰る",    "入る",    "売る",    "座る",    "乗る",
    "困る",    "話す",    "出す",    "返す",    "消す",    "貸す",    "渡す",    "表す",
    ];

    public static readonly ImmutableList<string> CuratedIchidanString = [
    "食る",    "見る",    "寝る",    "出る",    "起きる",    "答える",    "覚える",    "着る"
        ];
    public static readonly ImmutableList<JapaneseVerb> CuratedGodan = [.. CuratedGodanString.Select(x => new JapaneseVerb(x, VerbType.GODAN))];
    public static readonly ImmutableList<JapaneseVerb> CuratedIchidan = [.. CuratedGodanString.Select(x => new JapaneseVerb(x, VerbType.ICHIDAN))];
    public static readonly ImmutableList<JapaneseVerb> CuratedJapaneseVerbs = [..CuratedSpecial, ..CuratedGodan, ..CuratedIchidan];

    public static readonly List<VerbConjure> ConjuredList = [];

    public static void GenerateConjuredList()
    {
        //ConjuredList.Add(new VerbConjure(new("合う", VerbType.ICHIDAN)));
        ConjuredList.AddRange(CuratedJapaneseVerbs.Select(v => new VerbConjure(v.Clone())));
    }

    public VerbConjure CloneWithoutSubmit()
    {
        if (CuratedGodanString.Contains(Verb.DictionaryForm))
        {
            return new VerbConjure(new(Verb.DictionaryForm, VerbType.GODAN));
        }
        if (CuratedIchidanString.Contains(Verb.DictionaryForm))
        {
            return new VerbConjure(new(Verb.DictionaryForm, VerbType.ICHIDAN));
        }
        if (Verb.DictionaryForm == "する")
        {
            return new VerbConjure(new(Verb.DictionaryForm, VerbType.SURU));
        }
        if (Verb.DictionaryForm == "来る")
        {
            return new VerbConjure(new(Verb.DictionaryForm, VerbType.KURU));
        }
        throw new Exception("WTF");
    }


    public JapaneseVerb Verb { get; } = baseVerb;

    public string DictionaryForm => Verb.DictionaryForm;


    [ObservableProperty]
    private string content = baseVerb.Content;

    [ObservableProperty]
    private VerbType baseType = baseVerb.BaseType;

    [ObservableProperty]
    private bool submitted;

    partial void OnBaseTypeChanged(VerbType value)
    {
        Verb.BaseType = value;
        Content = Verb.ToText();
    }

    [ObservableProperty]
    private FormType formType;

    public bool IsModifierEnable(string option)
    {
        if (Submitted)
        {
            return false;
        }

        if (Enum.TryParse(option, out ModifierType result))
        {
            return Verb.CanEnable(result);
        }

        throw new InvalidEnumArgumentException();
    }

    partial void OnFormTypeChanged(FormType value)
    {
        if (Submitted)
        {
            FormType = Verb.FormType;
            return;
        }

        Verb.FormType = value;
        Content = Verb.ToText();
    }

    [ObservableProperty]
    private ConjureType conjureType;

    partial void OnConjureTypeChanged(ConjureType value)
    {
        if (Submitted)
        {
            ConjureType = Verb.ConjureType;
            return;
        }

        Verb.ConjureType = value;
        UpdateModifier();
        OnPropertyChanged(nameof(IsModifierEnable));
        Content = Verb.ToText();
    }


    private void UpdateModifier()
    {
        IsNegative = Verb.IsNegative;
        IsPolite = Verb.IsPolite;
        IsPast = Verb.IsPast;
    }


    [ObservableProperty]
    private bool isPast;

    partial void OnIsPastChanged(bool value)
    {

        if (Submitted)
        {
            IsPast = Verb.IsPast;
            return;
        }

        Verb.SetModifier(ModifierType.PAST, value);
        if (IsPast != Verb.IsPast)
        {
            IsPast = Verb.IsPast;
            return;
        }

        Content = Verb.ToText();
        OnPropertyChanged(nameof(IsModifierEnable));

    }

    [ObservableProperty]
    private bool isNegative;

    partial void OnIsNegativeChanged(bool value)
    {

        if (Submitted)
        {
            IsNegative = Verb.IsNegative;
            return;
        }

        Verb.SetModifier(ModifierType.NEGATIVE, value);
        if (IsNegative != Verb.IsNegative)
        {
            IsNegative = Verb.IsNegative;
            return;
        }

        Content = Verb.ToText();
        OnPropertyChanged(nameof(IsModifierEnable));

    }

    [ObservableProperty]
    private bool isPolite;

    partial void OnIsPoliteChanged(bool value)
    {

        if (Submitted)
        {
            IsPolite = Verb.IsPolite;
            return;
        }

        Verb.SetModifier(ModifierType.POLITE, value);
        if (IsPolite != Verb.IsPolite)
        {
            IsPolite = Verb.IsPolite;
            return;
        }

        Content = Verb.ToText();
        OnPropertyChanged(nameof(IsModifierEnable));

    }

    private bool IsConjureSelected(ConjureType conjureType)
    {
        var conjureSetting = Setting.Instance.VerbConjureSetting.Conjure;

        return conjureType switch
        {
            ConjureType.NONE => conjureSetting.None,
            ConjureType.CONDITIONAL_BA => conjureSetting.Ba,
            ConjureType.CONDITIONAL_TARA => conjureSetting.Tara,
            ConjureType.IMPERATIVE => conjureSetting.Imperative,
            ConjureType.VOLITIONAL => conjureSetting.Volitional,
            ConjureType.CONJUNCTIVE => conjureSetting.Conjunctive,
            ConjureType.TE => conjureSetting.Te,
            ConjureType.SOU => conjureSetting.Sou,
            ConjureType.TAI => conjureSetting.Tai,
            ConjureType.ZU => conjureSetting.Zu,
            ConjureType.TARI => conjureSetting.Tari,
            _ => false,
        };
    }

    private bool IsFormSelected(FormType formType)
    {
        var formSetting = Setting.Instance.VerbConjureSetting.Form;

        return formType switch
        {
            FormType.DICTIONARY => formSetting.Dictionary,
            FormType.POTENTIAL => formSetting.Potential,
            FormType.POTENTIAL_SHORT => formSetting.PotentialShort,
            FormType.PASSIVE => formSetting.Passive,
            FormType.CAUSATIVE => formSetting.Causative,
            FormType.CAUSATIVE_SHORT => formSetting.CausativeShort,
            FormType.CAUSATIVE_PASSIVE => formSetting.CausativePassive,
            FormType.CAUSATIVE_PASSIVE_SHORT => formSetting.CausativePassiveShort,
            _ => false,
        };
    }


    public void Randomized()
    {
        Random random = new();

        var isNegative = random.Next(100) < 50;
        var isPolite = random.Next(100) < 50;
        var isPast = random.Next(100) < 50;

        var conjureType = Enum.GetValues<ConjureType>().Where(IsConjureSelected).PickRandom();
        var formType = Enum.GetValues<FormType>().Where(IsFormSelected).PickRandom();
        //var conjureType = Enum.GetValues<ConjureType>().PickRandom();

        ConjureType = conjureType;
        FormType = formType;
        IsPolite = isPolite;
        IsPast = isPast;
        IsNegative = isNegative;
    }
}
