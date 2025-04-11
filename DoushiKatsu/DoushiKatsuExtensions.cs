using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoushiKatsu;

public static class DoushiKatsuExtensions
{
    public static string ToDisplay(this FormType type) => type switch
    {
        FormType.DICTIONARY => "Dictionary",
        FormType.POTENTIAL => "Potential",
        FormType.POTENTIAL_SHORT => "Potential (Short)",
        FormType.PASSIVE => "Passive",
        FormType.CAUSATIVE => "Causative",
        FormType.CAUSATIVE_SHORT => "Causative (Short)",
        FormType.CAUSATIVE_PASSIVE => "Causative-Passive",
        FormType.CAUSATIVE_PASSIVE_SHORT => "Causative-Passive (Short)",
        _ => throw new InvalidEnumArgumentException(),
    };

    public static string ToDisplay(this ConjureType type) => type switch
    {
        ConjureType.NONE => "None",
        ConjureType.CONDITIONAL_BA => "Cond. (ば)",
        ConjureType.CONDITIONAL_TARA => "Cond. (たら)",
        ConjureType.IMPERATIVE => "Imperative",
        ConjureType.VOLITIONAL => "Volitional",
        ConjureType.CONJUNCTIVE => "Conjunctive",
        ConjureType.TE => "て",
        ConjureType.SOU => "そう",
        ConjureType.TAI => "たい",
        ConjureType.ZU => "ず",
        ConjureType.TARI => "たり",
        ConjureType.SPECIAL => "None",
        _ => throw new InvalidEnumArgumentException(),
    };

}
