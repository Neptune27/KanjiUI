using KBE.Components.Kanji;
using KBE.Components.Settings;
using KBE.Components.Utils;
using KBE.Models;
using KUnsafe;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Globalization;

namespace KanjiUI.Utils;

public record JapanesePhonemeWithRomanji
{

	public static string CombineRomanji(string furigana)
	{
		var res = "";
		furigana += " ";
		for (int i = 0; i < furigana.Length - 1; i++)
		{
			var current = furigana[i];
			var next = furigana[i + 1];
			HiraRomanji.TryGetValue(current.ToString(), out var romanji);
			var isIn = HiraRomanji.TryGetValue(current.ToString() + next, out var nextRomanji);
			if (isIn)
			{
				res += nextRomanji;
				i++;
				continue;
			}

			res += romanji;
		}

		if (Setting.Instance.ConnectedRomanji)
		{
			return ToCombinationRomanji(res);
		}

		return res;
	}

	public static string ToCombinationRomanji(string romanji, string next = "")
	{
		if (string.IsNullOrWhiteSpace(romanji))
		{
			return romanji;
		}

        if (romanji == "-")
        {
			return next[0].ToString();
        }

		var newString = string.Empty;

        for (int i = 0; i < romanji.Length - 1; i++)
        {
            var current = romanji[i];
			if (current == '-')
			{
				var nextChar = romanji[i + 1];
				newString += nextChar;
			}
			else
			{
				newString += current;
			}
        }

        if (romanji[^1] == '-' && !string.IsNullOrWhiteSpace(next))
        {
			newString += next[0];
        }
		else
		{
			newString += romanji[^1];
		}


		return newString;

    }


	public static string ToHtml(IList<JapanesePhoneme> phonemeWithRomanjis)
	{
		
		var res = "<p><ruby>";
		for (int i = 0; i < phonemeWithRomanjis.Count; i++)
		{

			var phoneme = phonemeWithRomanjis[i];

			var displayText = phoneme.DisplayText;
			var yomiText = phoneme.YomiText;


            if (yomiText == " ")
			{
				res += "&nbsp";
				continue;
			}

            if (displayText == "\r" || displayText == "\n")
            {
				res += "</ruby></p><p><ruby>";
				continue;

			}

            var isIn = HiraRomanji.TryGetValue(yomiText, out var romanji);
            if (!isIn) 
            {
				romanji = CombineRomanji(yomiText);

            }

			if (i < phonemeWithRomanjis.Count - 1)
			{
				var nextPhoneme = phonemeWithRomanjis[i + 1];
				var isDigraphs = HiraRomanji.TryGetValue(yomiText + nextPhoneme.DisplayText,
					out var nextDigraphs);


				

				if (Setting.Instance.ConnectedRomanji)
				{
					if (romanji == "-" || (romanji.Length > 1 && romanji[^1] == '-'))
					{
						var isNextRomanji = HiraRomanji.TryGetValue(nextPhoneme.YomiText[0].ToString(),
							out var nextRomanji);
						if (isNextRomanji)
						{
							romanji = ToCombinationRomanji(romanji, nextRomanji);
						}
					}


                }

				//For きょ,しょ,...
				if (isDigraphs)
				{
					romanji = nextDigraphs;
					displayText += nextPhoneme.DisplayText;
					yomiText += nextPhoneme.YomiText;
					i++;
				}



			}





			if (!Setting.Instance.FuriganaHiragana)
			{
				yomiText = "";
			}
            if (!Setting.Instance.FuriganaRomanji)
            {
				romanji = "";
            }


            res += $"""
					<ruby>{displayText}<rt>{yomiText}</rt></ruby><rt>{romanji}</rt><rb></rb>
					""";
        }

		return res + "</ruby></p>";
		//return $"""
		//	<ruby>
		//	    <ruby>{DisplayText}
		//	        <rt>{YomiText}</rt>
		//	    </ruby>
		//	    <rt>{RomanjiText}</rt>
		//	</ruby>
		//	""";
	}



