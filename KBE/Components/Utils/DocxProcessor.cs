using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using System.Diagnostics;
using KBE.Enums;
using KBE.Models;
using KBE.Components.Settings;

namespace KBE.Components.Utils
{
    public class DocxProcessor
    {
        public string Title { get; set; } = "Kanji";
        public string Location { get; set; } = ".\\output.docx";
        MainDocumentPart? mainPart;
        public DocxProcessor() { }
        public DocxProcessor(string title, string location)
        {
            Title = title;
            Location = location;
        }

        public async Task<EErrorType> CreateKanjiDocument(List<KanjiWord> kanjis, SaveOptions options)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(Location))
                    {
                        File.Delete(Location);
                    }
                    File.Copy($"{Setting.Directory}\\Data\\Template\\template.docx", Location);


                    using WordprocessingDocument doc = WordprocessingDocument.Open($@"{Location}", true);


                    var main = doc.MainDocumentPart;
                    if (main is null)
                    {
                        main = doc.AddMainDocumentPart();
                    }
                    mainPart = main;
                    mainPart.Document = new();

                    AddTitle(Title);

                    foreach (var kanji in kanjis)
                    {
                        AddKanji(kanji, options);
                    }

                    mainPart.Document.Save();
                    Debug.Write("B");
                    return EErrorType.NORMAL;
                }
                catch (IOException ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.GetType());
                    return EErrorType.FILE_IN_USE;
                }
            });
        }

        private void AddKanji(KanjiWord kanji, SaveOptions option)
        {
            var kanjiProperties = kanji.GetProperties(option);
            var kanjiKey = nameof(option.Kanji);
            var sinoVietnameseKey = nameof(option.SinoVietnamese);

            var isFirst = true;

            if (kanjiProperties.ContainsKey(kanjiKey) &&
                kanjiProperties.ContainsKey(sinoVietnameseKey))
            {
                AddHeading1($"{kanjiProperties[kanjiKey]} - {kanjiProperties[sinoVietnameseKey]}", kanji.Color);
                kanjiProperties.Remove(kanjiKey);
                kanjiProperties.Remove(sinoVietnameseKey);

                isFirst = false;
            }
            else if (kanjiProperties.ContainsKey(kanjiKey) &&
                !kanjiProperties.ContainsKey(sinoVietnameseKey))
            {
                AddHeading1($"{kanjiProperties[kanjiKey]}", kanji.Color);
                kanjiProperties.Remove(kanjiKey);

                isFirst = false;
            }

            kanjiProperties.Remove(nameof(kanji.Color));

            foreach (var (key, value) in kanjiProperties)
            {
                if (isFirst)
                {
                    AddHeading1($"{key}: {value}");
                }
                else
                {
                    AddText($"{key}: {value}");
                }
            }
        }

        private void AddTitle(string text, string textColor = "")
        {
            AddTextWithProperty(text, "Title", textColor);
        }

        private void AddHeading1(string text, string textColor = "")
        {
            AddTextWithProperty(text, "Heading1", textColor);
        }

        private void AddHeading2(string text, string textColor = "")
        {
            AddTextWithProperty(text, "Heading2", textColor);
        }


        private void AddTextWithProperty(string text, string styleId, string textColor = "")
        {
            if (mainPart is null)
            {
                return;
            }

            Body body = mainPart.Document.AppendChild(new Body());
            Paragraph para = body.AppendChild(new Paragraph());
            Run run = para.AppendChild(new Run());
            run.AppendChild(new Text(text));
            para.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = styleId });

            run.RunProperties ??= new RunProperties();
            Color? color = run.RunProperties.Elements<Color>().FirstOrDefault();

            if (color != null)
            {
                run.RunProperties.RemoveChild(color);
            }

            if (string.Compare(textColor, "", StringComparison.Ordinal) == 0)
            {
                return;
            }

            run.RunProperties.Append(new Color() { Val = textColor });
        }

        private void AddText(string text)
        {
            if (mainPart is null)
            {
                return;
            }

            Body body = mainPart.Document.AppendChild(new Body());
            Paragraph para = body.AppendChild(new Paragraph());
            Run run = para.AppendChild(new Run());
            run.AppendChild(new Text(text));
        }

    }

}