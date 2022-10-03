﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KBE.Components
{
    public class FilterProcessingOption{
        public bool isKanjiEnable = true;
        public bool isLossySearch = true;
        public bool isOnlyKanj = false;
    }

    public class KanjiProcessor
    {
        private static readonly Dictionary<char, char> CHINESE_REPLACE = new()
        {
            {'⼀', '一'},{'⼆', '二'},{'⼈', '人'},{'⼄', '乙'},{'丨', '丨'},{'⼃', '丿'},
            {'⼅', '亅'},{'⼇', '亠'},{'⼉', '儿'},{'⼊', '入'},{'⼋', '八'},{'⼌', '冂'},
            {'⼍', '冖'},{'⼎', '冫'},{'⼏', '几'},{'⼐', '凵'},{'⼑', '刀'},{'⼒', '力'},
            {'⼓', '勹'},{'⼔', '匕'},{'⼕', '匚'},{'⼖', '匸'},{'⼗', '十'},{'⼘', '卜'},
            {'⼙', '卩'},{'⼚', '厂'},{'⼛', '厶'},{'⼜', '又'},{'⼝', '口'},{'⼞', '囗'},
            {'⼟', '土'},{'⼠', '士'},{'⼡', '夂'},{'⼢', '夊'},{'⼣', '夕'},{'⼤', '大'},
            {'⼥', '女'},{'⼦', '子'},{'⼧', '宀'},{'⼨', '寸'},{'⼩', '小'},{'⼪', '尢'},
            {'⼫', '尸'},{'⼬', '屮'},{'⼭', '山'},{'⼮', '巛'},{'⼯', '工'},{'⼰', '己'},
            {'⼱', '巾'},{'⼲', '干'},{'⼳', '幺'},{'⼴', '广'},{'⼵', '廴'},{'⼶', '廾'},
            {'⼷', '弋'},{'⼸', '弓'},{'⼹', '彐'},{'⼺', '彡'},{'⼻', '彳'},{'⼼', '心'},
            {'⼽', '戈'},{'⼾', '戸'},{'⼿', '手'},{'⽀', '支'},{'⽁', '攴'},{'⽂', '文'},
            {'⽃', '斗'},{'⽄', '斤'},{'⽅', '方'},{'⽆', '无'},{'⽇', '日'},{'⽈', '曰'},
            {'⽉', '月'},{'⽊', '木'},{'⽋', '欠'},{'⽌', '止'},{'⽍', '歹'},{'⽎', '殳'},
            {'⽏', '毋'},{'⽐', '比'},{'⽑', '毛'},{'⽒', '氏'},{'⽓', '气'},{'⽔', '水'},
            {'⽕', '火'},{'⽖', '爪'},{'⽗', '父'},{'⽘', '爻'},{'⽙', '爿'},{'⽚', '片'},
            {'⽛', '牙'},{'⽜', '牛'},{'⽝', '犬'},{'⽞', '玄'},{'⽟', '玉'},{'⽠', '瓜'},
            {'⽡', '瓦'},{'⽢', '甘'},{'⽣', '生'},{'⽤', '用'},{'⽥', '田'},{'⽦', '疋'},
            {'⽧', '疒'},{'⽨', '癶'},{'⽩', '白'},{'⽪', '皮'},{'⽫', '皿'},{'⽬', '目'},
            {'⽭', '矛'},{'⽮', '矢'},{'⽯', '石'},{'⽰', '示'},{'⽱', '禸'},{'⽲', '禾'},
            {'⽳', '穴'},{'⽴', '立'},{'⽵', '竹'},{'⽶', '米'},{'⽷', '糸'},{'⽸', '缶'},
            {'⽹', '网'},{'⽺', '羊'},{'⽻', '羽'},{'⽼', '老'},{'⽽', '而'},{'⽾', '耒'},
            {'⽿', '耳'},{'⾀', '聿'},{'⾁', '肉'},{'⾂', '臣'},{'⾃', '自'},{'⾄', '至'},
            {'⾅', '臼'},{'⾆', '舌'},{'⾇', '舛'},{'⾈', '舟'},{'⾉', '艮'},{'⾊', '色'},
            {'⾋', '艸'},{'⾌', '虍'},{'⾍', '虫'},{'⾎', '血'},{'⾏', '行'},{'⾐', '衣'},
            {'⾑', '襾'},{'⾒', '見'},{'⾓', '角'},{'⾔', '言'},{'⾕', '谷'},{'⾖', '豆'},
            {'⾗', '家'},{'⾘', '豸'},{'⾙', '貝'},{'⾚', '赤'},{'⾛', '走'},{'⾜', '足'},
            {'⾝', '身'},{'⾞', '車'},{'⾟', '辛'},{'⾠', '辰'},{'⾡', '辵'},{'⾢', '邑'},
            {'⾣', '酉'},{'⾤', '釆'},{'⾥', '里'},{'⾦', '金'},{'⾧', '長'},{'⾨', '門'},
            {'⾩', '阜'},{'⾪', '隶'},{'⾫', '隹'},{'⾬', '雨'},{'⾭', '靑'},{'⾮', '非'},
            {'⾯', '面'},{'⾰', '革'},{'⾱', '韋'},{'⾲', '韭'},{'⾳', '音'},{'⾴', '頁'},
            {'⾵', '風'},{'⾶', '飛'},{'⾷', '食'},{'⾸', '首'},{'⾹', '香'},{'⾺', '馬'},
            {'⾻', '骨'},{'⾼', '高'},{'⾽', '髟'},{'⾾', '鬥'},{'⾿', '鬯'},{'⿀', '鬲'},
            {'⿁', '鬼'},{'⿂', '魚'},{'⿃', '鳥'},{'⿄', '鹵'},{'⿅', '鹿'},{'⿆', '麥'},
            {'⿇', '麻'},{'⿈', '黃'},{'⿉', '黍'},{'⿊', '黒'},{'⿋', '黹'},{'⿌', '黽'},
            {'⿍', '鼎'},{'⿎', '鼓'},{'⿏', '鼠'},{'⿐', '鼻'},{'⿑', '齊'},{'⿒', '歯'},
            {'⿓', '龍'},{'⿔', '龜'},{'⿕', '龠'}
        };

        public static List<string> GetKanjis(string words)
        {
            List<string> kanjis = new();
            foreach (var item in CHINESE_REPLACE)
            {
                words = words.Replace(item.Key, item.Value);
            }

            foreach (var letter in words)
            {
                if (IsKanji(letter))
                {
                    kanjis.Add(letter.ToString());
                }
            }
            return kanjis;
        }

        public static bool IsKanji(char letter)
        {
            switch (letter)
            {
                case >= '\u2E80' and <= '\u2EFF':
                case >= '\u2F00' and <= '\u2FDF':
                case >= '\u3000' and <= '\u303f':
                case >= '\u31C0' and <= '\u31ef':
                case >= '\u3400' and <= '\u4dbf':
                case >= '\u4e00' and <= '\u9fff':
                case >= '\uf900' and <= '\ufaff':
                    return true;
                default:
                    return false;
            }
        }
        public static List<string> FilterProcessing(string filter, FilterProcessingOption option)
        {
            List<string> filters = new() { filter };

            var processFilter = Regex.Replace(filter, @"[,./\\;]+", "");
            processFilter = Regex.Replace(processFilter, @"[\n]+", " ");

            if (option.isOnlyKanj)
            {
                filters.Clear();
                processFilter = Regex.Replace(processFilter, @"[a-z A-Z0-9\ ]+", "");
                processFilter = new string(processFilter.ToCharArray().Distinct().ToArray());
                option.isLossySearch = false;
            }

            if (option.isKanjiEnable)
            {
                filters.AddRange(GetKanjis(processFilter));
            }

            if (option.isLossySearch)
            {
                filters.AddRange(processFilter.Split());
            }

            return filters;
        }
    }
}
