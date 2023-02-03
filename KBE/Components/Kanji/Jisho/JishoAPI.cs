using HtmlAgilityPack;
//List<string> list = new () { "光" };
//var s = await KBE.WordController.FetchAll(list);
//int a = 0;

//string tmp;

//using (var sr = new StreamReader("..\\..\\..\\Test.txt"))
//{
//    // Read the stream as a string, and write the string to the console.
//    tmp = sr.ReadToEnd();
//}
//var a = JsonSerializer.Deserialize<KBE.MaziiAPI>(tmp);
////var d = JsonSerializer.Deserialize<KBE.MaziiAPIResults>(a.results[0].ToString());
namespace KBE.Components.Kanji.Jisho
{
    class JishoAPI
    {
        static HtmlDocument doc = new();

        public string strokes { get; set; } = "";
        public string english { get; set; } = "";
        public string taught { get; set; } = "";
        public string jlpt { get; set; } = "";
        public string radicals { get; set; } = "";


        private string CheckNullAndTrim(HtmlNode? node)
        {
            if (node is null)
            {
                return "N/A";
            }
            return node.InnerText.ToString().Replace("\n", "").Trim();
        }
        public JishoAPI(string htmlContent)
        {
            doc.LoadHtml(htmlContent);

            strokes = CheckNullAndTrim(doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[1]/div/div[2]/div[1]"));
            english = CheckNullAndTrim(doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[2]/div/div[1]/div[1]"));
            taught = CheckNullAndTrim(doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[2]/div/div[2]/div/div[1]"));
            jlpt = CheckNullAndTrim(doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[2]/div/div[2]/div/div[2]"));
            radicals = CheckNullAndTrim(doc.DocumentNode.SelectSingleNode("//*[@id='result_area']/div/div[1]/div[1]/div/div[2]/div[2]/dl/dd/span"));
        }
    }
}
