using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KBE.Components
{

    public class Setting
    {

#if !DEBUG
        static string Directory = ".";
        static string FilePath = $"{Directory}\\Setting\\setting.json";
#elif DEBUG
        static string Directory { get; set; } = "..\\..\\..\\..\\..";
        static string FilePath { get; set; } = $"{Directory}\\Setting\\setting.json";
#endif
        public static JsonSerializerOptions SaveOptions { get; set; } = new() { WriteIndented = true };

        public int FetchSize { get; set; }
        public string DatabaseConnectDirectory { get; set; } = "";
        public SQLKanjiOptions SQLKanjiOption { get; set; } = new();

        public bool LossySearch { get; set; } = true;

        public static Setting Instance { get; private set; } = MakeSetting();

        public static Setting GetSetting()
        {
            return Instance;
        }

        static string ReadFromFile()
        {
            string output = "";
            try
            {
                using (var sr = new StreamReader(FilePath))
                {
                    // Read the stream as a string, and write the string to the console.
                    output = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return output;
        }

        static Setting MakeSetting()
        {
            try
            {
                Setting? setting = JsonSerializer.Deserialize<Setting>(ReadFromFile());

                if (setting is null)
                {
                    throw new NullReferenceException();
                }
                return setting;

            }
            catch (Exception e) when (e is JsonException || e is NullReferenceException)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);


                ResetToAndSaveDefaultSetting();
                return MakeSetting();
            }

        }

        public static void ResetToAndSaveDefaultSetting()
        {
            var defaultSetting = new Setting
            {
                FetchSize = 100,
                DatabaseConnectDirectory = $"{Directory}\\Data\\DefaultDatabase.db",
                SQLKanjiOption = new SQLKanjiOptions(),
                LossySearch = true
            };

            System.IO.Directory.CreateDirectory($"{Directory}\\Setting");
            System.IO.Directory.CreateDirectory($"{Directory}\\Data");
            System.IO.Directory.CreateDirectory($"{Directory}\\Data\\Img");

            using (StreamWriter sw = new($"{FilePath}"))
            {
                var str = JsonSerializer.Serialize(defaultSetting, SaveOptions);
                sw.Write(str);
            }
            Instance = defaultSetting;
        }

        public void SaveSetting()
        {
            using StreamWriter sw = new($"{FilePath}");
            Debug.WriteLine(JsonSerializer.Serialize(this, SaveOptions));
            sw.Write(JsonSerializer.Serialize(this, SaveOptions));
        }

    }
}

