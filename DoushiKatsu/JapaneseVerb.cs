using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DoushiKatsu;


public class InvalidJapaneseVerbType : Exception
{
    public InvalidJapaneseVerbType()
        : base("The verb type provided is invalid.")
    { }

    public InvalidJapaneseVerbType(string message)
        : base(message)
    { }

    public InvalidJapaneseVerbType(string message, Exception inner)
        : base(message, inner)
    { }
}

public static class JapaneseVerbTransformExtension
{


    private static void HandleSpecialForm(JapaneseVerb verb)
    {
        switch (verb.FormType)
        {
            case FormType.DICTIONARY:
                if (verb.IsConjunctive())
                {
                    verb.Add("t");
                    verb.MakeIchidan();
                }
                break;
            case FormType.CAUSATIVE_SHORT:
                verb.MakeGodan();
                break;
            default:
                verb.MakeIchidan();
                break;
        }
    }
    private static JapaneseVerb HandleKuruForm(JapaneseVerb verb)
    {

        var valueToReplace = verb.FormType switch
        {
            FormType.DICTIONARY => verb.IsConjunctive() == true
                                                ? "来" 
                                                : verb.DictionaryForm,
            FormType.POTENTIAL or FormType.PASSIVE => "来られる",
            FormType.POTENTIAL_SHORT => "来れる",
            FormType.CAUSATIVE => "来させる",
            FormType.CAUSATIVE_SHORT => "来さす",
            FormType.CAUSATIVE_PASSIVE => "来させられる",
            FormType.CAUSATIVE_PASSIVE_SHORT => "来させられる",
            _ => throw new InvalidJapaneseVerbType(),
        };

        verb.SetContentTo(valueToReplace);

        HandleSpecialForm(verb);
      

        return verb;
    }

   

    private static JapaneseVerb HandleSuruForm(JapaneseVerb verb)
    {

        var valueToReplace = verb.FormType switch
        {
            FormType.DICTIONARY => verb.IsConjunctive() == true
                                                ? "し"
                                                : verb.DictionaryForm,
            FormType.POTENTIAL => "できる",
            FormType.PASSIVE => "される",
            FormType.POTENTIAL_SHORT => "できる",
            FormType.CAUSATIVE => "させる",
            FormType.CAUSATIVE_SHORT => "さす",
            FormType.CAUSATIVE_PASSIVE => "させられる",
            FormType.CAUSATIVE_PASSIVE_SHORT => "させられる",
            _ => throw new InvalidJapaneseVerbType(),
        };

        verb.SetContentTo(valueToReplace);

        HandleSpecialForm(verb);


        return verb;

    }

    private static JapaneseVerb HandleIchidanForm(JapaneseVerb verb)
    {
        return verb.FormType switch
        {
            FormType.DICTIONARY => verb,
            FormType.POTENTIAL => verb.ReplaceEnding("られる"),
            FormType.POTENTIAL_SHORT => verb.ReplaceEnding("れる"),
            FormType.PASSIVE => verb.ReplaceEnding("られる"),
            FormType.CAUSATIVE => verb.ReplaceEnding("させる"),
            FormType.CAUSATIVE_SHORT => verb.ReplaceEnding("さす"),
            FormType.CAUSATIVE_PASSIVE => verb.ReplaceEnding("させられる"),
            FormType.CAUSATIVE_PASSIVE_SHORT => verb.ReplaceEnding("させられる"),
            _ => throw new InvalidJapaneseVerbType(),
        };
    }


    private static JapaneseVerb Conjunctive(this JapaneseVerb verb)
    {

        if (verb.DictionaryForm == "来る" || verb.DictionaryForm == "する")
        {
            return verb.ReplaceEnding("");
        }

        return verb.Type switch
        {
            _ => verb.SwapVowelOfLastKana('い'),
        };
    }

    private static JapaneseVerb SwapAdd(this JapaneseVerb verb, char swapChar, string add)
    {
        verb.SwapVowelOfLastKana(swapChar);
        verb.Add(add);
        return verb;
    }

    private static JapaneseVerb SwapAddMake(this JapaneseVerb verb,char swapChar, string add, VerbType type)
    {
        verb.SwapAdd(swapChar, add);

        switch (type)
        {
            case VerbType.GODAN:
                verb.MakeGodan();
                break;
            case VerbType.ICHIDAN:
                verb.MakeIchidan();
                break;
            case VerbType.SURU:
                break;
            case VerbType.KURU:
                break;
            case VerbType.UNKNOWN:
                break;
            default:
                break;
        }
        return verb;
    }

