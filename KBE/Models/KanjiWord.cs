using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Components;
using KBE.Components.Settings;
using KBE.Components.SQL;
using KBE.Components.Utils;
using KBE.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KBE.Models
{
    public partial class KanjiWord : ObservableObject
    {

        [ObservableProperty]
        [Key]
        private string kanji = "";

        [ObservableProperty]
        private string sinoVietnamese = "";
        
        [ObservableProperty]
        private string onyumi = "";
        
        [ObservableProperty]
        private string kunyumi = "";
        
        [ObservableProperty]
        private string level = "";
        
        private string english = "";

        public string English
        {
            get => english;
            set
            {
                SetProperty(ref english, value);
                OnPropertyChanged(nameof(FirstEnglish));
            }
        }

        
        [ObservableProperty]
        private string vietnamese = "";
        
        [ObservableProperty]
        private string strokes = "";
        
        [ObservableProperty]
        private string radicals = "";
        
        [ObservableProperty]
        private string parts = "";
        
        [ObservableProperty]
        private string taught = "";

        private string? color = null;

        [NotMapped]
        public string? Color { 
            get { return color; }
            set {
                switch (value)
                {
                    case EKanjiColor.DEFAULT:
                    case EKanjiColor.RED:
                    case EKanjiColor.YELLOW:
                    case EKanjiColor.GREEN:
                        SetProperty(ref color, value);

                        break;
                    default:
                        return;
                }
            } 
        }


        public void ResetColor()
        {
            Color = EKanjiColor.DEFAULT;
        }

        public string FirstEnglish => english.Split(",")[0];

        public KanjiWord() { }

        public KanjiWord(string kanji = "", string sinoVietnamese = "", string on = "", string kun = "", string level = "",
            string english = "", string vietnamese = "", string strokes = "", string radicals = "", string parts = "", string taught = "")
        {
            this.kanji = kanji ?? "ERROR";
            this.sinoVietnamese = sinoVietnamese ?? "";
            onyumi = on ?? "";
            kunyumi = kun ?? "";
            this.level = level ?? "";
            this.english = english ?? "";
            this.vietnamese = vietnamese ?? "" ;
            this.strokes = strokes ?? "";
            this.radicals = radicals ?? "";
            this.parts = parts ?? "";
            this.taught = taught ?? "";

            color = null;
        }


        public static List<KanjiWord> GenerateDummies()
        {
            return new() {
                new() {english= "one, one radical (no.1)", kanji= "一", level="5", sinoVietnamese="NHẤT",strokes="1 strokes", taught="Jōyō kanji, taught in grade 1", radicals="one 一", parts="一", onyumi="ひと- ひと.つ", kunyumi="イチ イツ" ,vietnamese="Một, là số đứng đầu các số đếm. Phàm vật gì chỉ có một đều gọi là Nhất cả.##Cùng. Như sách Trung Dung nói : Cập kì thành công nhất dã [及其成工一也] nên công cùng như nhau vậy.##Dùng về lời nói hoặc giả thế chăng. Như vạn nhất [萬一] muôn một, nhất đán [一旦] một mai, v.v.##Bao quát hết thẩy. Như nhất thiết [一切] hết thẩy, nhất khái [一概] một mực như thế cả, v.v.##Chuyên môn về một mặt. Như nhất vị [一味] một mặt, nhất ý [一意] một ý, v.v." },
                new() {english= "seven", kanji= "七" }
            };
        }

        public static KanjiWord GenerateDummy()
        {
            return new() { english = "one, one radical (no.1)", kanji = "一", level = "5", sinoVietnamese = "NHẤT", strokes = "1 strokes", taught = "Jōyō kanji, taught in grade 1", radicals = "one 一", parts = "一", onyumi = "ひと- ひと.つ", kunyumi = "イチ イツ", vietnamese = "Một, là số đứng đầu các số đếm. Phàm vật gì chỉ có một đều gọi là Nhất cả.##Cùng. Như sách Trung Dung nói : Cập kì thành công nhất dã [及其成工一也] nên công cùng như nhau vậy.##Dùng về lời nói hoặc giả thế chăng. Như vạn nhất [萬一] muôn một, nhất đán [一旦] một mai, v.v.##Bao quát hết thẩy. Như nhất thiết [一切] hết thẩy, nhất khái [一概] một mực như thế cả, v.v.##Chuyên môn về một mặt. Như nhất vị [一味] một mặt, nhất ý [一意] một ý, v.v." };
        }

        public KanjiWord Clone() => new()
        {
            english = english ?? "",
            kanji = kanji ?? "",
            sinoVietnamese = sinoVietnamese ?? "",
            onyumi = onyumi ?? "",
            kunyumi = kunyumi ?? "",
            level = level ?? "",
            vietnamese = vietnamese ?? "",
            strokes = strokes ?? "",
            radicals = radicals ?? "",
            taught = taught ?? ""
        };
        
        public KanjiWord UpdateFrom(KanjiWord kanji)
        {
            English = kanji.english;
            Kanji = kanji.kanji;
            SinoVietnamese = kanji.sinoVietnamese;
            Onyumi = kanji.onyumi;
            Kunyumi = kanji.kunyumi;
            Level = kanji.level;
            Vietnamese = kanji.vietnamese;
            Strokes = kanji.strokes;
            Radicals = kanji.radicals;
            Taught = kanji.taught;

            OnPropertyChanged(nameof(English));

            return this;
        }

        public string[] ToList()
        {
            return new string[11] { kanji, sinoVietnamese, onyumi, kunyumi, level, english, vietnamese, strokes, radicals, parts, taught };
        }

        public static KanjiWord ToKanjiWord(string[] strings)
        {
            if (strings.Length != 11)
            {
                throw new IncorrectLengthError($"needed {typeof(KanjiWord).GetProperties().Length}, got {strings.Length}");
            }

            return new KanjiWord(kanji: strings[0], sinoVietnamese: strings[1], on: strings[2], kun: strings[3], level: strings[4],
                english: strings[5], vietnamese: strings[6], strokes: strings[7], radicals: strings[8], parts: strings[9],
                taught: strings[10]);

        }

        public string? GetValueOf(string propertyName)
        {
			var prop = GetType().GetProperty(propertyName);
			if (prop is null)
			{
				return null;
			}

			if (prop.PropertyType != typeof(string))
			{
				return null;
			}


            return prop.GetValue(this, null) as string;
		}

        public Dictionary<string,string?> GetProperties(SearchOptions sqlKanjiOption)
        {
            var sqlMember = sqlKanjiOption.GetType().GetProperties();
            Dictionary<string,string?> properties = new();   

            foreach (var item in sqlMember)
            {
                if (item.PropertyType != typeof(bool))
                {
                    continue;
                }

                var value = item.GetValue(sqlKanjiOption, null);
                if (value is null)
                {
                    continue;
                }


                var name = item.Name;
                if ((bool)value == false)
                {
                    continue;
                }

                var propVal = GetValueOf(name);
                if (propVal is null)
                {
                    continue;
                }

                properties.Add(name, propVal);
            }
            return properties;
        }

        public bool ApplyFilter(List<string> filters, SearchOptions sqlKanjiOption, bool isLossySearch = true)
        {
            var properties = GetProperties(sqlKanjiOption);

            foreach (var (_, value) in properties)
            {

                if (value is null)
                {
                    continue;
                }

                if (Search(value, filters))
                {
                    return true;
                }
            }
            return false;
        }


        private bool Search(string propVal, IEnumerable<string> filters)
        {
            return filters.Where(filter => filter.Length != 0)
                .Any(filter => propVal.Contains(filter, StringComparison.InvariantCultureIgnoreCase));
        }

        public string ToString(SearchOptions sqlKanjiOptions)
        {
            var properties = GetProperties(sqlKanjiOptions);
            StringBuilder stringBuilder = new();

            foreach (var (name, value) in properties)
            {
                if (value is null)
                {
                    continue;
                }
                stringBuilder.Append($"{name}: {value}\n");
            }
            return stringBuilder.ToString();
        }

    }


}
