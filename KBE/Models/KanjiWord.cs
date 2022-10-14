using CommunityToolkit.Mvvm.ComponentModel;
using KBE.Components;
using KBE.Components.Settings;
using KBE.Components.SQL;
using KBE.Components.Utils;
using KBE.Enums;
using System;
using System.Collections.Generic;
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
        private string kanji = "";
        
        [ObservableProperty]
        private string sinoVietnamese = "";
        
        [ObservableProperty]
        private string onyumi = "";
        
        [ObservableProperty]
        private string kunyumi = "";
        
        [ObservableProperty]
        private string level = "";
        
        [ObservableProperty]
        private string english = "";
        
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

        private string color = EKanjiColor.DEFAULT;

        public string Color { 
            get { return color; }
            set {
                switch (value)
                {
                    case EKanjiColor.DEFAULT:
                    case EKanjiColor.RED:
                    case EKanjiColor.GREEN:
                        SetProperty(ref color, value);
                        break;
                    default:
                        return;
                }
            } 
        }


        public string FirstEnglish => english.Split(",")[0];

        public KanjiWord(string kanji = "", string sinoVietnamese = "", string on = "", string kun = "", string level = "",
            string english = "", string vietnamese = "", string strokes = "", string radicals = "", string parts = "", string taught = "")
        {
            this.kanji = kanji;
            this.sinoVietnamese = sinoVietnamese;
            onyumi = on;
            kunyumi = kun;
            this.level = level;
            this.english = english;
            this.vietnamese = vietnamese;
            this.strokes = strokes;
            this.radicals = radicals;
            this.parts = parts;
            this.taught = taught;
        }


        public static List<KanjiWord> GenerateDummy()
        {
            return new() {
                new() {english= "one, one radical (no.1)", kanji= "一", level="5", sinoVietnamese="NHẤT",strokes="1 strokes", taught="Jōyō kanji, taught in grade 1", radicals="one 一", parts="一", onyumi="ひと- ひと.つ", kunyumi="イチ イツ" ,vietnamese="Một, là số đứng đầu các số đếm. Phàm vật gì chỉ có một đều gọi là Nhất cả.##Cùng. Như sách Trung Dung nói : Cập kì thành công nhất dã [及其成工一也] nên công cùng như nhau vậy.##Dùng về lời nói hoặc giả thế chăng. Như vạn nhất [萬一] muôn một, nhất đán [一旦] một mai, v.v.##Bao quát hết thẩy. Như nhất thiết [一切] hết thẩy, nhất khái [一概] một mực như thế cả, v.v.##Chuyên môn về một mặt. Như nhất vị [一味] một mặt, nhất ý [一意] một ý, v.v." },
                new() {english= "seven", kanji= "七" }
            };
        }

        public KanjiWord Clone() => new()
        {
            english = english,
            kanji = kanji,
            sinoVietnamese = sinoVietnamese,
            onyumi = onyumi,
            kunyumi = kunyumi,
            level = level,
            vietnamese = vietnamese,
            strokes = strokes,
            radicals = radicals,
            taught = taught
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

                var prop = GetType().GetProperty(name);
                if (prop is null)
                {
                    continue;
                }

                if (prop.PropertyType != typeof(string))
                {
                    continue;
                }


                string? propVal = prop.GetValue(this, null) as string;
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

                var res = Search(value, filters);

                if (res)
                {
                    return true;
                }
            }
            return false;
        }


        private bool Search(string propVal, List<string> filters)
        {
            foreach (var filter in filters)
            {
                if (propVal.Contains(filter, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
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