    private static JapaneseVerb Potential(this JapaneseVerb verb)
    {
        verb.SwapVowelOfLastKana('え');
        verb.Add("る");
        verb.MakeIchidan();
        return verb.SwapAddMake('え', "る", VerbType.ICHIDAN);
    }

    private static JapaneseVerb Passive(this JapaneseVerb verb)
    {
        return verb.SwapAddMake('あ', "れる", VerbType.ICHIDAN);
    }

    private static JapaneseVerb PotentialShort(this JapaneseVerb verb)
    {
        return verb.SwapAddMake('え', "る", VerbType.ICHIDAN);
    }

    private static JapaneseVerb Causative(this JapaneseVerb verb)
    {
        return verb.SwapAddMake('あ', "せる", VerbType.ICHIDAN);
    }

    private static JapaneseVerb CausativeShort(this JapaneseVerb verb)
    {
        return verb.SwapAddMake('あ', "す", VerbType.GODAN);
    }

    private static JapaneseVerb CausativePassive(this JapaneseVerb verb)
    {
        return verb.SwapAddMake('あ', "せられる", VerbType.ICHIDAN);
    }
    private static JapaneseVerb CausativePassiveShort(this JapaneseVerb verb)
    {
        if (verb.Content[^1] == 'す')
        {
            return verb.ReplaceEnding("させられる");
        }

        return verb.SwapAddMake('あ', "される", VerbType.ICHIDAN);
    }


    private static JapaneseVerb BasicConjuration(this JapaneseVerb verb, char swap, string replace)
    {
        if (verb.Type == VerbType.ICHIDAN)
        {
            return verb.ReplaceEnding(replace);
        }

        return verb.SwapAdd(swap, replace);

    }

    private static JapaneseVerb Negative(this JapaneseVerb verb)
    {

        if (verb.Content == "ある")
        {
            return verb.SetContentTo("ない");
        }

        return verb.BasicConjuration('あ', "ない");
    }

