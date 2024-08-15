using KBE.Components.Settings;
using KBE.Enums;
using KBE.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.Utils
{
    public class TextProcessor
    {
        public string Title { get; set; } = "Kanji";
        public string Location { get; set; } = ".\\output.docx";

        public TextProcessor() { }
        public TextProcessor(string title, string location)
        {
            Title = title;
            Location = location;
        }

        public async Task<EErrorType> CreateFile(List<KanjiWord> kanjis, SaveOptions options)
        {
            try
            {
                if (File.Exists(Location))
                {
                    File.Delete(Location);
                }

                StreamWriter sw = new(Location);
                StringBuilder sb = new StringBuilder();
                foreach (var kanji in kanjis)
                {
                    var kanjiProperties = kanji.GetProperties(options);
                    kanjiProperties.Remove(nameof(options.Color));
                    foreach (var (key, value) in kanjiProperties)
                    {
                        sb.AppendLine($"{key}: {value}");
                    }
                    sb.AppendLine();
                }

                await sw.WriteAsync(sb.ToString());
                sw.Close();
                return EErrorType.NORMAL;
            }
            catch (IOException ex) { 
                Setting.Logger.Error("[TextProcessor] [CreateFile] Error: {@error}",ex.ToString());
                return EErrorType.FILE_IN_USE;
            }

        }
    }
}
