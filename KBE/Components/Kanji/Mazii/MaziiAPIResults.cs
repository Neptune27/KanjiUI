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

namespace KBE.Components.Kanji.Mazii
{
    public class MaziiAPIResults
    {
        public List<MaziiCompDetail> compDetail { get; set; } = [];
        public List<string> level { get; set; } = [];
        public string kun { get; set; } = "";
        public string on { get; set; } = "";
        public string mean { get; set; } = "";
        public string detail { get; set; } = "";
    }
}