    private static JapaneseVerb Polite(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "ます");
    }

    private static JapaneseVerb Past(this JapaneseVerb verb)
    {
        verb.Te();
        _ = verb.Content[^1] == 'て' ? verb.ReplaceEnding("た") : verb.ReplaceEnding("だ");

        return verb;
    }

    private static JapaneseVerb NegativePast(this JapaneseVerb verb)
    {
        return verb.Negative().ReplaceEnding("かった");
    }

    private static JapaneseVerb PoliteNegative(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "ません");
    }

    private static JapaneseVerb PolitePast(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "ました");
    }

    private static JapaneseVerb PoliteNegativePast(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "ませんでした");
    }



    private static JapaneseVerb Te(this JapaneseVerb verb)
    {
        if (verb.Content == "する")
            return verb.SetContentTo("して");
        
        if (verb.Content == "来る")
            return verb.SetContentTo("来て");
        
        if (verb.Content == "行く")
            return verb.SetContentTo("行って");
        
        if (verb.Type == VerbType.ICHIDAN)
            return verb.ReplaceEnding("て");

        return verb.Content[..^1] switch
        {
            "す" => verb.ReplaceEnding("して"),
            "く" or "ぐ" => verb.ReplaceEnding("いて"),
            "む" or "ぶ" or "ぬ" => verb.ReplaceEnding("んで"),
            "る" or "う" or "つ" => verb.ReplaceEnding("って"),
            _ => verb,
        };
    }

    private static JapaneseVerb Tara(this JapaneseVerb verb)
    {
        verb.Te();
        _ = verb.Content[^1] == 'て' ? verb.ReplaceEnding("たら") 
                                    : verb.ReplaceEnding("だら");
        return verb;
    }

    private static JapaneseVerb TaraNegative(this JapaneseVerb verb)
    {
        return verb.Negative().ReplaceEnding("かったら");
    }

    private static JapaneseVerb TaraPolite(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "ましたら");
    }

    private static JapaneseVerb Tari(this JapaneseVerb verb)
    {
        verb.Te();
        _ = verb.Content[^1] == 'て' ? verb.ReplaceEnding("たり")
                                    : verb.ReplaceEnding("だり");
        return verb;
    }

    private static JapaneseVerb TariNegative(this JapaneseVerb verb)
    {
        return verb.Negative().ReplaceEnding("かったり");
    }

    private static JapaneseVerb Sou(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "そう");
    }

    private static JapaneseVerb SouNegative(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('あ', "なさそう");
    }

    private static JapaneseVerb Tai(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "たい");
    }

    private static JapaneseVerb TaiNegative(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "たくない");
    }

    private static JapaneseVerb TaiPast(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "たかった");
    }

    private static JapaneseVerb TaiNegativePast(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "たくなかった");
    }

    private static JapaneseVerb Naide(this JapaneseVerb verb)
    {
        return verb.Negative().Add("で");
    }

    private static JapaneseVerb Nakute(this JapaneseVerb verb)
    {
        return verb.Negative().ReplaceEnding("くて");
    }



    private static JapaneseVerb Conditional(this JapaneseVerb verb)
    {
        return verb.SwapAdd('え', "ば");
    }

    private static JapaneseVerb ConditionalNegative(this JapaneseVerb verb)
    {
        return verb.Negative().ReplaceEnding("ければ");
    }

    private static JapaneseVerb Imperative(this JapaneseVerb verb)
    {
        if (verb.Content == "来る")
        {
            return verb.ReplaceEnding("い");
        }

        if (verb.Type == VerbType.ICHIDAN)
        {
            return verb.ReplaceEnding("ろ");
        }

        return verb.SwapVowelOfLastKana('え');
    }
    private static JapaneseVerb ImperativePolite(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "なさい");
    }

    private static JapaneseVerb ImperativeNegative(this JapaneseVerb verb)
    {
        return verb.Add("な");
    }

    private static JapaneseVerb Volitional(this JapaneseVerb verb)
    {

        if (verb.Type == VerbType.ICHIDAN)
        {
            return verb.ReplaceEnding("よう");
        }

        return verb.SwapAdd('お', "う");
    }

    private static JapaneseVerb VolitionalPolite(this JapaneseVerb verb)
    {
        return verb.BasicConjuration('い', "ましょう");
    }

    private static JapaneseVerb Zu(this JapaneseVerb verb)
    {

        if (verb.Content == "する")
        {
            return verb.SetContentTo("せず");
        }

        return verb.BasicConjuration('あ', "ず");
    }



    private static JapaneseVerb TransformForm(JapaneseVerb verb)
    {

        if (verb.DictionaryForm == "来る")
        {
            return HandleKuruForm(verb);
        }
        
        if (verb.DictionaryForm == "する")
        {
            return HandleSuruForm(verb);
        }

        if (verb.Type == VerbType.ICHIDAN)
        {
            return HandleIchidanForm(verb);
        }

        return verb.FormType switch
        {
            FormType.DICTIONARY => verb,
            FormType.POTENTIAL => verb.Potential(),
            FormType.POTENTIAL_SHORT => verb.PotentialShort(),
            FormType.PASSIVE => verb.Passive(),
            FormType.CAUSATIVE => verb.Causative(),
            FormType.CAUSATIVE_SHORT => verb.CausativeShort(),
            FormType.CAUSATIVE_PASSIVE => verb.CausativePassive(),
            FormType.CAUSATIVE_PASSIVE_SHORT => verb.CausativePassiveShort(),
            _ => throw new InvalidJapaneseVerbType(),
        };
    }

    private static JapaneseVerb ConjureNone(JapaneseVerb verb)
    {
        if (verb.IsPolite && verb.IsNegative && verb.IsPast)
        {
            return verb.PoliteNegativePast();
        }

        if (verb.IsPolite && verb.IsNegative)
        {
            return verb.PoliteNegative();
        }

        if (verb.IsPolite && verb.IsPast)
        {
            return verb.PolitePast();
        }

        if (verb.IsNegative && verb.IsPast)
        {
            return verb.NegativePast();
        }

        if (verb.IsPast)
        {
            return verb.Past();
        }
        if (verb.IsPolite)
        {
            return verb.Polite();
        }

        if (verb.IsNegative)
        {
            return verb.Negative();
        }

        return verb;
    }

    private static JapaneseVerb TransformConjure(JapaneseVerb verb)
    {

        switch (verb.ConjureType)
        {
            case ConjureType.NONE:
            case ConjureType.SPECIAL:
                return ConjureNone(verb);
            case ConjureType.CONDITIONAL_BA:
                return verb.IsNegative 
                    ? verb.ConditionalNegative() 
                    : verb.Conditional();
            case ConjureType.CONDITIONAL_TARA:
                if (verb.IsNegative)
                {
                    return verb.TaraNegative();
                }
                if (verb.IsPolite)
                {
                    return verb.TaraPolite();
                }
                return verb.Tara();
            case ConjureType.IMPERATIVE:
                if (verb.IsNegative)
                {
                    return verb.ImperativeNegative();
                }
                if (verb.IsPolite)
                {
                    return verb.ImperativePolite();
                }
                return verb.Imperative();
            case ConjureType.VOLITIONAL:
                return verb.IsPolite
                    ? verb.VolitionalPolite()
                    : verb.Volitional();
            case ConjureType.CONJUNCTIVE:
                return verb.Conjunctive();
            case ConjureType.TE:
                if (verb.IsNegative)
                {
                    if (verb.AlternativeTe)
                    {
                        return verb.Naide();
                    }
                    return verb.Nakute();
                }
                return verb.Te();
            case ConjureType.SOU:
                return verb.IsNegative
                    ? verb.SouNegative()
                    : verb.Sou();
            case ConjureType.TAI:
                if (verb.IsNegative)
                {
                    if (verb.IsPast)
                    {
                        return verb.TaiNegativePast();
                    }
                    return verb.TaiNegative();
                }

                if (verb.IsPast)
                {
                    return verb.TaiPast();
                }
                return verb.Tai();
            case ConjureType.ZU:
                return verb.Zu();
            case ConjureType.TARI:
                return verb.IsNegative
                    ? verb.TariNegative()
                    : verb.Tari();
            default:
                throw new InvalidJapaneseVerbType();

        }
    }

    private static JapaneseVerb Preprocess(JapaneseVerb verb)
    {
        if (verb.DictionaryForm != "する" && verb.DictionaryForm != "来る")
        {
            return verb;
        }
        
        if (verb.FormType != FormType.DICTIONARY)
        {
            return verb;
        }

        if (verb.IsNegative || verb.IsPast || verb.IsPolite)
        {
            return verb.SetConjure(ConjureType.SPECIAL);
        }

        return verb;

    }

    public static JapaneseVerb Transform(this JapaneseVerb verb)
    {
        var baseConjure = verb.ConjureType;
        Preprocess(verb);
        TransformForm(verb);
        verb.SetConjure(baseConjure);

        TransformConjure(verb);

        return verb;
    }

}