	public static IReadOnlyDictionary<string, string> HiraRomanji = new Dictionary<string, string>()
	{
		{"１","1"},{"２","2"},{"３","3"},{"４","4"},{"５","5"},{"６","6"},{"７","7"},{"８","8"},{"９","9"},{"０","0"},
		{"！","!"},{"“","\""},{"”","\""},{"＃","#"},{"＄","$"},{"％","%"},{"＆","&"},{"’","'"},{"（","("},{"）",")"},
		{"＝","="},{"～","~"},{"｜","|"},{"＠","@"},{"‘","`"},{"＋","+"},{"＊","*"},{"；",";"},{"：",":"},{"＜","<"},
		{"＞",">"},{"、",","},{"。","."},{"／","/"},{"？","?"},{"＿","_"},{"・","･"},{"「","\""},{"」","\""},{"｛","{"},
		{"｝","}"},{"￥","\\"},{"＾","^"},

		{"あ","a"},{"い","i"},{"う","u"},{"え","e"},{"お","o"},
		{"か","ka"},{"き","ki"},{"く","ku"},{"け","ke"},{"こ","ko"},
		{"さ","sa"},{"し","shi"},{"す","su"},{"せ","se"},{"そ","so"},
		{"た","ta"},{"ち","chi"},{"つ","tsu"},{"て","te"},{"と","to"},
		{"な","na"},{"に","ni"},{"ぬ","nu"},{"ね","ne"},{"の","no"},
		{"は","ha"},{"ひ","hi"},{"ふ","fu"},{"へ","he"},{"ほ","ho"},
		{"ま","ma"},{"み","mi"},{"む","mu"},{"め","me"},{"も","mo"},
		{"ら","ra"},{"り","ri"},{"る","ru"},{"れ","re"},{"ろ","ro"},

		{"や","ya"},{"ゆ","yu"},{"よ","yo"},
		{"わ","wa"},{"ゐ","wi"},{"ゑ","we"},{"を","wo"},

		{"が","ga"},{"ぎ","gi"},{"ぐ","gu"},{"げ","ge"},{"ご","go"},
		{"ざ","za"},{"じ","ji"},{"ず","zu"},{"ぜ","ze"},{"ぞ","zo"},
		{"だ","da"},{"ぢ","dji"},{"づ","dzu"},{"で","de"},{"ど","do"},
		{"ば","ba"},{"び","bi"},{"ぶ","bu"},{"べ","be"},{"ぼ","bo"},
		{"ぱ","pa"},{"ぴ","pi"},{"ぷ","pu"},{"ぺ","pe"},{"ぽ","po"},

		{"きゃ","kya"},{"きゅ","kyu"},{"きょ","kyo"},
		{"しゃ","sha"},{"しゅ","shu"},{"しょ","sho"},
		{"ちゃ","cha"},{"ちゅ","chu"},{"ちょ","cho"},
		{"にゃ","nya"},{"にゅ","nyu"},{"にょ","nyo"},
		{"ひゃ","hya"},{"ひゅ","hyu"},{"ひょ","hyo"},
		{"みゃ","mya"},{"みゅ","myu"},{"みょ","myo"},
		{"りゃ","rya"},{"りゅ","ryu"},{"りょ","ryo"},


		{"ぎゃ","gya"},{"ぎゅ","gyu"},{"ぎょ","gyo"},
		{"じゃ","ja"},{"じゅ","ju"},{"じょ","jo"},
		{"ぢゃ","dja"},{"ぢゅ","dju"},{"ぢょ","djo"},
		{"びゃ","bya"},{"びゅ","byu"},{"びょ","byo"},
		{"ぴゃ","pya"},{"ぴゅ","pyu"},{"ぴょ","pyo"},
		{"くゎ","kwa"},{"ぐゎ","gwa"},{"ギャ","gya"},
		{"っ", "-" },

		{"ぁ","a"},{"ぃ","i"},{"ぅ","u"},{"ぇ","e"},{"ぉ","o"},{"ゃ","ya"},{"ゅ","yu"},{"ょ","yo"},{"ゎ","wa"},
		{"ァ","a"},{"ィ","i"},{"ゥ","u"},{"ェ","e"},{"ォ","o"},{"ャ","ya"},{"ュ","yu"},{"ョ","yo"},{"ヮ","wa"},
		{"ヵ","ka"},{"ヶ","ke"},{"ん","n"},{"ン","n"},{" "," "},{"いぇ","ye"},{"きぇ","kye"},{"くぃ","kwi"},
		{"くぇ","kwe"},{"くぉ","kwo"},{"ぐぃ","gwi"},{"ぐぇ","gwe"},{"ぐぉ","gwo"},{"イェ","ye"},{"キェ","kya"},
		{"クィ","kwi"},{"クェ","kwe"},{"クォ","kwo"},{"グィ","gwi"},{"グェ","gwe"},{"グォ","gwo"},{"しぇ","sye"},
		{"じぇ","zye"},{"すぃ","swi"},{"ずぃ","zwi"},{"ちぇ","tye"},{"つぁ","twa"},{"つぃ","twi"},{"つぇ","twe"},
		{"つぉ","two"},{"にぇ","nye"},{"ひぇ","hye"},{"ふぁ","hwa"},{"ふぃ","hwi"},{"ふぇ","hwe"},{"ふぉ","hwo"},
		{"ふゅ","hwyu"},{"ふょ","hwyo"},{"シェ","sye"},{"ジェ","zye"},{"スィ","swi"},{"ズィ","zwi"},{"チェ","tye"},
		{"ツァ","twa"},{"ツィ","twi"},{"ツェ","twe"},{"ツォ","two"},{"ニェ","nye"},{"ヒェ","hye"},{"ファ","hwa"},
		{"フィ","hwi"},{"フェ","hwe"},{"フォ","hwo"},{"フュ","hwyu"},{"フョ","hwyo"},

	};


}

