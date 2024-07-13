using KBE.Components.Settings;
using KBE.Models;
using System;
using System.Collections.Generic;
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
		return res;
	}
	public static string ToHtml(IReadOnlyList<JapanesePhoneme> phonemeWithRomanjis)
	{
		var res = "<ruby>";
		for (int i = 0; i < phonemeWithRomanjis.Count; i++)
		{

			var phoneme = phonemeWithRomanjis[i];


			var displayText = phoneme.DisplayText;
			var yomiText = phoneme.YomiText;
			var isIn = HiraRomanji.TryGetValue(yomiText, out var romanji);
            if (!isIn) 
            {
				romanji = CombineRomanji(yomiText);
            }


			if (i < phonemeWithRomanjis.Count - 1)
			{
				var nextPhoneme = phonemeWithRomanjis[i + 1];
				var isNextIn = HiraRomanji.TryGetValue(yomiText + nextPhoneme.DisplayText,
					out var nextRomanji);

				if (isNextIn)
				{
					romanji = nextRomanji;
					displayText += nextPhoneme.DisplayText;
					yomiText += nextPhoneme.YomiText;
					i++;
				}
			}


			res += $"""
					<ruby>
					    {displayText}
					    <rt>{yomiText}</rt>
					</ruby>
					<rt>{romanji}</rt>
					<rb></rb>
					""";
        }

		return res + "</ruby>";
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
		{"しゃ","sya"},{"しゅ","syu"},{"しょ","syo"},
		{"ちゃ","tya"},{"ちゅ","tyu"},{"ちょ","tyo"},
		{"にゃ","nya"},{"にゅ","nyu"},{"にょ","nyo"},
		{"ひゃ","hya"},{"ひゅ","hyu"},{"ひょ","hyo"},
		{"みゃ","mya"},{"みゅ","myu"},{"みょ","myo"},
		{"りゃ","rya"},{"りゅ","ryu"},{"りょ","ryo"},


		{"ぎゃ","gya"},{"ぎゅ","gyu"},{"ぎょ","gyo"},
		{"じゃ","zya"},{"じゅ","zyu"},{"じょ","zyo"},
		{"ぢゃ","dya"},{"ぢゅ","dyu"},{"ぢょ","dyo"},
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

	public static async Task<List<IReadOnlyList<JapanesePhoneme>>> ToFurigana(string text, bool isMono = false)
	{
		var textChunks = NewLineRegex().Split(text);
		return await Task.Run(() =>
		{
			return textChunks.AsParallel().AsOrdered().Select(c => JapanesePhoneticAnalyzer.GetWords(c,isMono)).ToList();
		});
	}

	public static async Task<string> ToFuriganaText(string text)
	{
		return string.Join("\n",(await ToFurigana(text)).AsParallel().AsOrdered().Select(ToFuriganaTextChunk));
	}

	public static async Task<string> ToFuriganaRomanjiHtml(string text)
	{
		var withRomanjis = (await ToFurigana(text, true));
		var t = withRomanjis
			.Select(JapanesePhonemeWithRomanji.ToHtml);

		var res = "";
		foreach (var phrase in t)
		{
			res += $"<p>{phrase}</p>";
		}
		return res;
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