public class JapaneseVerb(string dictionaryForm, VerbType type)
{

    public static readonly string Vowels = "あいうえお";
    public static readonly Dictionary<char, string> BaseTransformation = new()
    {
        { 'く', "かき　けこ" },
        { 'ぐ', "がぎ　げご" },
        { 'す', "さし　せそ" },
        { 'む', "まみ　めも" },
        { 'ぶ', "ばび　べぼ" },
        { 'ぬ', "なに　ねの" },
        { 'る', "らり　れろ" },
        { 'つ', "たち　てと" },
        { 'う', "わい　えお" }
    };

    public string DictionaryForm { get; } = dictionaryForm;
    public VerbType BaseType { get; set; } = type;
    public VerbType Type { get; set; } = type;

    public FormType FormType { get; set; }

    private ConjureType conjureType;

    public ConjureType ConjureType {
        get => conjureType;
        set {
            conjureType = value;
            UpdateModifier();
        } 
    
    }

    public bool IsPast { get; set; }

    public bool IsNegative { get; set; }

    public bool IsPolite { get; set; }

    public bool AlternativeTe { get; set; } = false;

    public string Content { get; set; } = dictionaryForm;

    public bool IsConjunctive()
    {
        return ConjureType switch
        {
            ConjureType.NONE 
            or ConjureType.CONDITIONAL_BA 
            or ConjureType.ZU => false,
            _ => true,
        };
    }

    public JapaneseVerb Default()
    {
        ConjureType = ConjureType.NONE;
        FormType = FormType.DICTIONARY;
        IsNegative = false;
        IsPolite = false;
        IsPast = false;
        Content = DictionaryForm;

        return this;
    }