public partial class FuriganaHelpers
{

	public static async Task<string> GetFuriganaTextBySetting(string text, string code)
	{
		if (code != "Japanese")
		{
			return text;
		}

		if (!Setting.Instance.Furigana)
		{
			return text;
		}
		return await ToFuriganaText(text);
	}

	private static readonly IReadOnlyList<string> _seperatedList = ["\n", "\r"];


	public static async Task<List<IReadOnlyList<JapanesePhoneme>>> ToFurigana(string text, bool isMono = false)
	{
		//var textChunks = NewLineRegex().Split(text);
		var textChunks = text.ToChunks(90, _seperatedList);
	
        if (Setting.Instance.UnsafeJapaneseAnalyzer)
        {
			return await Task.Run(() =>
			{
				try
				{
					return textChunks.AsParallel().AsOrdered()
										.Select(c => JapanesePhoneticAnalyzerUnsafe.GetWords(c, isMono)).ToList();
				}
				catch (Exception ex)
				{
					Setting.Logger.Error("[FuriganaHelper] [ToFurigana] It's that thing again.");
					Setting.Logger.Error("[FuriganaHelper] [ToFurigana] Error: {@message}.", ex.Message);
					Setting.Instance.UnsafeJapaneseAnalyzer = false;
					Setting.Instance.SaveSetting();
					throw;
				}
				
			});
		}
		else
		{

			try
			{
				return textChunks.Select(c => JapanesePhoneticAnalyzer.GetWords(c, isMono)).ToList();
			}
			catch (Exception ex)
			{
				Setting.Logger.Error("[FuriganaHelper] [ToFurigana] Error 2: {@message}.", ex.Message);
				throw;
			}

		}




	}

	public static async Task<string> ToFuriganaText(string text)
	{
		return string.Join("\n",(await ToFurigana(text)).AsParallel().AsOrdered().Select(ToFuriganaTextChunk));
	}

	public static async Task<string> ToFuriganaRomanjiHtml(string text)
	{
		var withRomanjis = await ToFurigana(text, true);
		
		//Combine it into a single list for accurate conversion.
		var htmlList = new List<JapanesePhoneme>();

		//Why the fuck does IReadOnlyList does not support SelectMany, Select, or ForEach.
		//Then what's the point of using that interface then.
		//I thought after the api call for Japanese Phoneme it'll convert to freaking List
		//and not just a raw array masquerade as a IReadOnlyList, why???. And this is from Microsoft too.
		//My dissappointment is immeasurable and my day is ruined because I have to do this.
		//Freaking Windows.Globalization... Why???
		foreach (var chunk in withRomanjis)
		{
            for (var i = 0; i < chunk.Count; i++)
            {
				var phoneme = chunk[i];
                htmlList.Add(phoneme);
            }
        }

		return string.Join("",JapanesePhonemeWithRomanji.ToHtml(htmlList));
	}


	private static string ToFuriganaTextChunk(IReadOnlyList<JapanesePhoneme> yomiChunks)
	{
		var result = string.Empty;
		foreach (var phoneme in yomiChunks)
		{
			if (phoneme.DisplayText == phoneme.YomiText)
			{
				result += phoneme.DisplayText;
			}
			else
			{
				result += $" {phoneme.DisplayText}({phoneme.YomiText}) ";
			}
		}

		return result;
	}

	[GeneratedRegex(@"[\n\r]")]
	private static partial Regex NewLineRegex();
}