    public JapaneseVerb Clone()
    {
        return new(dictionaryForm, BaseType)
        {
            AlternativeTe = AlternativeTe,
            ConjureType = ConjureType,
            Content = Content,
            IsPast = IsPast,
            IsNegative = IsNegative,
            IsPolite = IsPolite,
            FormType = FormType,
            Type = Type
        };
    }
    public bool CanEnable(ModifierType type)
    {
        if (ConjureType == ConjureType.CONDITIONAL_TARA || ConjureType == ConjureType.IMPERATIVE)
        {
            if (IsPolite && type == ModifierType.NEGATIVE)
            {
                return false;
            }

            if (IsNegative && type == ModifierType.POLITE)
            {
                return false;
            }
        }

        return ConjureType switch
        {
            ConjureType.NONE => true,
            ConjureType.CONJUNCTIVE or ConjureType.ZU => false,
            ConjureType.CONDITIONAL_TARA or ConjureType.IMPERATIVE => type switch
            {
                ModifierType.PAST => false,
                _ => true,
            },
            ConjureType.VOLITIONAL => type switch
            {
                ModifierType.PAST or ModifierType.NEGATIVE => false,
                _ => true,
            },
            ConjureType.TAI => type switch
            {
                ModifierType.POLITE => false,
                _ => true,
            },
            ConjureType.CONDITIONAL_BA or ConjureType.TE or ConjureType.SOU or ConjureType.TARI => type switch
            {
                ModifierType.PAST or ModifierType.POLITE => false,
                _ => true,
            },
            _ => true,
        };
    }

    public void MakeIchidan()
    {
        Type = VerbType.ICHIDAN;
    }

    public void MakeGodan()
    {
        Type = VerbType.GODAN;
    }

    public void MakeSpecial()
    {
        Type = VerbType.SPECIAL;
    }

    public JapaneseVerb SetContentTo(string newContent)
    {
        Content = newContent;
        return this;
    }

    private void UpdateModifier()
    {
        if (IsPast)
        {
            IsPast = CanEnable(ModifierType.PAST);
        }

        if (IsNegative)
        {
            IsNegative = CanEnable(ModifierType.NEGATIVE);
        }

        if (IsPolite)
        {
            IsPolite = CanEnable(ModifierType.POLITE);
        }

    }


    private static char TransformEnding(char lastKata, char baseVowel)
    {
        int vowelLocation = Vowels.IndexOf(baseVowel);
        return BaseTransformation[lastKata][vowelLocation];
    }

    public JapaneseVerb SwapVowelOfLastKana(char vowel)
    {
        var lastKana = Content[^1];
        var transformed = TransformEnding(lastKana, vowel);
        return ReplaceEnding(transformed);

    }

    public JapaneseVerb SetModifier(ModifierType modifier, bool value)
    {

        var canEnable = CanEnable(modifier);

        switch (modifier)
        {
            case ModifierType.NONE:
                IsNegative = false;
                IsPolite = false;
                IsPast = false;
                break;
            case ModifierType.PAST:
                IsPast = value ? canEnable : value;
                break;
            case ModifierType.POLITE:
                IsPolite = value ? canEnable : value;
                break;
            case ModifierType.NEGATIVE:
                IsNegative = value ? canEnable : value;
                break;
            default:
                break;
        }
        //UpdateModifier();
        return this;
    }
    public JapaneseVerb Negative => SetModifier(ModifierType.NEGATIVE, true);
    public JapaneseVerb Positive => SetModifier(ModifierType.NEGATIVE, false);
    public JapaneseVerb Casual => SetModifier(ModifierType.POLITE, false);
    public JapaneseVerb Polite => SetModifier(ModifierType.POLITE, true);
    public JapaneseVerb Past => SetModifier(ModifierType.PAST, true);
    public JapaneseVerb Present => SetModifier(ModifierType.PAST, false);

    public JapaneseVerb SetForm(FormType type)
    {
        FormType = type;
        return this;
    }

    public JapaneseVerb SetConjure(ConjureType type)
    {
        ConjureType = type;
        return this;
    }

    public JapaneseVerb ReplaceEnding(char value)
    {
        ReplaceEnding(value.ToString());

        return this;
    }
    public JapaneseVerb ReplaceEnding(string value)
    {
        Content = Content[..^1] + value;

        return this;

    }

    public JapaneseVerb Add(string value)
    {
        Content += value;
        return this;
    }


    public string ToText()
    {
        Content = DictionaryForm;
        Type = BaseType;
        var item = this.Transform().Content;
        
        return item;
    }
}
